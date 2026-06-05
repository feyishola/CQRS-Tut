namespace OrdersAPI.Handlers
{
    public interface IQueryHandler<TQuery, TResult>
    {
        Task<TResult?> HandleAsync(TQuery query); // The method returns a Task that may contain a TResult or be null if the query does not yield a result.
    }
}