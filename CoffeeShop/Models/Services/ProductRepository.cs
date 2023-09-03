using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;

namespace CoffeeShop.Models.Services
{
    public class ProductRepository : IProductRepository
    {
        private CoffeeShopDbContext dbcontext;
        public ProductRepository(CoffeeShopDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return dbcontext.Products;
        }

        public Product? GetProductDetail(int id)
        {
            return dbcontext.Products.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetTrendingProducts()
        {
            return dbcontext.Products.Where(p => p.IsTrendingProduct);
        }
    }
}
