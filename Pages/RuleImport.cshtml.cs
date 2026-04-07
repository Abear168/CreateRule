using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using CreateRule.Utils;

namespace CreateRule.Pages
{
    [Authorize]
    public class RuleImportModel : PageModel
    {
        private static HttpClient _httpClient;
        private readonly ILogger<RuleImportModel> _logger;
        private static readonly object _lock = new object();

        public RuleImportModel(ILogger<RuleImportModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public FilterModel FilterModel { get; set; } = new FilterModel();

        public List<MatchResult> MatchResults { get; set; } = new List<MatchResult>();

        public PaginationModel Pagination { get; set; } = new PaginationModel();

        [TempData]
        public string SuccessMessage { get; set; }

        private HttpClient GetHttpClient()
        {
            if (_httpClient == null)
            {
                lock (_lock)
                {
                    if (_httpClient == null)
                    {
                        var handler = new SocketsHttpHandler
                        {
                            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2)
                        };

                        _httpClient = new HttpClient(handler)
                        {
                            Timeout = TimeSpan.FromSeconds(90)
                        };

                        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    }
                }
            }
            return _httpClient;
        }

        public async Task OnGetAsync(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                Pagination.PageIndex = pageIndex;
                Pagination.PageSize = pageSize;

                var allData = await GetDataFromApiAsync();

                if (allData != null && allData.Any())
                {
                    var projectCounts = allData
                        .GroupBy(x => x.ProjectName)
                        .Select(g => new
                        {
                            ProjectName = g.Key,
                            Count = g.Count(),
                            Records = g.ToList()
                        })
                        .ToList();

                    _logger.LogInformation($"项目统计: {string.Join(", ", projectCounts.Select(p => $"{p.ProjectName}:{p.Count}"))}");

                    var projectsToShow = projectCounts
                        .Where(p => p.Count >= 2)
                        .ToList();

                    var distinctData = projectsToShow
                        .Select(p => p.Records.OrderByDescending(x => x.Id).FirstOrDefault())
                        .Where(x => x != null)
                        .ToList();

                    _logger.LogInformation($"去重后显示 {distinctData.Count} 个项目，原始共 {allData.Count} 条");

                    if (!distinctData.Any())
                    {
                        MatchResults = new List<MatchResult>();
                        Pagination.TotalCount = 0;
                        Pagination.TotalPages = 0;
                        SuccessMessage = "没有找到重复的项目数据";
                    }

                    var filteredData = ApplyFilter(distinctData);

                    Pagination.TotalCount = filteredData.Count;
                    Pagination.TotalPages = (int)Math.Ceiling(Pagination.TotalCount / (double)Pagination.PageSize);

                    if (Pagination.PageIndex > Pagination.TotalPages && Pagination.TotalPages > 0)
                    {
                        Pagination.PageIndex = Pagination.TotalPages;
                    }
                    else if (Pagination.PageIndex < 1)
                    {
                        Pagination.PageIndex = 1;
                    }

                    MatchResults = filteredData
                        .Skip((Pagination.PageIndex - 1) * Pagination.PageSize)
                        .Take(Pagination.PageSize)
                        .ToList();
                }
                else
                {
                    MatchResults = new List<MatchResult>();
                    Pagination.TotalCount = 0;
                    Pagination.TotalPages = 0;
                    SuccessMessage = "API返回空数据";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取数据时发生错误");
                ModelState.AddModelError(string.Empty, $"获取数据失败: {ex.Message}");
                MatchResults = new List<MatchResult>();
                Pagination.TotalCount = 0;
                Pagination.TotalPages = 0;
            }
        }

