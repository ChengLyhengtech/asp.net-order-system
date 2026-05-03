using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _db;

        public PaymentsController(IPaymentService paymentService, AppDbContext db)
        {
            _paymentService = paymentService;
            _db = db;
        }

        // =========================
        // QR IMAGE
        // =========================
        [HttpGet("qr-image/{invoice}")]
        public async Task<IActionResult> GetQrImage(string invoice)
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(x => x.Invoice == invoice);
            if (payment == null) return NotFound();

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payment.QrString, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            var qrCodeImage = qrCode.GetGraphic(20);

            return File(qrCodeImage, "image/png");
        }

        // =========================
        // GENERATE KHQR
        // =========================
        [HttpPost("generate-khqr")]
        public async Task<IActionResult> Generate([FromBody] CreatePaymentRequestDto request)
        {
            if (string.IsNullOrEmpty(request.OrderId))
                request.OrderId = $"SET-{DateTime.UtcNow.Ticks}";

            try
            {
                var khqr = _paymentService.GenerateKhqr(request);

                var payment = new PaymentModel
                {
                    Invoice = khqr.Invoice,
                    Amount = request.Amount,
                    Md5 = khqr.Md5,
                    QrString = khqr.QrString,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };

                _db.Payments.Add(payment);
                await _db.SaveChangesAsync();

                return Ok(khqr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =========================
        // DEEPLINK
        // =========================
        [HttpPost("v1/generate_deeplink_by_qr")]
        public async Task<IActionResult> GenerateDeeplink([FromBody] DeeplinkRequest dto)
        {
            var result = await _paymentService.GenerateDeeplinkAsync(dto.QrString);
            return Ok(result);
        }

        // =========================
        // CHECK PAYMENT
        // =========================
        [HttpGet("check-payment/{invoice}")]
        public async Task<IActionResult> CheckStatus(string invoice)
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Invoice == invoice);

            if (payment == null)
                return Ok(new { status = "NOT_FOUND" });

            if (payment.Status == "PAID")
                return Ok(new { status = "PAID" });

            var bakongResponse = await _paymentService.CheckPaymentStatusAsync(payment.Md5);

            // 🔥 DEBUG (VERY IMPORTANT)
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(bakongResponse));

            // =========================
            // REAL CHECK (FIXED LOGIC)
            // =========================
            var isPaid =
                bakongResponse != null &&
                bakongResponse.Data != null &&
                (
                    bakongResponse.Data.Status?.ToUpper() == "SUCCESS" ||
                    bakongResponse.Data.Status?.ToUpper() == "COMPLETED"
                );

            if (isPaid)
            {
                payment.Status = "PAID";
                await _db.SaveChangesAsync();

                return Ok(new { status = "PAID" });
            }

            return Ok(new { status = "PENDING" });
        }
    }
}