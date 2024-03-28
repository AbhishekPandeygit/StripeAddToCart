using AddToCart.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe.BillingPortal;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using Session = Stripe.Checkout.Session;
using Microsoft.Extensions.Options;

namespace AddToCart.Controllers
{
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            List<ProductEntity> productList = new List<ProductEntity>();

            productList = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Product = "Tommy Hilfiger",
                    Rate = 1500,
                    Quantity = 1,
                    ImagePath = "image/1.png"
                },
                  new ProductEntity
                {
                    Product = "Timewear",
                    Rate = 1000,
                    Quantity = 1,
                    ImagePath = "image/2.png"
                },
            };

            return View(productList);
        }

        public IActionResult Success()
        {

            return View();
        }

        public IActionResult Login()
        {

            return View();
        }

        public IActionResult OrderConfirmation()
        {
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(TempData["Session"].ToString());
            if(session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId.ToString();
                return View("Success");
            }

            return View("Login");
        }

        public IActionResult CheckOut()
        {
            List<ProductEntity> productList = new List<ProductEntity>();

            productList = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Product = "Tommy Hilfiger",
                    Rate = 1500,
                    Quantity = 1,
                    ImagePath = "image/1.png"
                },
                  new ProductEntity
                {
                    Product = "Timewear",
                    Rate = 1000,
                    Quantity = 1,
                    ImagePath = "image/2.png"
                },
            };

            var domain = "http://localhost:44386/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"CheckOut/OrderConfirmation",
                CancelUrl = domain + "CheckOut/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment" 
            };

            foreach(var item in productList)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Rate * item.Quantity),
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ToString(),
                        }

                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionListItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location",session.Url);

            return new StatusCodeResult(303);
        }
    }
}
