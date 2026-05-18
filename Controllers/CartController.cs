using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantMvcUltimatePro.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "Cart";

        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private readonly string merchantId = "MC750532";
        private readonly string password = "74h16z3495";
        private readonly string integritySalt = "sx354815a9";
        private readonly string jazzCashUrl = "https://sandbox.jazzcash.com.pk/CustomerPortal/transactionmanagement/merchantform/";

        private readonly string easypaisaStoreId = "YOUR_EASYPAISA_STORE_ID";
        private readonly string easypaisaHashKey = "YOUR_EASYPAISA_HASH_KEY";
        private readonly string easypaisaUrl = "https://easypaystg.easypaisa.com.pk/easypay/Index.jsf";

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, string name, string imageUrl, decimal price, int quantity = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Please login first to add items into cart.";
                return RedirectToAction("Login", "Account");
            }

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var existingItem = cart.FirstOrDefault(x => x.MenuItemId == id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MenuItemId = id,
                    Name = name,
                    ImageUrl = imageUrl,
                    Price = price,
                    Quantity = quantity
                });
            }

            HttpContext.Session.SetObject(CartKey, cart);

            TempData["Success"] = "Item added to cart successfully.";
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.MenuItemId == id);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout(
      string paymentMethod,
      decimal grandTotal,
      string customerName,
      string customerEmail,
      string phone,
      string address)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                TempData["Error"] = "Please select a payment method.";
                return RedirectToAction("Index");
            }

            if (grandTotal <= 0)
            {
                TempData["Error"] = "Invalid order amount.";
                return RedirectToAction("Index");
            }

            if (paymentMethod == "Cash On Delivery")
            {
                int orderId = SaveOrder(
                    grandTotal,
                    "Cash On Delivery",
                    customerName,
                    customerEmail,
                    phone,
                    address
                );

                _context.Notifications.Add(new NotificationModel
                {
                    Title = "New Order Placed",
                    Message = $"Your order #{orderId} has been placed successfully.",
                    CreatedAt = DateTime.Now,
                    
                });

                _context.SaveChanges();

                TempData["Success"] = "Order placed successfully.";
                return RedirectToAction("Success", "Order", new { id = orderId });
            }

            if (paymentMethod == "JazzCash" ||
                paymentMethod == "EasyPaisa" ||
                paymentMethod == "Card Payment")
            {
                TempData["CustomerName"] = customerName;
                TempData["CustomerEmail"] = customerEmail;
                TempData["Phone"] = phone;
                TempData["Address"] = address;

                return RedirectToAction("Payment", new
                {
                    method = paymentMethod,
                    amount = grandTotal
                });
            }

            TempData["Error"] = "Invalid payment method.";
            return RedirectToAction("Index");
        }

        public IActionResult Payment(string method, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(method) || amount <= 0)
            {
                TempData["Error"] = "Invalid payment request.";
                return RedirectToAction("Index");
            }

            ViewBag.PaymentMethod = method;
            ViewBag.Amount = amount;

            TempData.Keep("CustomerName");
            TempData.Keep("CustomerEmail");
            TempData.Keep("Phone");
            TempData.Keep("Address");

            return View();
        }

        public IActionResult CardPayment(decimal amount)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Invalid payment amount.";
                return RedirectToAction("Index");
            }

            ViewBag.Amount = amount;

            TempData.Keep("CustomerName");
            TempData.Keep("CustomerEmail");
            TempData.Keep("Phone");
            TempData.Keep("Address");

            return View();
        }

        [HttpPost]
        public IActionResult ProcessCardPayment(
            decimal amount,
            string cardName,
            string cardNumber,
            string expiry,
            string cvv)
        {
            if (amount <= 0 ||
                string.IsNullOrWhiteSpace(cardName) ||
                string.IsNullOrWhiteSpace(cardNumber) ||
                string.IsNullOrWhiteSpace(expiry) ||
                string.IsNullOrWhiteSpace(cvv))
            {
                TempData["Error"] = "Please fill all card details.";
                return RedirectToAction("CardPayment", new { amount });
            }

            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (!IsValidCardHolderName(cardName))
            {
                TempData["Error"] = "Invalid card holder name.";
                return RedirectToAction("CardPayment", new { amount });
            }

            if (!IsValidCardNumber(cardNumber))
            {
                TempData["Error"] = "Invalid card number.";
                return RedirectToAction("CardPayment", new { amount });
            }

            if (!IsValidExpiry(expiry))
            {
                TempData["Error"] = "Invalid or expired card expiry date.";
                return RedirectToAction("CardPayment", new { amount });
            }

            if (!IsValidCvv(cvv))
            {
                TempData["Error"] = "Invalid CVV.";
                return RedirectToAction("CardPayment", new { amount });
            }

            string customerName = TempData["CustomerName"]?.ToString() ?? "Guest";
            string customerEmail = TempData["CustomerEmail"]?.ToString() ?? "guest@gmail.com";
            string phone = TempData["Phone"]?.ToString() ?? "N/A";
            string address = TempData["Address"]?.ToString() ?? "N/A";

            int orderId = SaveOrder(amount, "Paid", customerName, customerEmail, phone, address);

            TempData["Success"] = "Card payment successful.";
            return RedirectToAction("Success", "Order", new { id = orderId });
        }

        private bool IsValidCardHolderName(string name)
        {
            return name.Length >= 3 && name.All(c => char.IsLetter(c) || c == ' ');
        }

        private bool IsValidCardNumber(string cardNumber)
        {
            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            if (!cardNumber.All(char.IsDigit))
                return false;

            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = cardNumber[i] - '0';

                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }

                sum += n;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }

        private bool IsValidExpiry(string expiry)
        {
            if (string.IsNullOrWhiteSpace(expiry))
                return false;

            expiry = expiry.Trim();

            var parts = expiry.Split('/');

            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out int month))
                return false;

            if (!int.TryParse(parts[1], out int year))
                return false;

            if (month < 1 || month > 12)
                return false;

            year += 2000;

            var expiryDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return expiryDate >= DateTime.Today;
        }

        private bool IsValidCvv(string cvv)
        {
            return cvv.All(char.IsDigit) && (cvv.Length == 3 || cvv.Length == 4);
        }
        public IActionResult JazzCashPayment(decimal amount)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Invalid payment amount.";
                return RedirectToAction("Index");
            }

            string ngrokBaseUrl = "https://crudest-chet-noncarbolic.ngrok-free.dev";

            string txnRefNo = "JC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string amountInPaisa = ((int)(amount * 100)).ToString();

            string txnDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string expiry = DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss");

            string returnUrl = $"{ngrokBaseUrl}/Cart/JazzCashCallback";

            var data = new SortedDictionary<string, string>
    {
        { "pp_Version", "1.1" },
        { "pp_TxnType", "MWALLET" },
        { "pp_Language", "EN" },
        { "pp_MerchantID", merchantId },
        { "pp_SubMerchantID", "" },
        { "pp_Password", password },
        { "pp_BankID", "" },
        { "pp_ProductID", "" },
        { "pp_TxnRefNo", txnRefNo },
        { "pp_Amount", amountInPaisa },
        { "pp_TxnCurrency", "PKR" },
        { "pp_TxnDateTime", txnDateTime },
        { "pp_BillReference", txnRefNo },
        { "pp_Description", "Restaurant Order Payment" },
        { "pp_TxnExpiryDateTime", expiry },
        { "pp_ReturnURL", returnUrl },
        { "ppmpf_1", "" },
        { "ppmpf_2", "" },
        { "ppmpf_3", "" },
        { "ppmpf_4", "" },
        { "ppmpf_5", "" }
    };

            data.Add("pp_SecureHash", GenerateJazzCashHash(data));
            data.Add("gatewayUrl", jazzCashUrl);

            TempData.Keep("CustomerName");
            TempData.Keep("CustomerEmail");
            TempData.Keep("Phone");
            TempData.Keep("Address");

            return View("JazzCashPayment", data);
        }
        [HttpPost]
        [HttpGet]
        public IActionResult JazzCashCallback()
        {
            string responseCode = Request.Form["pp_ResponseCode"];
            string responseMessage = Request.Form["pp_ResponseMessage"];
            string txnRefNo = Request.Form["pp_TxnRefNo"];

            if (responseCode == "000")
            {
                var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
                decimal amount = cart.Sum(x => x.Price * x.Quantity);

                string customerName = TempData["CustomerName"]?.ToString() ?? "Guest";
                string customerEmail = TempData["CustomerEmail"]?.ToString() ?? "guest@gmail.com";
                string phone = TempData["Phone"]?.ToString() ?? "N/A";
                string address = TempData["Address"]?.ToString() ?? "N/A";

                int orderId = SaveOrder(amount, "Paid", customerName, customerEmail, phone, address);

                TempData["Success"] = $"JazzCash payment successful. Transaction: {txnRefNo}";
                return RedirectToAction("Success", "Order", new { id = orderId });
            }

            TempData["Error"] = $"JazzCash payment failed: {responseMessage}";
            return RedirectToAction("Index");
        }
        private string GenerateJazzCashHash(SortedDictionary<string, string> data)
        {
            var filteredData = data
                .Where(x =>
                    x.Key != "pp_SecureHash" &&
                    x.Key != "gatewayUrl" &&
                    !string.IsNullOrWhiteSpace(x.Value))
                .OrderBy(x => x.Key);

            string hashString = integritySalt + "&" + string.Join("&", filteredData.Select(x => x.Value));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(integritySalt));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashString));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
        }
        public IActionResult EasyPaisaPayment(decimal amount)
        {
            string orderRefNum = "EP" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string postBackUrl = Url.Action("EasyPaisaCallback", "Cart", null, Request.Scheme);

            var data = new SortedDictionary<string, string>
            {
                { "storeId", easypaisaStoreId },
                { "amount", amount.ToString("0.00") },
                { "orderRefNum", orderRefNum },
                { "postBackURL", postBackUrl },
                { "timeStamp", DateTime.Now.ToString("yyyyMMdd HH:mm:ss") },
                { "paymentMethod", "InitialRequest" }
            };

            data.Add("encryptedHashRequest", GenerateEasyPaisaHash(data));
            data.Add("checkoutUrl", easypaisaUrl);

            TempData.Keep("CustomerName");
            TempData.Keep("CustomerEmail");
            TempData.Keep("Phone");
            TempData.Keep("Address");

            return View("EasyPaisaPayment", data);
        }

        [HttpPost]
        public IActionResult EasyPaisaCallback(IFormCollection form)
        {
            string responseCode = form["responseCode"];
            string responseDesc = form["responseDesc"];
            string orderRefNum = form["orderRefNum"];

            if (responseCode == "000" || responseCode == "0000")
            {
                var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
                decimal amount = cart.Sum(x => x.Price * x.Quantity);

                string customerName = TempData["CustomerName"]?.ToString() ?? "Guest";
                string customerEmail = TempData["CustomerEmail"]?.ToString() ?? "guest@gmail.com";
                string phone = TempData["Phone"]?.ToString() ?? "N/A";
                string address = TempData["Address"]?.ToString() ?? "N/A";

                int orderId = SaveOrder(amount, "Paid", customerName, customerEmail, phone, address);

                TempData["Success"] = $"EasyPaisa payment successful. Order Ref: {orderRefNum}";
                return RedirectToAction("Success", "Order", new { id = orderId });
            }

            TempData["Error"] = $"EasyPaisa payment failed: {responseDesc}";
            return RedirectToAction("Index");
        }

        private string GenerateEasyPaisaHash(SortedDictionary<string, string> data)
        {
            var filteredData = data
                .Where(x => x.Key != "encryptedHashRequest" && x.Key != "checkoutUrl" && !string.IsNullOrEmpty(x.Value))
                .OrderBy(x => x.Key);

            string hashString = string.Join("&", filteredData.Select(x => $"{x.Key}={x.Value}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(easypaisaHashKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashString));

            return Convert.ToBase64String(hashBytes);
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.MenuItemId == id);

            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartKey);
            return RedirectToAction("Index");
        }

        private int SaveOrder(
            decimal amount,
            string status,
            string customerName,
            string customerEmail,
            string phone,
            string address)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();

            if (!cart.Any())
                return 0;

            var order = new Order
            {
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                Phone = phone,
                Address = address,
                TotalAmount = amount,
                Status = status,
                OrderDate = DateTime.Now,

                OrderItems = cart.Select(x => new OrderItem
                {
                    Name = x.Name,
                    ImageUrl = x.ImageUrl,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            HttpContext.Session.Remove(CartKey);

            return order.Id;
        }
    }
}