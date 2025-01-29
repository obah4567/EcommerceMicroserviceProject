using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProduitApi.Application.Interfaces;
using ProduitApi.Domain.Entities;
using ProduitApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProduitApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // check if product already exist
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{entity.Name} already added");
                }

                // Add product
                var currencyEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currencyEntity is not null && currencyEntity.Id > 0)
                {
                    return new Response(true, $"{entity.Name} added to database successfully");
                }
                else
                {
                    return new Response(false, $"Error occured while adding {entity.Name}");
                }
            }
            catch (Exception ex)
            {
                // Log the original exception (Appel des méthodes definis dans Log) 
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                return new Response(false, "Error occured adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                context.Entry(product).State = EntityState.Detached;
                context.Products.Remove(entity);
                await context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} was deleting successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception (Appel des méthodes definis dans Log) 
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                return new Response(false, "Error occured deleting product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception (Appel des méthodes definis dans Log) 
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                throw new Exception("Error occured retrieving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var product = await context.Products.AsNoTracking().ToListAsync();
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception (Appel des méthodes definis dans Log) 
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                throw new Exception("Error occured retrieving product");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception (Appel des méthodes definis dans Log) 
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                throw new Exception("Error occured retrieving product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                // check if product already exist
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                // Add product
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception (Appel des méthodes definis dans Log) 
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                return new Response(false, "Error occured updating existing product");
            }
        }
    }
}
