using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using Microsoft.AspNetCore.DataProtection;
using QRCoder;
using System;

namespace aps.net_order_system.Services
{
    public class TableQrService : ITableQrService
    {
        private readonly IDataProtector _protector;
        //private readonly AppDbContext _context;

        public TableQrService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("TableQrService");
        }

        public GenerateQrResponseDto GenerateQrForTable(string tableId)
        {
            // Encrypt the table ID
            string encryptedTableId = _protector.Protect(tableId);
            string encodedToken = Uri.EscapeDataString(encryptedTableId);
            string orderUrl = $"https://yourdomain.com/order?token={encodedToken}";

            // Generate QR code
            using (QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(orderUrl, QRCodeGenerator.ECCLevel.Q);

                // Use PngByteQRCode to get the QR code as a byte array
                PngByteQRCode qrCode = new PngByteQRCode(qRCodeData);
                byte[] qrCodeImageBytes = qrCode.GetGraphic(20);

                string base64QrCode = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImageBytes)}";

                return new GenerateQrResponseDto
                {
                    TableId = tableId,
                    EncryptedUrl = orderUrl,
                    QrCodeImageBase64 = base64QrCode
                };

            }
        }
    }
}