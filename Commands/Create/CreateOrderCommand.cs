using aps.net_order_system.Data;
using aps.net_order_system.Hubs;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace aps.net_order_system.Commands
{
    public class CreateOrderItemCommand
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; } = string.Empty;
    }

    public class CreateOrderCommand
    {
        public string? TableId { get; set; }
        public List<CreateOrderItemCommand> Items { get; set; } = new();
    }

    public class CreateOrderCommandHandler
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        // Injected all 4 mandatory components cleanly
        public CreateOrderCommandHandler(
            AppDbContext context,
            IHubContext<OrderHub> hubContext,
            HttpClient httpClient,
            IConfiguration config)
        {
            _context = context;
            _hubContext = hubContext;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<int> Handle(CreateOrderCommand command)
        {
            if (command.Items == null || !command.Items.Any())
                throw new Exception("Order must have at least one item");

            var order = new OrderModel
            {
                OrderId = $"ORD-{Guid.NewGuid().ToString()[..5].ToUpper()}",
                TableId = command.TableId,
                Status = "Pending",
                PaymentStatus = "Paid",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItemModel>()
            };

            decimal totalAmount = 0;

            foreach (var item in command.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Quantity must be greater than 0");

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found");

                var subtotal = (decimal)product.Price * item.Quantity;
                totalAmount += subtotal;

                order.OrderItems.Add(new OrderItemModel
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name, // <-- ADD THIS LINE HERE TO FIX THE NULL ERROR
                    Quantity = item.Quantity,
                    SpecialInstructions = item.SpecialInstructions,
                    Subtotal = subtotal
                });
            }

            order.TotalAmount = totalAmount;
            _context.Orders.Add(order);

            // Global counter updates
            var globalCounter = await _context.TotalCountOrders.FirstOrDefaultAsync();
            if (globalCounter == null)
            {
                _context.TotalCountOrders.Add(new TotalCountOderModel { TotalCount = 1 });
            }
            else
            {
                globalCounter.TotalCount += 1;
            }

            // 1. Commit transaction safely to SQL Server database context
            await _context.SaveChangesAsync();

            // 2. Map data for execution signals using in-memory local join safely
            var broadcastItems = (from oi in order.OrderItems
                                  join p in _context.Products on oi.ProductId equals p.Id
                                  select new
                                  {
                                      oi.ProductId,
                                      ProductName = p.Name,
                                      oi.Quantity,
                                      oi.Subtotal,
                                      oi.SpecialInstructions
                                  }).ToList();

            // 3. Broadcast to React Staff Screen Monitors via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", new
            {
                id = order.Id,
                orderId = order.OrderId,
                tableId = order.TableId,
                totalPrice = order.TotalAmount,
                createdAt = order.CreatedAt,
                items = broadcastItems
            });

            // 4. Trigger Telegram Push Notification channel (Asynchronous Fire & Forget layout)
            _= SendTelegramNotificationAsync(order.OrderId, order.TableId, order.TotalAmount, broadcastItems);

            return order.Id;
        }

        // Private background pipeline worker for building the Telegram payload message
        private async Task SendTelegramNotificationAsync(string orderId, string? tableId, decimal total, IEnumerable<dynamic> items)
        {
            try
            {
                string botToken = _config["Telegram:BotToken"];
                string chatId = _config["Telegram:ChatId"];

                if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId)) return;

                string url = $"https://api.telegram.org/bot{botToken}/sendMessage";

                var sb = new StringBuilder();
                sb.AppendLine("<b>☕ NEW ORDER RECEIVED ☕</b>");
                sb.AppendLine("━━━━━━━━━━━━━━━━━━━━");
                sb.AppendLine($"🆔 <b>Order:</b> <code>{orderId}</code>");
                sb.AppendLine($"📍 <b>Location:</b> {tableId ?? "Takeaway"}");
                sb.AppendLine($"⏰ <b>Time:</b> {DateTime.Now:hh:mm tt}");
                sb.AppendLine("━━━━━━━━━━━━━━━━━━━━");
                sb.AppendLine("📋 <b>Items Details:</b>");

                foreach (var item in items)
                {
                    // Using standard safe HTML tags
                    sb.AppendLine($"• x{item.Quantity} <b>{item.ProductName}</b>");
                    if (!string.IsNullOrWhiteSpace(item.SpecialInstructions))
                    {
                        sb.AppendLine($"   ┗ 📝 <i>\"{item.SpecialInstructions}\"</i>");
                    }
                }

                sb.AppendLine("━━━━━━━━━━━━━━━━━━━━");
                sb.AppendLine($"💰 <b>Total Paid:</b> ${total.ToString("F2")}");

                var payload = new
                {
                    chat_id = chatId,
                    text = sb.ToString(),
                    parse_mode = "HTML" // <-- SWITCHED PARSE MODE TO HTML
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                // Let's read the server response to catch exactly what Telegram replies
                var response = await _httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Telegram API Error: {response.StatusCode} - {errorResponse}");
                }
                else
                {
                    Console.WriteLine("✅ Telegram notification pushed successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Telegram Notification System Log Interception: {ex.Message}");
            }
        }
    }
}