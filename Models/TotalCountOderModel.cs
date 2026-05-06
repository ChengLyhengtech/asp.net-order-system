using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.Models
{
    public class TotalCountOderModel
    {
        [Key]
        public int Id { get; set; }
        public int TotalCount { get; set; }
    }
}
