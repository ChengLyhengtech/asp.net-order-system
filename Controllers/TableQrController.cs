using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableQrController : ControllerBase
    {
        private readonly ITableQrService _tableQrService;
        public TableQrController(ITableQrService tableQrService)
        {
            _tableQrService = tableQrService;
        }

        // POST/GET: api/TableQr/generate/{tableId}
        [HttpGet("generate/{tableId}")]
        public async Task<ActionResult<GenerateQrResponseDto>> GenerateQrForTable(string tableId)
        {
            if (string.IsNullOrWhiteSpace(tableId))
            {
                return BadRequest("Table ID cannot be empty.");
            }

            try
            {
                var result = await _tableQrService.GenerateQrForTableAsync(tableId);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "Issue creating QR Code", Error = ex.Message });
            }
        }

        // GET: api/TableQr
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableQrCodeModel>>> GetAll()
        {
            var results = await _tableQrService.GetAllQrCodesAsync();
            return Ok(results);
        }

        // GET: api/TableQr/{tableId}
        [HttpGet("{tableId}")]
        public async Task<ActionResult<TableQrCodeModel>> GetById(string tableId)
        {
            var result = await _tableQrService.GetQrCodeByIdAsync(tableId);
            if (result == null)
            {
                return NotFound(new { Message = $"QR Code for Table {tableId} not found." });
            }
            return Ok(result);
        }

        // DELETE: api/TableQr/{tableId}
        [HttpDelete("{tableId}")]
        public async Task<IActionResult> Delete(string tableId)
        {
            var success = await _tableQrService.DeleteQrCodeAsync(tableId);
            if (!success)
            {
                return NotFound(new { Message = $"QR Code for Table {tableId} not found or already deleted." });
            }

            return Ok(new { Message = $"Table {tableId} QR Code removed from database successfully." });
        }
    }
}