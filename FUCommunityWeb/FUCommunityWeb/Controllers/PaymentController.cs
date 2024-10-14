using Azure.Core;
using FuCommunityWebModels.Models;
using Microsoft.AspNetCore.Mvc;
using FuCommunityWebServices.Services;
using FuCommunityWebDataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FuCommunityWebDataAccess.Data;

namespace FUCommunityWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;
        private readonly VnPayService _vnPayService;
        private readonly OrderRepo _orderRepository;
        private readonly ApplicationDbContext _context;


        public PaymentController(IConfiguration configuration, ILogger<PaymentController> logger, VnPayService vnPayService, OrderRepo orderRepository, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _vnPayService = vnPayService;
            _orderRepository = orderRepository;
            _context = context;
        }

        // Index Action (formerly in HomeController)
        public IActionResult Index()
        {
            return View();
        }

        // Pay Actions
        [HttpGet]
        public IActionResult Pay()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pay(OrderInfo order, string paymentMethod, string locale, int Amount)
        {
            // Lấy thông tin người dùng hiện tại từ Claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Ghi log để kiểm tra paymentMethod
            _logger.LogInformation($"Received payment method: {paymentMethod}");

            if (string.IsNullOrEmpty(paymentMethod))
            {
                _logger.LogWarning("Payment method is null or empty.");
                ModelState.AddModelError("paymentMethod", "Vui lòng chọn phương thức thanh toán.");
                return View(order); // Trả lại form nếu không có phương thức thanh toán nào được chọn
            }

            if (Amount <= 0)
            {
                _logger.LogWarning("Invalid amount entered.");
                ModelState.AddModelError("Amount", "Vui lòng nhập số tiền hợp lệ.");
                return View(order);
            }

            // Thiết lập chi tiết đơn hàng
            order.Amount = Amount; // Sử dụng số tiền người dùng nhập
            order.Status = "0";
            order.CreatedDate = DateTime.Now;
            order.UserID = userId; // Gắn UserId cho đơn hàng

            // Xác định BankCode dựa trên phương thức thanh toán được chọn
            string bankCode = paymentMethod switch
            {
                "VNPAYQR" => "VNPAYQR",
                "VNBANK" => "VNBANK",
                "INTCARD" => "INTCARD",
                _ => throw new ArgumentException("Phương thức thanh toán không hợp lệ."),
            };

            order.BankCode = bankCode;

            // Lưu đơn hàng vào cơ sở dữ liệu
            await _orderRepository.AddOrderAsync(order);

            // Ghi log để kiểm tra bankCode
            _logger.LogInformation($"Determined BankCode: {bankCode}");

            // Xác định ngôn ngữ (locale)
            string selectedLocale = locale switch
            {
                "en" => "en",
                _ => "vn",
            };

            // Tạo URL thanh toán
            string paymentUrl = _vnPayService.CreateRequestUrl(order, bankCode, selectedLocale);
            _logger.LogInformation($"Generated VNPAY URL: {paymentUrl}");

            // Điều hướng tới URL thanh toán
            return Redirect(paymentUrl);
        }







        // QueryDR Actions
        [HttpGet]
        public IActionResult QueryDR()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QueryDR(string orderId, string payDate)
        {
            string response = await _vnPayService.QueryDR(orderId, payDate);
            ViewBag.Response = response;
            return View();
        }

        // Refund Actions
        [HttpGet]
        public IActionResult Refund()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Refund(string orderId, long amount, string refundCategory, string payDate, string user)
        {
            string response = await _vnPayService.Refund(orderId, amount, refundCategory, payDate, user);
            ViewBag.Response = response;
            return View();
        }

        // IPN Action
        [HttpPost]
        public async Task<IActionResult> IPN()
        {
            var vnpayData = Request.Form;
            SortedList<string, string> sortedVnpayData = new SortedList<string, string>();
            foreach (var key in vnpayData.Keys)
            {
                if (key.StartsWith("vnp_"))
                {
                    sortedVnpayData.Add(key, vnpayData[key]);
                }
            }

            string vnp_HashSecret = _configuration["VnPay:HashSecret"];
            string inputHash = vnpayData["vnp_SecureHash"];
            bool isValid = _vnPayService.ValidateSignature(sortedVnpayData, inputHash, vnp_HashSecret);

            string returnContent = string.Empty;

            if (isValid)
            {
                long orderId = Convert.ToInt64(sortedVnpayData["vnp_TxnRef"]);
                long amount = Convert.ToInt64(sortedVnpayData["vnp_Amount"]) / 100;
                long vnpayTranId = Convert.ToInt64(sortedVnpayData["vnp_TransactionNo"]);
                string responseCode = sortedVnpayData["vnp_ResponseCode"];
                string transactionStatus = sortedVnpayData["vnp_TransactionStatus"];

                // Retrieve order from database using orderId
                OrderInfo order = await _orderRepository.GetOrderByIdAsync(orderId);

                if (order != null)
                {
                    if (order.Amount == amount)
                    {
                        if (order.Status == "0")
                        {
                            if (responseCode == "00" && transactionStatus == "00")
                            {
                                // Payment successful
                                _logger.LogInformation($"Payment successful, OrderId={orderId}, VNPAY TranId={vnpayTranId}");
                                order.Status = "1";
                                ViewBag.Message = "Giao dịch thành công";
                            }
                            else
                            {
                                // Payment failed
                                _logger.LogInformation($"Payment failed, OrderId={orderId}, VNPAY TranId={vnpayTranId}, ResponseCode={responseCode}");
                                order.Status = "2";
                                ViewBag.Message = $"Có lỗi xảy ra. Mã lỗi: {responseCode}";
                            }

                            // Update order status in database
                            await _orderRepository.UpdateOrderAsync(order);

                            returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                        }
                    }
                    else
                    {
                        returnContent = "{\"RspCode\":\"04\",\"Message\":\"Invalid amount\"}";
                    }
                }
                else
                {
                    returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                }
            }
            else
            {
                _logger.LogInformation($"Invalid signature, InputData={Request.Form}");
                returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
            }

            return Content(returnContent, "application/json");
        }


        // Return Action
        [HttpGet]
        public async Task<IActionResult> Return()
        {
            var vnpayData = Request.Query;
            SortedList<string, string> sortedVnpayData = new SortedList<string, string>();
            foreach (var key in vnpayData.Keys)
            {
                if (key.StartsWith("vnp_"))
                {
                    sortedVnpayData.Add(key, vnpayData[key]);
                }
            }

            string vnp_HashSecret = _configuration["VnPay:HashSecret"];
            string inputHash = vnpayData["vnp_SecureHash"];
            bool isValid = _vnPayService.ValidateSignature(sortedVnpayData, inputHash, vnp_HashSecret);

            if (isValid)
            {
                string responseCode = sortedVnpayData["vnp_ResponseCode"];
                if (responseCode == "00")
                {
                    ViewBag.Message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";

                    // Lấy OrderId từ VNPAY
                    var orderId = Convert.ToInt64(sortedVnpayData["vnp_TxnRef"]);
                    var amount = Convert.ToInt64(sortedVnpayData["vnp_Amount"]) / 100;

                    // Lấy thông tin đơn hàng từ cơ sở dữ liệu
                    var order = await _orderRepository.GetOrderByIdAsync(orderId);

                    if (order != null && order.Status == "0") // Trạng thái "0" là chưa xử lý
                    {
                        // Đánh dấu đơn hàng là đã hoàn thành
                        order.Status = "1";
                        await _orderRepository.UpdateOrderAsync(order);

                        // Lấy thông tin người dùng từ Claim
                        var userId = order.UserID;
                        var user = await _context.Users.FindAsync(userId);

                        if (user != null)
                        {
                            // Cộng số điểm tương ứng với số tiền đã nạp
                            user.Point += amount / 1000;

                            // Cập nhật thông tin người dùng
                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();

                            _logger.LogInformation($"User {user.UserName} đã được cộng {amount} điểm.");
                        }
                    }
                }
                else if (responseCode == "24")
                {
                    // Điều hướng người dùng trở lại trang thanh toán
                    return RedirectToAction("Pay", "VnPay");
                }
                else
                {
                    ViewBag.Message = $"Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: {responseCode}";
                }
            }
            else
            {
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
            }

            return View();
        }



    }
}
