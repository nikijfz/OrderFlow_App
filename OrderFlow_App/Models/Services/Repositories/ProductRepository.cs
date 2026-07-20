using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.Models.DomainModels.ProductAggregates;
using OrderFlow_App.Frameworks.ResponseFrameworks;
using OrderFlow_App.Models.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using OrderFlow_App.Frameworks;
using System.Net;

namespace OrderFlow_App.Models.Services.Repositories
{
    public class ProductRepository : IProductRepository
    {
        #region [- Private Fields -]
        private readonly ProjectDbContext _context;
        #endregion

        #region [- Ctor() -]
        public ProductRepository(ProjectDbContext context)
        {
            _context = context;
        }
        #endregion

        #region [- InsertAsync() -]
        public async Task<IResponse<Product>> InsertAsync(Product product)
        {
            try
            {
                await _context.AddAsync(product);
                await _context.SaveChangesAsync();

                return new Response<Product>(true, HttpStatusCode.Created, ResponseMessages.SuccessfullOperation, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Product>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- UpdateAsync() -]
        public async Task<IResponse<Product>> UpdateAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Set<Product>().SingleOrDefaultAsync(p => p.Id == product.Id);
                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                await _context.SaveChangesAsync();

                return new Response<Product>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Product>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- DeleteAsync() -]
        public async Task<IResponse<Product>> DeleteAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Set<Product>().SingleOrDefaultAsync(p => p.Id == product.Id);
                existingProduct.IsDeleted = true;
                _context.Update(existingProduct);
                await _context.SaveChangesAsync();

                return new Response<Product>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Product>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SelectAllAsync() -]
        public async Task<IResponse<IEnumerable<Product>>> SelectAllAsync()
        {
            try
            {
                var products = await _context.Set<Product>().AsNoTracking().ToListAsync();

                return new Response<IEnumerable<Product>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, products);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<IEnumerable<Product>>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion

        #region [- SelectByIdAsync() -]
        public async Task<IResponse<Product>> SelectByIdAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Set<Product>().SingleOrDefaultAsync(p => p.Id == product.Id);
               
                return new Response<Product>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, existingProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<Product>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
            }
        }
        #endregion
    }
}
