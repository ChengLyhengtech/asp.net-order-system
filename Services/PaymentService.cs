using System.Net.Http.Headers;
using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using kh.gov.nbc.bakong_khqr;
using kh.gov.nbc.bakong_khqr.model;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public PaymentService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    // =========================
    // GENERATE KHQR
    // =========================
    public PaymentResponseDto GenerateKhqr(CreatePaymentRequestDto request)
    {
        string invoice = $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var merchantInfo = new MerchantInfo
        {
            BakongAccountID = _config["Bakong:BakongId"],
            MerchantName = _config["Bakong:MerchantName"],
            MerchantCity = _config["Bakong:MerchantCity"],

            MerchantID = _config["Bakong:AcquiringId"],

            AcquiringBank = "Bakong",
            Currency = KHQRCurrency.USD,
            Amount = (double)request.Amount,
            BillNumber = invoice,

            // ⚠️ MUST be MILLISECONDS (NOT seconds)
            ExpirationTimestamp = DateTimeOffset
                .UtcNow.AddMinutes(30)
                .ToUnixTimeMilliseconds()
        };

        var result = BakongKHQR.GenerateMerchant(merchantInfo);

        if (result.Status.Code != 0)
            throw new Exception(result.Status.Message);

        return new PaymentResponseDto
        {
            QrString = result.Data.QR,
            Md5 = result.Data.MD5,
            Invoice = invoice
        };
    }

    // =========================
    // CHECK PAYMENT
    // =========================
    public async Task<CheckTransactionResponseDto?> CheckPaymentStatusAsync(string md5)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config["Bakong:Token"]);

        var response = await _httpClient.PostAsJsonAsync(
            "https://api-bakong.nbc.gov.kh/v1/check_transaction_by_md5",
            new { md5 }
        );

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<CheckTransactionResponseDto>();
    }

    // =========================
    // DEEPLINK
    // =========================
    public async Task<object> GenerateDeeplinkAsync(string qrString)
    {
        var payload = new
        {
            qr = qrString,
            sourceInfo = new
            {
                appIconUrl = "https://bakong.nbc.gov.kh/images/logo.svg",
                appName = "Fantastic",
                appDeepLinkCallback = "https://bakong.nbc.gov.kh/"
            }
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config["Bakong:Token"]);

        var response = await _httpClient.PostAsJsonAsync(
            "https://api-bakong.nbc.gov.kh/v1/generate_deeplink_by_qr",
            payload
        );

        return await response.Content.ReadFromJsonAsync<object>();
    }
}