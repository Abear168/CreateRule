using System.Net.Http;
using System.Text;
using System.Text.Json;
using CreateRule.Models;

namespace CreateRule.Services
{
    public class WorkOrderService
    {
        private readonly HttpClient _httpClient;
        private string? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        private const string LoginUrl = "http://10.10.150.250:8080/hmmes/api/ac/login";
        private const string WorkOrderUrl = "http://10.10.150.250/hmmes/api/workOrder/page";

        public WorkOrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Console.WriteLine("WorkOrderService 构造函数被调用");
        }

        private async Task<string> GetTokenAsync()
        {
            Console.WriteLine("=== GetTokenAsync 开始 ===");

            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.Now < _tokenExpiry)
            {
                Console.WriteLine("使用缓存的Token");
                return _cachedToken;
            }

            Console.WriteLine($"正在登录: {LoginUrl}");

            var loginRequest = new LoginRequest
            {
                Account = "superadmin",
                Password = "admin"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            Console.WriteLine($"登录请求: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(LoginUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"登录响应: {responseBody}");

            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody);

            if (loginResponse?.Success == true && !string.IsNullOrEmpty(loginResponse.Result?.TokenValue))
            {
                _cachedToken = loginResponse.Result.TokenValue;
                _tokenExpiry = DateTime.Now.AddMinutes(60);
                Console.WriteLine($"获取Token成功: {_cachedToken}");
                return _cachedToken;
            }

            throw new Exception($"获取Token失败: {loginResponse?.Msg ?? "未知错误"}");
        }

        public async Task<List<string>> GetWorkOrderNamesAsync()
        {
            Console.WriteLine("=== GetWorkOrderNamesAsync 开始 ===");

            var token = await GetTokenAsync();

            var request = new WorkOrderRequest
            {
                CreateUserId = "1",
                ModifyUserId = "1",
                PageSize = 1000,
                PageNumber = 1,
                StartCreateTimeDay = "20250101",
                EndCreateTimeDay = "20300101"
            };

            var json = JsonSerializer.Serialize(request);
            Console.WriteLine($"工单请求: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, WorkOrderUrl);
            httpRequest.Content = content;
            httpRequest.Headers.Add("tokenValue", token);

            Console.WriteLine($"正在请求工单列表...");

            var response = await _httpClient.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"工单响应: {responseBody}");

            var workOrderResponse = JsonSerializer.Deserialize<WorkOrderResponse>(responseBody);

            if (workOrderResponse?.Success == true && workOrderResponse.Result != null)
            {
                var names = workOrderResponse.Result.Records
                    .Select(r => r.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .ToList();
                Console.WriteLine($"获取到 {names.Count} 个工单");
                return names;
            }

            throw new Exception($"获取工单列表失败: {workOrderResponse?.Msg ?? "未知错误"}");
        }
    }
}
