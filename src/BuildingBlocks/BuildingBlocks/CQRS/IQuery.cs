using MediatR;

namespace BuildingBlocks.CQRS;

public interface IQuery<out TReponse> : IRequest<TReponse>
    where TReponse : notnull { }