        public async Task<IActionResult> OnPostFilterAsync(int pageIndex = 1)
        {
            try
            {
                var allData = await GetDataFromApiAsync();

                if (allData == null || !allData.Any())
                {
                    MatchResults = new List<MatchResult>();
                    Pagination.TotalCount = 0;
                    Pagination.TotalPages = 0;
                    SuccessMessage = "API返回空数据";
                    return Page();
                }

                var projectCounts = allData
                    .GroupBy(x => x.ProjectName)
                    .Select(g => new
                    {
                        ProjectName = g.Key,
                        Count = g.Count(),
                        Records = g.ToList()
                    })
                    .ToList();

                _logger.LogInformation($"项目统计: {string.Join(", ", projectCounts.Select(p => $"{p.ProjectName}:{p.Count}"))}");

                var projectsToShow = projectCounts
                    .Where(p => p.Count >= 2)
                    .ToList();

                var distinctData = projectsToShow
                    .Select(p => p.Records.OrderByDescending(x => x.Id).FirstOrDefault())
                    .Where(x => x != null)
                    .ToList();

                _logger.LogInformation($"去重后显示 {distinctData.Count} 个项目，原始共 {allData.Count} 条");

                if (!distinctData.Any())
                {
                    MatchResults = new List<MatchResult>();
                    Pagination.TotalCount = 0;
                    Pagination.TotalPages = 0;
                    SuccessMessage = "没有找到重复的项目数据";
                    return Page();
                }

                var filteredData = ApplyFilter(distinctData);

                Pagination.PageIndex = pageIndex;
                Pagination.PageSize = 10;
                Pagination.TotalCount = filteredData.Count;
                Pagination.TotalPages = (int)Math.Ceiling(Pagination.TotalCount / (double)Pagination.PageSize);

                if (Pagination.PageIndex > Pagination.TotalPages && Pagination.TotalPages > 0)
                {
                    Pagination.PageIndex = Pagination.TotalPages;
                }
                else if (Pagination.PageIndex < 1)
                {
                    Pagination.PageIndex = 1;
                }

                MatchResults = filteredData
                    .Skip((Pagination.PageIndex - 1) * Pagination.PageSize)
                    .Take(Pagination.PageSize)
                    .ToList();

                SuccessMessage = $"找到 {distinctData.Count} 个重复项目，筛选后 {filteredData.Count} 条，当前显示 {MatchResults.Count} 条";

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "筛选数据时发生错误");
                ModelState.AddModelError(string.Empty, $"筛选失败: {ex.Message}");
                return null;
            }
        }

        public async Task<IActionResult> OnPostGenerateAsync(
    int[] selectedIds,
    int pageIndex = 1,
    int[] itemIds = null,
    string[] itemNames = null,
    int[] itemStartPositions = null,
    int[] itemEndPositions = null,
    string[] itemMatchStrings = null)
        {
            try
            {
                if (selectedIds == null || selectedIds.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, "请至少选择一个项目");
                    await OnGetAsync(pageIndex);
                    return Page();
                }

                var currentPageData = new List<MatchResult>();

                if (itemIds != null && itemNames != null && itemMatchStrings != null)
                {
                    for (int i = 0; i < itemIds.Length; i++)
                    {
                        currentPageData.Add(new MatchResult
                        {
                            Id = itemIds[i],
                            ProjectName = itemNames[i],
                            StartPosition = itemStartPositions != null && i < itemStartPositions.Length ? itemStartPositions[i] : 0,
                            EndPosition = itemEndPositions != null && i < itemEndPositions.Length ? itemEndPositions[i] : 0,
                            MatchString = itemMatchStrings[i]
                        });
                    }
                }

                var selectedItems = currentPageData
                    .Where(x => selectedIds.Contains(x.Id))
                    .ToList();

                if (!selectedItems.Any())
                {
                    ModelState.AddModelError(string.Empty, "选中的项目不存在");
                    await OnGetAsync(pageIndex);
                    return Page();
                }

                if (!FilterModel.StartTime.HasValue || !FilterModel.EndTime.HasValue)
                {
                    ModelState.AddModelError(string.Empty, "请填写开始时间和结束时间");
                    await OnGetAsync(pageIndex);
                    return Page();
                }

                if (FilterModel.StartTime >= FilterModel.EndTime)
                {
                    ModelState.AddModelError(string.Empty, "开始时间必须早于结束时间");
                    await OnGetAsync(pageIndex);
                    return Page();
                }

                DateTime startDate = FilterModel.StartTime.Value.Date;
                DateTime endDate = FilterModel.EndTime.Value.Date;
                int totalDays = (int)(endDate - startDate).TotalDays + 1;

                var client = GetHttpClient();
                int successCount = 0;
                int failureCount = 0;
                var errorMessages = new List<string>();

                for (int day = 0; day < totalDays; day++)
                {
                    DateTime currentDate = startDate.AddDays(day);

                    string dateCode = DateCodeHelper.GenerateDateCode(currentDate);

                    foreach (var item in selectedItems)
                    {
                        try
                        {
                            string originalRule = item.MatchString;
                            string newRule = GenerateRuleWithDate(originalRule, dateCode);

                            var requestData = new
                            {
                                stationId = item.ProjectName,
                                startIndex = item.StartPosition,
                                endIndex = item.EndPosition,
                                logicalStr = newRule.Replace("^.{0}", "").Replace(".*$", "").Replace("\\", ""),
                                rule = newRule,
                                remark= item.ProjectName
                            };

                            string url = "http://192.168.8.211:8006/databus/config/createCheckRule";
                            string json = JsonSerializer.Serialize(requestData);
                            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                            _logger.LogInformation($"调用接口: {url}, 数据: {json}");

                            var response = await client.PostAsync(url, content);

                            if (response.IsSuccessStatusCode)
                            {
                                ModelState.AddModelError(string.Empty, $"{item.ProjectName}生成成功  ");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, $"{item.ProjectName}生成失败   ");
                            }
                        }
                        catch (Exception ex)
                        {
                            failureCount++;
                            string errorMsg = $"{item.ProjectName} - {currentDate:yyyy-MM-dd}: {ex.Message}";
                            errorMessages.Add(errorMsg);
                            _logger.LogError(ex, $"生成规则异常: {errorMsg}");
                        }
                    }
                }

