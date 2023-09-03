using CoffeeShop.Models;
using CoffeeShop.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private IOrderRepository orderRepository;
        private IShoppingCartRepository shopCartRepository;

        public OrderController(IOrderRepository orderRepository,IShoppingCartRepository shoppingCartRepository)
        {
            this.orderRepository = orderRepository;
            this.shopCartRepository = shoppingCartRepository;
        }
        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckOut(Order user)
        {
            orderRepository.PlaceOrder(user);
            shopCartRepository.ClearCart();
            HttpContext.Session.SetInt32("CartCount", 0);
            return RedirectToAction("CheckOutComplete");
        }

        public IActionResult CheckOutComplete()
        {
            return View();
        }
    }
}
