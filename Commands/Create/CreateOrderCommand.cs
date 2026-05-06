using aps.net_order_system.Data;
using aps.net_order_system.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Commands
{
    public class CreateOrderItemCommand 
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; } = string.Empty; // Added
    }

    public class CreateOrderCommand
    {
        public int TableId { get; set; }
        public List<CreateOrderItemCommand> Items { get; set; } = new();
    }

    public class CreateOrderCommandHandler
    {
        private readonly AppDbContext _context;
        public CreateOrderCommandHandler(AppDbContext context) => _context = context;

        public async Task<int> Handle(CreateOrderCommand command)
        {
            if (command.Items == null || !command.Items.Any())
                throw new Exception("Order must have at least one item");

            var order = new OrderModel
            {
                OrderId = $"ORD-{Guid.NewGuid().ToString()[..5].ToUpper()}",
                TableId = command.TableId,
                Status = "Pending",
                PaymentStatus = "Unpaid", // ✅ FIX
                CreatedAt = DateTime.Now,
                OrderItems = new List<OrderItemModel>()
            };

            decimal totalAmount = 0;

            foreach (var item in command.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Quantity must be greater than 0");

                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found"); // ✅ FIX

                var subtotal = (decimal)product.Price * item.Quantity;
                totalAmount += subtotal;

                order.OrderItems.Add(new OrderItemModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SpecialInstructions = item.SpecialInstructions,
                    Subtotal = subtotal
                });
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);

            // ✅ Global counter
            var globalCounter = await _context.TotalCountOrders.FirstOrDefaultAsync();

            if (globalCounter == null)
            {
                _context.TotalCountOrders.Add(new TotalCountOderModel { TotalCount = 1 });
            }
            else
            {
                globalCounter.TotalCount += 1;
            }

            await _context.SaveChangesAsync();

            return order.Id;
        }
    }
}