                return RedirectToPage(new { handler = "Filter", pageIndex });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成操作时发生错误");
                ModelState.AddModelError(string.Empty, $"生成失败: {ex.Message}");
                await OnGetAsync(pageIndex);
                return Page();
            }
        }

        private string GenerateRuleWithDate(string originalRule, string dateCode)
        {
            if (string.IsNullOrEmpty(originalRule))
                return $".*{dateCode}.*$";

            if (originalRule.EndsWith(".*$"))
            {
                return originalRule.Substring(0, originalRule.Length - 6) + dateCode + ".*$";
            }
            else if (originalRule.EndsWith("$"))
            {
                return originalRule.Substring(0, originalRule.Length - 1) + dateCode + "$";
            }
            else
            {
                return originalRule + dateCode;
            }
        }

        public async Task<IActionResult> OnGetResetAsync()
        {
            try
            {
                FilterModel = new FilterModel();
                SuccessMessage = "筛选条件已重置";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置操作时发生错误");
                ModelState.AddModelError(string.Empty, $"重置失败: {ex.Message}");
                return null;
            }
        }

        private List<MatchResult> ApplyFilter(List<MatchResult> data)
        {
            var filteredData = data.Where(item =>
                (string.IsNullOrEmpty(FilterModel.ProjectName) ||
                 item.ProjectName?.Contains(FilterModel.ProjectName, StringComparison.OrdinalIgnoreCase) == true) &&
                (string.IsNullOrEmpty(FilterModel.MatchString) ||
                 item.MatchString?.Contains(FilterModel.MatchString, StringComparison.OrdinalIgnoreCase) == true)
            ).ToList();

            return filteredData;
        }

        private async Task<List<MatchResult>> GetDataFromApiAsync()
        {
            var client = GetHttpClient();

            try
            {
                var url = "http://192.168.8.211:8006/databus/config/allCheckRules";

                _logger.LogInformation($"正在从API获取数据: {url}");

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"API响应: {jsonString}");

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse?.Code != 0)
                {
                    _logger.LogError($"API返回错误: {apiResponse?.Msg}");
                    throw new Exception($"API错误: {apiResponse?.Msg}");
                }

                if (apiResponse.Result == null || !apiResponse.Result.Any())
                {
                    _logger.LogInformation("API返回空数据");
                    return new List<MatchResult>();
                }

                var results = new List<MatchResult>();

                foreach (var item in apiResponse.Result)
                {
                    var positions = ParsePositionsFromRule(item.Rule);

                    results.Add(new MatchResult
                    {
                        Id = item.Id,
                        ProjectName = !string.IsNullOrEmpty(item.Remark) ? item.Remark : $"项目_{item.Id}",
                        StartPosition = positions.Start,
                        EndPosition = positions.End,
                        MatchString = item.Rule
                    });
                }

                Pagination.allCount = results.Count;
                _logger.LogInformation($"成功获取 {results.Count} 条原始数据");
                return results;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP请求失败");
                throw new Exception($"API请求失败: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON解析失败");
                throw new Exception($"数据解析失败: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取数据失败");
                throw new Exception($"获取数据失败: {ex.Message}");
            }
        }

        private (int Start, int End) ParsePositionsFromRule(string rule)
        {
            string aa = rule.Replace("^.{0}","").Replace(".*$","").Replace("\\","");
            return (1, aa.Length);
        }
    }

    public class ApiResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<ApiResultItem> Result { get; set; }
    }

    public class ApiResultItem
    {
        public int Id { get; set; }
        public string Remark { get; set; }
        public string Rule { get; set; }
    }

    public class FilterModel
    {
        [ValidateNever]
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [ValidateNever]
        [Display(Name = "匹配字符串")]
        public string MatchString { get; set; }

        [Display(Name = "开始时间")]
        [DataType(DataType.DateTime)]
        public DateTime? StartTime { get; set; }

        [Display(Name = "结束时间")]
        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }
    }

    public class MatchResult
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public string MatchString { get; set; }
    }

    public class PaginationModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public int allCount { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
