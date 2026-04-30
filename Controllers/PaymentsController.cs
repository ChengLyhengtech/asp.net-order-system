using System.Drawing;
using System.Drawing.Imaging;
using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _db;

        public PaymentsController(IPaymentService paymentService, AppDbContext db)
        {
            _paymentService = paymentService;
            _db = db;
        }
        [HttpGet("qr-image/{invoice}")]
        public IActionResult GetQrImage(string invoice)
        {
            var payment = _db.Payments.FirstOrDefault(x => x.Invoice == invoice);

            if (payment == null)
                return NotFound(new { message = "Invoice not found" });

            // IMPORTANT: use KHQR string, NOT MD5
            var khqrResult = _paymentService.GenerateKhqr(new CreatePaymentRequestDto
            {
                OrderId = payment.Invoice,
                Amount = payment.Amount,
                Currency = "USD"
            });

            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(khqrResult.QrString, QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode qrCode = new PngByteQRCode(data);
            byte[] qrBytes = qrCode.GetGraphic(20);

            return File(qrBytes, "image/png");
        }

        // 1. Generate the KHQR and save the initial record to DB
        [HttpPost("generate-khqr")]
        public async Task<IActionResult> Generate([FromBody] CreatePaymentRequestDto request)
        {
            try
            {
                // Logic within Service handles KHQR generation and MD5 hashing
                var khqr = _paymentService.GenerateKhqr(request);

                var payment = new PaymentModel
                {
                    Invoice = khqr.Invoice,
                    Amount = request.Amount,
                    Md5 = khqr.Md5,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };

                _db.Payments.Add(payment);
                await _db.SaveChangesAsync();

                return Ok(khqr);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // 2. Poll this endpoint from the Frontend to see if the user has paid
        [HttpGet("check-status/{invoice}")]
        public async Task<IActionResult> CheckStatus(string invoice)
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Invoice == invoice);

            if (payment == null)
                return NotFound(new { message = "Invoice not found" });

            // If we already know they paid, don't waste an API call to Bakong
            if (payment.Status == "PAID")
                return Ok(new { status = "PAID", message = "Transaction already completed" });

            // Call the Bakong external API via our Service
            var result = await _paymentService.CheckPaymentStatusAsync(payment.Md5);

            // Logic: ResponseCode 0 means the API responded, and result.Data ensures a transaction was found
            if (result.ResponseCode == 0 && result.Data != null)
            {
                payment.Status = "PAID";
                await _db.SaveChangesAsync();

                return Ok(new { status = "PAID", data = result.Data });
            }

            return Ok(new { status = "PENDING", message = "Awaiting payment..." });
        }
    }
}