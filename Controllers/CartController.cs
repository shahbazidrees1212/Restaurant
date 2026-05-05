using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Models;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantMvcUltimatePro.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "Cart";

        // JAZZCASH SANDBOX CONFIG
        private readonly string merchantId = "YOUR_JAZZCASH_MERCHANT_ID";
        private readonly string password = "YOUR_JAZZCASH_PASSWORD";
        private readonly string integritySalt = "YOUR_JAZZCASH_INTEGRITY_SALT";
        private readonly string jazzCashUrl = "https://sandbox.jazzcash.com.pk/CustomerPortal/transactionmanagement/merchantform/";

        // EASYPAISA SANDBOX CONFIG
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
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var existingItem = cart.FirstOrDefault(x => x.MenuItemId == id);

            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    MenuItemId = id,
                    Name = name,
                    ImageUrl = imageUrl,
                    Price = price,
                    Quantity = quantity
                });

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
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
        public IActionResult Checkout(string paymentMethod, decimal grandTotal)
        {
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
                TempData["Success"] = $"Order placed successfully with Cash On Delivery. Total: PKR {grandTotal}";
                HttpContext.Session.Remove(CartKey);
                return RedirectToAction("Index");
            }

            if (paymentMethod == "JazzCash")
                return RedirectToAction("JazzCashPayment", new { amount = grandTotal });

            if (paymentMethod == "EasyPaisa")
                return RedirectToAction("EasyPaisaPayment", new { amount = grandTotal });

            if (paymentMethod == "Bank Transfer")
                return RedirectToAction("Payment", new { method = paymentMethod, amount = grandTotal });

            TempData["Error"] = "Invalid payment method.";
            return RedirectToAction("Index");
        }

        public IActionResult Payment(string method, decimal amount)
        {
            ViewBag.PaymentMethod = method;
            ViewBag.Amount = amount;
            return View();
        }

        // =========================
        // JAZZCASH PAYMENT
        // =========================
        public IActionResult JazzCashPayment(decimal amount)
        {
            string txnRefNo = "JC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string amountInPaisa = ((int)(amount * 100)).ToString();

            string txnDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string expiry = DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss");
            string returnUrl = Url.Action("JazzCashCallback", "Cart", null, Request.Scheme);

            var data = new SortedDictionary<string, string>
            {
                { "pp_Version", "1.1" },
                { "pp_TxnType", "MWALLET" },
                { "pp_Language", "EN" },
                { "pp_MerchantID", merchantId },
                { "pp_Password", password },
                { "pp_TxnRefNo", txnRefNo },
                { "pp_Amount", amountInPaisa },
                { "pp_TxnCurrency", "PKR" },
                { "pp_TxnDateTime", txnDateTime },
                { "pp_TxnExpiryDateTime", expiry },
                { "pp_BillReference", txnRefNo },
                { "pp_Description", "Restaurant Order Payment" },
                { "pp_ReturnURL", returnUrl }
            };

            data.Add("pp_SecureHash", GenerateJazzCashHash(data));
            data.Add("gatewayUrl", jazzCashUrl);

            return View("JazzCashPayment", data);
        }

        private string GenerateJazzCashHash(SortedDictionary<string, string> data)
        {
            var filteredData = data
                .Where(x => x.Key != "pp_SecureHash" && x.Key != "gatewayUrl" && !string.IsNullOrEmpty(x.Value))
                .OrderBy(x => x.Key);

            string hashString = integritySalt + "&" + string.Join("&", filteredData.Select(x => x.Value));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(integritySalt));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashString));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
        }

        [HttpPost]
        public IActionResult JazzCashCallback(IFormCollection form)
        {
            string responseCode = form["pp_ResponseCode"];
            string responseMessage = form["pp_ResponseMessage"];
            string txnRefNo = form["pp_TxnRefNo"];

            if (responseCode == "000")
            {
                TempData["Success"] = $"JazzCash payment successful. Transaction: {txnRefNo}";
                HttpContext.Session.Remove(CartKey);
            }
            else
            {
                TempData["Error"] = $"JazzCash payment failed: {responseMessage}";
            }

            return RedirectToAction("Index");
        }

        // =========================
        // EASYPAISA PAYMENT
        // =========================
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

            return View("EasyPaisaPayment", data);
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
        public IActionResult EasyPaisaCallback(IFormCollection form)
        {
            string responseCode = form["responseCode"];
            string responseDesc = form["responseDesc"];
            string orderRefNum = form["orderRefNum"];

            if (responseCode == "000" || responseCode == "0000")
            {
                TempData["Success"] = $"EasyPaisa payment successful. Order Ref: {orderRefNum}";
                HttpContext.Session.Remove(CartKey);
            }
            else
            {
                TempData["Error"] = $"EasyPaisa payment failed: {responseDesc}";
            }

            return RedirectToAction("Index");
        }

        // =========================
        // BANK TRANSFER
        // NOTE: This is manual verification.
        // Real bank API needs bank/payment gateway merchant credentials.
        // =========================
        [HttpPost]
        public IActionResult ConfirmPayment(
            string paymentMethod,
            decimal amount,
            string accountNumber,
            string transactionId,
            string senderName)
        {
            if (string.IsNullOrWhiteSpace(senderName) ||
                string.IsNullOrWhiteSpace(accountNumber) ||
                string.IsNullOrWhiteSpace(transactionId))
            {
                TempData["Error"] = "Please fill all bank payment details.";
                return RedirectToAction("Payment", new
                {
                    method = paymentMethod,
                    amount = amount
                });
            }

            TempData["Success"] =
                $"Bank payment submitted successfully. Amount: PKR {amount}. Transaction ID: {transactionId}";

            HttpContext.Session.Remove(CartKey);
            return RedirectToAction("Index");
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
    }
}