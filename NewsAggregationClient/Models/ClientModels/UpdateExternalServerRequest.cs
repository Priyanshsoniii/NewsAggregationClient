namespace NewsAggregationClient.Models.ClientModels
{
        public class UpdateExternalServerRequest
        {
            public int ServerId { get; set; }
            public string ApiKey { get; set; } = string.Empty;
        }

        public class AddCategoryRequest
        {
            public string CategoryName { get; set; } = string.Empty;
        }
}
