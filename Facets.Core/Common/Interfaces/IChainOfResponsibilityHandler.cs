using Facets.SharedKernal.Responses;

namespace Facets.Core.Common.Interfaces;

public interface IHandler<TModel, TResponse> where TModel : class
                                             where TResponse : class
{
    IHandler<TModel, TResponse> SetNextHandler(IHandler<TModel, TResponse> next);

    ResponseResult<TResponse?> Handle(TModel model);
}

public interface IHandler<TModel> where TModel : class
{
    IHandler<TModel> SetNextHandler(IHandler<TModel> next);

    ResponseResult Handle(TModel model);
}

public interface IAsyncHandler<TModel> where TModel : class
{
    IAsyncHandler<TModel> SetNextHandler(IAsyncHandler<TModel> next);

    Task<ResponseResult> Handle(TModel model);
}
