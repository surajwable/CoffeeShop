using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private CoffeeShopDbContext dbContext;

        public ShoppingCartRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<ShoppingCartItem>? ShoppingCartItems { get ; set ; }
        public string? ShoppingCartId { get; set ; }

        //It will be executed automatically when an instance of productRepository class is created.
        //This method takes service provider which we will pass through service registration in program.cs.
        public static ShoppingCartRepository GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;

            CoffeeShopDbContext context = services.GetService<CoffeeShopDbContext>() ?? throw new Exception("Error initializing coffeeshopdbcontext");

            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();

            session?.SetString("CartId", cartId);

            return new ShoppingCartRepository(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Product product)
        {
           var shoppingCartItem = dbContext.ShoppingCartItems.FirstOrDefault(p => p.Product.Id ==  product.Id && p.ShoppingCartId == ShoppingCartId);
           if(shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Qty = 1
                };

                dbContext.ShoppingCartItems.Add(shoppingCartItem);                                 
            }
            else
            {
                shoppingCartItem.Qty++;
            }
            dbContext.SaveChanges();
        }

        public void ClearCart()
        {
            var cartItems = dbContext.ShoppingCartItems.Where(p => p.ShoppingCartId == ShoppingCartId);
            dbContext.ShoppingCartItems.RemoveRange(cartItems);
            dbContext.SaveChanges();    
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??= dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId).Include(p=> p.Product).ToList();      
        }

        public decimal GetShoppingCartTotal()
        {
           var total = dbContext.ShoppingCartItems.Where(s=> s.ShoppingCartId == ShoppingCartId).Select(p => p.Product.Price * p.Qty).Sum();
            return total;
        }

        public int RemoveFromCart(Product product)
        {
            var shoppingCartItem = dbContext.ShoppingCartItems.FirstOrDefault(p => p.Product.Id == product.Id && p.ShoppingCartId == ShoppingCartId);
            var quantity = 0;
            
            if(shoppingCartItem != null)
            {
                if(shoppingCartItem.Qty > 1)
                {
                    shoppingCartItem.Qty--;
                    quantity = shoppingCartItem.Qty;
                }
                else
                {
                    dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }

            }
            dbContext.SaveChanges();
            return quantity;
        }

    }
}
