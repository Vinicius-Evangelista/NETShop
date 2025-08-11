using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Basket.Api.OpenTelemetry.Processors;

public class BasketBaggageProcessor : BaseProcessor<Activity>
{
    /// <summary>
    /// Ignores the baggage if the activity is a server request
    /// </summary>
    /// <param name="activity"></param>
    public override void OnEnd(Activity activity)
    {
        if (activity.Kind == ActivityKind.Server &&
            activity.GetTagItem("http.method") as string
            == "GET" &&
            activity.GetTagItem("http.route") == null)
        {
            activity.ActivityTraceFlags &=
                ~ActivityTraceFlags.Recorded;
        }

    }

    public override void OnStart(Activity activity)
    {
        var basketName = activity.Baggage
            .FirstOrDefault(b => b.Key == "basket.name").Value;
        if (!string.IsNullOrEmpty(basketName))
        {
            activity.SetTag("basket.name", basketName);
        }
    }
}
