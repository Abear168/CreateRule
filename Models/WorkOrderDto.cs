using System.Text.Json.Serialization;

namespace CreateRule.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = "superadmin";

        [JsonPropertyName("password")]
        public string Password { get; set; } = "admin";
    }

    public class LoginResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonPropertyName("result")]
        public LoginResult? Result { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }
    }

    public class LoginResult
    {
        [JsonPropertyName("tokenName")]
        public string TokenName { get; set; } = string.Empty;

        [JsonPropertyName("tokenValue")]
        public string TokenValue { get; set; } = string.Empty;

        [JsonPropertyName("isLogin")]
        public bool IsLogin { get; set; }
    }

    public class WorkOrderRequest
    {
        [JsonPropertyName("createUserId")]
        public string CreateUserId { get; set; } = "1";

        [JsonPropertyName("modifyUserId")]
        public string ModifyUserId { get; set; } = "1";

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 1000;

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; } = 1;

        [JsonPropertyName("startCreateTimeDay")]
        public string StartCreateTimeDay { get; set; } = "20250101";

        [JsonPropertyName("endCreateTimeDay")]
        public string EndCreateTimeDay { get; set; } = "20300101";
    }

    public class WorkOrderResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonPropertyName("result")]
        public WorkOrderResult? Result { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }
    }

    public class WorkOrderResult
    {
        [JsonPropertyName("records")]
        public List<WorkOrderRecord> Records { get; set; } = new List<WorkOrderRecord>();

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalRow")]
        public int TotalRow { get; set; }
    }

    public class WorkOrderRecord
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("workOrderType")]
        public int WorkOrderType { get; set; }

        [JsonPropertyName("workOrderTypeName")]
        public string WorkOrderTypeName { get; set; } = string.Empty;

        [JsonPropertyName("partsName")]
        public string PartsName { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("startDay")]
        public int StartDay { get; set; }

        [JsonPropertyName("endDay")]
        public int EndDay { get; set; }
    }
}
