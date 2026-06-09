using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QRCoder;
using System;

namespace aps.net_order_system.Services
{
    public class TableQrService : ITableQrService
    {
        private readonly IDataProtector _protector;
        private readonly AppDbContext _context; // Inject DbContext

        public TableQrService(IDataProtectionProvider provider, AppDbContext context)
        {
            _protector = provider.CreateProtector("TableQrService");
            _context = context;
        }

        public async Task<GenerateQrResponseDto> GenerateQrForTableAsync(string tableId)
        {
            // 1. Check if it already exists in the database to avoid duplicate generations
            var existingQr = await _context.TableQrCodes.FindAsync(tableId);
           
            if (existingQr != null)
            {
                // EDGE CASE: If the table was deleted before, "revive" it instead of failing!
                if (existingQr.IsDeleted)
                {
                    existingQr.IsDeleted = false;
                    await _context.SaveChangesAsync();
                }

                return new GenerateQrResponseDto
                {
                    TableId = existingQr.TableId,
                    EncryptedUrl = existingQr.EncryptedUrl,
                    QrCodeImageBase64 = existingQr.QrCodeImageBase64
                };
            }

            // 2. Encryption and Token generation
            string encryptedTableId = _protector.Protect(tableId);
            string encodedToken = Uri.EscapeDataString(encryptedTableId);
            string orderUrl = $"http://localhost:5173/TableQr/{tableId}";

            // 3. Generate QR code
            using (QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(orderUrl, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qRCodeData);
                byte[] qrCodeImageBytes = qrCode.GetGraphic(20);
                string base64QrCode = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImageBytes)}";

                // 4. Map to entity model
                var newTableQr = new TableQrCodeModel
                {
                    TableId = tableId,
                    EncryptedUrl = orderUrl,
                    QrCodeImageBase64 = base64QrCode
                };

                // 5. Save to Database
                _context.TableQrCodes.Add(newTableQr);
                await _context.SaveChangesAsync();

                return new GenerateQrResponseDto
                {
                    TableId = tableId,
                    EncryptedUrl = orderUrl,
                    QrCodeImageBase64 = base64QrCode
                };
            }
        }

        public async Task<IEnumerable<TableQrCodeModel>> GetAllQrCodesAsync()
        {
            // ONLY return tables that are not soft-deleted
            return await _context.TableQrCodes
                .Where(table => !table.IsDeleted)
                .ToListAsync();
        }

        public async Task<TableQrCodeModel?> GetQrCodeByIdAsync(string tableId)
        {
            // Double check that the specific table requested isn't soft-deleted
            return await _context.TableQrCodes
                .FirstOrDefaultAsync(table => table.TableId == tableId && !table.IsDeleted);
        }

        public async Task<bool> DeleteQrCodeAsync(string tableId)
        {
            var qrCode = await _context.TableQrCodes.FindAsync(tableId);
            // If it doesn't exist, or it's already soft-deleted, return false
            if (qrCode == null || qrCode.IsDeleted) return false;

            qrCode.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public GenerateQrResponseDto GenerateQrForTable(string tableId)
        {
            throw new NotImplementedException();
        }
    }
}
