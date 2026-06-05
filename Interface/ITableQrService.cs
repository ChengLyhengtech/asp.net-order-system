using aps.net_order_system.DTOs;
using aps.net_order_system.Models;

namespace aps.net_order_system.Interface
{
    public interface ITableQrService
    {
        Task<GenerateQrResponseDto> GenerateQrForTableAsync(string tableId);
        Task<IEnumerable<TableQrCodeModel>> GetAllQrCodesAsync();
        Task<TableQrCodeModel?> GetQrCodeByIdAsync(string tableId);
        Task<bool> DeleteQrCodeAsync(string tableId);
    }
}