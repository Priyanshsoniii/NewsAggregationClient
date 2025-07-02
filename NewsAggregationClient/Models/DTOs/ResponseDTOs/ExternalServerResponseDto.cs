namespace NewsAggregationClient.Models.DTOs.ResponseDTOs
{
    public class ExternalServerResponseDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ServerType { get; set; }
        public string? Status { get; set; }
        public string? LastAccessed { get; set; }
        public string? ApiUrl { get; set; }
        public int? RequestsPerHour { get; set; }
        public int? CurrentHourRequests { get; set; }
        public string? CreatedAt { get; set; }
    }
} 