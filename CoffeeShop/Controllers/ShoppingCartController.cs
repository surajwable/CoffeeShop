using CoffeeShop.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IShoppingCartRepository shoppingCartRepository { get; set; }
        private IProductRepository productRepository { get; set; }
        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var items = shoppingCartRepository.GetShoppingCartItems();
            shoppingCartRepository.ShoppingCartItems = items;
            ViewBag.cartTotal = shoppingCartRepository.GetShoppingCartTotal();     // used viewbag to send data from controller to view even if the return type of this method is different 
            return View(items);
        }

        public RedirectToActionResult AddToShoppingCart(int pid)
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.Id == pid);
            if(product != null)
            {
                shoppingCartRepository.AddToCart(product);
                int cartCount = shoppingCartRepository.GetShoppingCartItems().Count();
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromCart(int pid) 
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.Id == pid);
            if(product != null)
            {
                shoppingCartRepository.RemoveFromCart(product);
                int cartCount = shoppingCartRepository.GetShoppingCartItems().Count();
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }
            return RedirectToAction("Index");
        }   
    }
}
