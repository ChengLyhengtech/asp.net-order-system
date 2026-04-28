using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
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

        [HttpGet("generate/{tableId}")]
        //[AllowAnonymous]
        public ActionResult<GenerateQrResponseDto> GenerateQrForTabl(string tableId)
        {
            if (string.IsNullOrWhiteSpace(tableId))
            {
                return BadRequest("Table ID cannot be empty.");
            }

            try
            {
                var result = _tableQrService.GenerateQrForTable(tableId);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = "Issue creat QR Code", Error = ex.Message });
            }
        }
    }
}