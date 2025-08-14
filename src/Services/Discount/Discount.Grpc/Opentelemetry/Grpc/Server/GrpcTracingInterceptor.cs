using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Discount.Grpc.Opentelemetry.Grpc.Server;

public class GrpcTracingInterceptor(TextMapPropagator propagator)
    : Interceptor
{
    private static readonly ActivitySource
        Source = new("Server.Grpc");

    public override async Task<TRes> UnaryServerHandler<TReq, TRes>(
        TReq request, ServerCallContext ctx,
        UnaryServerMethod<TReq, TRes> continuation)
    {
        var traceContext = propagator.Extract(default,
            ctx.RequestHeaders,
            static (headers, k) => [headers.GetValue(k)!]);
        Baggage.Current = traceContext.Baggage;
        using var activity = Source.StartActivity(ctx.Method,
            ActivityKind.Server, traceContext.ActivityContext);

        if (activity?.IsAllDataRequested != true)
        {
            return await continuation(request, ctx);
        }

        SetRpcAttributes(activity, ctx.Host, ctx.Method);

        try
        {
            var response = await continuation(request, ctx);
            SetStatus(activity, ctx.Status);
            return response;
        }
        catch (Exception ex)
        {
            SetStatus(activity, ex);
            throw;
        }
    }

    private static void SetRpcAttributes(Activity activity,
        string authority, string methodName)
    {
        GetHostAndPort(authority, out var host, out var port);
        GetServiceAndMethod(methodName, out var service,
            out var method);

        activity.SetTag("rpc.system", "grpc");
        activity.SetTag("rpc.service", service);
        activity.SetTag("rpc.method", method);
        activity.SetTag("net.host.name", host);
        if (port != 80 && port != 443)
        {
            activity.SetTag("net.host.port", port);
        }
    }

    private static void SetStatus(Activity activity,
        global::Grpc.Core.Status status)
    {
        activity.SetTag("rpc.grpc.status_code",
            (int)status.StatusCode);

        var activityStatusCode =
            ActivityStatusCode.Unset;
        if (status.StatusCode != global::Grpc.Core.StatusCode.OK)
        {
            activityStatusCode = ActivityStatusCode.Error;
        }

        if (status.DebugException != null)
        {
            activity.AddException(status.DebugException);
        }

        activity.SetStatus(activityStatusCode, status.Detail);
    }

    public override async Task DuplexStreamingServerHandler<TReq, TRes>(
        IAsyncStreamReader<TReq> requestStream,
        IServerStreamWriter<TRes> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TReq, TRes> continuation)
        where TReq : class
        where TRes : class
    {
        var traceContext =
            propagator.Extract(default, context.RequestHeaders,
                Getter);

        Baggage.Current = traceContext.Baggage;

        using var activity = Source.StartActivity(context.Method,
            ActivityKind.Server, traceContext.ActivityContext);

        if (activity?.IsAllDataRequested != true)
        {
            await continuation(requestStream, responseStream,
                context);
            return;
        }

        SetRpcAttributes(activity, context.Host, context.Method);

        var wrappedRequestStream =
            new RequestStreamWithEvents<TReq>(activity, requestStream);
        var wrappedResponseStream =
            new ResponseStreamWithEvents<TRes>(activity,
                responseStream);
        try
        {
            await continuation(wrappedRequestStream,
                wrappedResponseStream, context);
            SetStatus(activity, context.Status);
        }
        catch (Exception ex)
        {
            // if exception happens after first response, it won't be reflected in context.Status - request if over and this is a steram after.
            SetStatus(activity, ex);
            throw;
        }
    }

    private static IEnumerable<string> Getter(Metadata headers,
        string fieldName) =>
        headers.Where(h => h.Key == fieldName).Select(h => h.Value);

    private static void SetStatus(Activity activity,
        Exception exception)
    {
        activity.SetTag("rpc.grpc.status_code",
            (int)global::Grpc.Core.StatusCode.Unknown);

        var activityStatusCode =
            ActivityStatusCode.Error;

        activity.AddException(exception);

        activity.SetStatus(activityStatusCode, exception.Message);
    }

    private static void GetServiceAndMethod(string fullName,
        out string? service, out string method)
    {
        // could be a good idea to cache results
        var lastSlash = fullName.LastIndexOf('/');

        if (lastSlash == -1)
        {
            service = null;
            method = fullName;
        }
        else
        {
            service = fullName[..lastSlash];
            method = fullName[(lastSlash + 1)..];
        }
    }

    private static void GetHostAndPort(string authority,
        out string host, out int port)
    {
        // could be a good idea to cache results
        var colon = authority.IndexOf(':');

        if (colon == -1)
        {
            host = authority;
            port = 443;
        }
        else
        {
            host = authority[..colon];
            port = 443;
            var portStr = authority[(colon + 1)..];
            if (int.TryParse(portStr, out var p))
            {
                port = p;
            }
        }
    }


    private class ResponseStreamWithEvents<T>(
        Activity activity,
        IServerStreamWriter<T> inner)
        : IServerStreamWriter<T>
    {
        public int ResponsesSent => _responsesSent;
        private int _responsesSent = 0;

        public WriteOptions? WriteOptions
        {
            get => inner.WriteOptions;
            set => inner.WriteOptions = value;
        }

        public async Task WriteAsync(T message)
        {
            await inner.WriteAsync(message);
            ActivityTagsCollection tags = new()
            {
                { "message.type", "SENT" },
                {
                    "message.id",
                    Interlocked.Increment(ref _responsesSent)
                },
            };

            activity.AddEvent(new ActivityEvent("message",
                tags: tags));
        }
    }

    private class RequestStreamWithEvents<T>(
        Activity activity,
        IAsyncStreamReader<T> inner)
        : IAsyncStreamReader<T>
    {
        public int RequestsReceived => _requestsReseived;
        private int _requestsReseived = 0;

        public T Current => inner.Current;

        public async Task<bool> MoveNext(
            CancellationToken cancellationToken)
        {
            if (await inner.MoveNext(cancellationToken))
            {
                ActivityTagsCollection tags = new()
                {
                    { "message.type", "RECEIVED" },
                    {
                        "message.id",
                        Interlocked.Increment(ref _requestsReseived)
                    },
                };

                activity.AddEvent(
                    new ActivityEvent("message", tags: tags));

                return true;
            }

            return false;
        }
    }
}
