using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;

namespace OrderFlow_App.ApplicationServices.Services.Contracts
{
    public interface IApplicationService<TPost, TPut, TDelete, TGetById, TGetAll>
    {
        Task<IResponse<TPost>> PostAsync(TPost obj);
        Task<IResponse<TPut>> PutAsync(TPut obj);
        Task<IResponse<TDelete>> DeleteAsync(TDelete obj);
        Task<IResponse<List<TGetAll>>> GetAllAsync();
        Task<IResponse<TGetById>> GetByIdAsync(TGetById obj);
    }
}
