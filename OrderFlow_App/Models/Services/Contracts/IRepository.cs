using Azure;
using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;

namespace OrderFlow_App.Models.Services.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<IResponse<T>> InsertAsync(T entity);
        Task<IResponse<T>> UpdateAsync(T entity);
        Task<IResponse<T>> DeleteAsync(T entity);
        Task<IResponse<IEnumerable<T>>> SelectAllAsync();
        Task<IResponse<T>> SelectByIdAsync(T entity);
    }
}
