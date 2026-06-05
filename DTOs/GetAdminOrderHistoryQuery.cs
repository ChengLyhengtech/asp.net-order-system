namespace aps.net_order_system.DTOs
{
    public enum OrderDateFilter
    {
        AllTime, //0
        Today,//1
        Last7Days//2
    }

    public class GetAdminOrderHistoryQuery
    {
        public OrderDateFilter DateFilter { get; set; } = OrderDateFilter.AllTime;
        public string? SearchTerm { get; set; } // Can be OrderId (e.g. "ORD-001") or TableId (e.g. "4")
        public int Limit { get; set; } = 50;   // Default limit for history logs
    }
}
