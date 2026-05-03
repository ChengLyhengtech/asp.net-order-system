using aps.net_order_system.DTOs;

namespace aps.net_order_system.Interface
{
    public interface ITableQrService
    {
        GenerateQrResponseDto GenerateQrForTable(string tableId);
    }
}