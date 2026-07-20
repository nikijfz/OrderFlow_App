using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.Models.DomainModels.OrderAggregates;
using OrderFlow_App.Frameworks.ResponseFrameworks;
using OrderFlow_App.Models.Services.Contracts;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OrderFlow_App.Frameworks;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Net;


namespace OrderFlow_App.Models.Services.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        #region [- Private Fields -]
        private readonly ProjectDbContext _context;
        #endregion

        #region [- Ctor() -]
        public OrderRepository(ProjectDbContext context)
        {
            _context = context;
        }
        #endregion

        #region [- InsertOrderJsonAsync() -]
        public async Task<IResponse<bool>> InsertOrderJsonAsync(string orderJson)
        {
            try
            {
                var parameter = new Microsoft.Data.SqlClient.SqlParameter("@JsonData", orderJson);
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.Sp_InsertOrder @JsonData", parameter);

                return new Response<bool>(true, HttpStatusCode.Created, ResponseMessages.SuccessfullOperation, true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, false);
            }
        }
        #endregion

        #region [- UpdateAsync() -]
        public async Task<IResponse<bool>> UpdateOrderJsonAsync(string orderJson)
        {
            try
            {
                var parameter = new Microsoft.Data.SqlClient.SqlParameter("@JsonData", orderJson);
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.Sp_UpdateOrder @JsonData", parameter);

                return new Response<bool>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, false);
            }
        }
        #endregion

        #region [- DeleteAsync() -]
        public async Task<IResponse<OrderHeader>> DeleteAsync(OrderHeader orderHeader)
        {
            try
            {
                var jsonString = SerializeOrder(new { GuidKey = orderHeader.GuidKey });
                var jsonParam = new SqlParameter("@JsonData", jsonString);

                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.Sp_DeleteOrder @JsonData", jsonParam);

                return new Response<OrderHeader>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, orderHeader);
            }
            catch (Exception ex)
            {
                return new Response<OrderHeader>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SelectAllAsync() -]
        public async Task<IResponse<IEnumerable<OrderHeader>>> SelectAllAsync()
        {
            try
            {
                var orders = await _context.Set<OrderHeader>()
                    .Where(oh => !oh.IsDeleted)
                    .AsNoTracking()
                    .OrderByDescending(oh => oh.OrderDate)
                    .ToListAsync();

                return new Response<IEnumerable<OrderHeader>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SelectAllAsync: {ex.Message}");
                return new Response<IEnumerable<OrderHeader>>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SelectByIdAsync() -]
        public async Task<IResponse<OrderHeader>> SelectByIdAsync(OrderHeader orderHeader)
        {
            try
            {
                var order = await _context.Set<OrderHeader>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.GuidKey == orderHeader.GuidKey && !x.IsDeleted);

                return new Response<OrderHeader>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, order);
            }
            catch (Exception ex)
            {
                return new Response<OrderHeader>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SelectDetailsByHeaderIdAsync() -]
        public async Task<IResponse<IEnumerable<OrderDetail>>> SelectDetailsByHeaderIdAsync(Guid headerId)
        {
            try
            {
                var details = await _context.Set<OrderDetail>()
                    .Where(d => d.OrderHeaderId == headerId && !d.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();

                return new Response<IEnumerable<OrderDetail>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, details);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SelectDetailsByHeaderIdAsync: {ex.Message}");
                return new Response<IEnumerable<OrderDetail>>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SerializeOrder() -]
        private string SerializeOrder(object entity) => JsonSerializer.Serialize(entity, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        });
        #endregion
    }
}
