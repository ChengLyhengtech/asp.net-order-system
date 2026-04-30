using System.Net.Http.Headers;
using System.Net.Http.Json; // Ensure this is included for ReadFromJsonAsync
using System.Text.Json;
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


    public PaymentResponseDto GenerateKhqr(CreatePaymentRequestDto request)
    {
        string invoice = request.OrderId ?? $"INV-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var individualInfo = new IndividualInfo
        {
            BakongAccountID = _config["Bakong:BakongId"],
            MerchantName = _config["Bakong:MerchantName"],
            MerchantCity = _config["Bakong:MerchantCity"],
            Currency = request.Currency.ToUpper() == "USD" ? KHQRCurrency.USD : KHQRCurrency.KHR,
            Amount = (double)request.Amount,
            BillNumber = invoice,
            ExpirationTimestamp = request.ExpiryDate == default
                ? DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeMilliseconds()
                : new DateTimeOffset(request.ExpiryDate).ToUnixTimeMilliseconds()
        };

        var result = BakongKHQR.GenerateIndividual(individualInfo);

        if (result.Status?.Code != 0)
            throw new Exception(result.Status?.Message ?? "Failed to generate KHQR");

        return new PaymentResponseDto
        {
            QrString = result.Data.QR,
            Md5 = result.Data.MD5,
            Invoice = invoice
        };
    }

    public async Task<CheckTransactionResponseDto> CheckPaymentStatusAsync(string md5)
    {
        var baseUrl = _config["Bakong:ApiBaseUrl"] ?? "https://api-bakong.nbc.gov.kh";
        var token = _config["Bakong:AuthToken"];

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v1/check_transaction_by_md5");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { md5 });

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new CheckTransactionResponseDto
                {
                    ResponseCode = 1,
                    ResponseMessage = $"API Error: {response.StatusCode}"
                };
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = await response.Content.ReadFromJsonAsync<CheckTransactionResponseDto>(options);

            return result ?? new CheckTransactionResponseDto
            {
                ResponseCode = 1,
                ResponseMessage = "Empty Response"
            };
        }
        catch (Exception ex)
        {
            return new CheckTransactionResponseDto
            {
                ResponseCode = 1,
                ResponseMessage = ex.Message
            };
        }
    }
}