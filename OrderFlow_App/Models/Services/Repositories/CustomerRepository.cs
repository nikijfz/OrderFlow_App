using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.Models.DomainModels.CustomerAggregates;
using OrderFlow_App.Frameworks.ResponseFrameworks;
using OrderFlow_App.Models.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using OrderFlow_App.Frameworks;
using System.Net;

namespace OrderFlow_App.Models.Services.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        #region [- Private Fields -]
        private readonly ProjectDbContext _context;
        #endregion

        #region [- Ctor() -]
        public CustomerRepository(ProjectDbContext context)
        {
            _context = context;
        }
        #endregion

        #region [- InsertAsync() -]
        public async Task<IResponse<Customer>> InsertAsync(Customer customer)
        {
            try
            {
                await _context.AddAsync(customer);
                await _context.SaveChangesAsync();

                return new Response<Customer>(true, HttpStatusCode.Created, ResponseMessages.SuccessfullOperation, customer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Customer>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- UpdateAsync() -]
        public async Task<IResponse<Customer>> UpdateAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Set<Customer>().SingleOrDefaultAsync(c => c.Id == customer.Id);
                _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
                await _context.SaveChangesAsync();

                return new Response<Customer>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingCustomer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Customer>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- DeleteAsync() -]
        public async Task<IResponse<Customer>> DeleteAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Set<Customer>().SingleOrDefaultAsync(c => c.Id == customer.Id);
                existingCustomer.IsDeleted = true;
                _context.Update(existingCustomer);
                await _context.SaveChangesAsync();

                return new Response<Customer>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingCustomer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Customer>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SelectAll() -]
        public async Task<IResponse<IEnumerable<Customer>>> SelectAllAsync()
        {
            try
            {
                var customers = await _context.Set<Customer>().AsNoTracking().ToListAsync();

                return new Response<IEnumerable<Customer>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<IEnumerable<Customer>>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SelectByIdAsync() -]
        public async Task<IResponse<Customer>> SelectByIdAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Set<Customer>().SingleOrDefaultAsync(c => c.Id == customer.Id);

                return new Response<Customer>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingCustomer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Customer>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion
    }
}
