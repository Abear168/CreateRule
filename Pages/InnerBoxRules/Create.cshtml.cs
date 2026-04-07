using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CreateRule.Data;
using CreateRule.Models;
using CreateRule.Services;

namespace CreateRule.Pages.InnerBoxRules
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly WorkOrderService _workOrderService;

        public CreateModel(ApplicationDbContext context, WorkOrderService workOrderService)
        {
            _context = context;
            _workOrderService = workOrderService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public List<string> WorkOrders { get; set; } = new List<string>();

        public class InputModel
        {
            [Required(ErrorMessage = "工单号不能为空")]
            [StringLength(100)]
            public string WorkOrder { get; set; } = string.Empty;

            [Required(ErrorMessage = "规则模板不能为空")]
            [StringLength(500)]
            public string Template { get; set; } = string.Empty;

            [StringLength(100)]
            public string Prefix { get; set; } = string.Empty;

            public int PrefixLength { get; set; } = 4;

            [StringLength(100)]
            public string Constant { get; set; } = string.Empty;

            [Range(1, 10)]
            public int SequenceLength { get; set; } = 4;

            [Range(1, int.MaxValue)]
            public int SequenceStart { get; set; } = 1;
        }

        public async Task OnGetAsync()
        {
            Console.WriteLine("=== OnGetAsync 开始 ===");
            try
            {
                Console.WriteLine("开始调用 WorkOrderService.GetWorkOrderNamesAsync...");
                WorkOrders = await _workOrderService.GetWorkOrderNamesAsync();
                ViewData["WorkOrders"] = WorkOrders;
                Console.WriteLine($"获取到 {WorkOrders.Count} 个工单");
            }
            catch (Exception ex)
            {
                WorkOrders = new List<string>();
                ViewData["WorkOrders"] = WorkOrders;
                Console.WriteLine($"获取工单列表失败: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            Console.WriteLine("=== OnGetAsync 结束 ===");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            WorkOrders = ViewData["WorkOrders"] as List<string> ?? new List<string>();

            if (string.IsNullOrEmpty(Input.Template))
            {
                ModelState.AddModelError("Input.Template", "规则模板不能为空");
                return Page();
            }

            if (Input.Template.Contains("{CONST}") && string.IsNullOrEmpty(Input.Constant))
            {
                ModelState.AddModelError("Input.Constant", "当模板包含常量时请输入常量值");
                return Page();
            }

            if (Input.Template.Contains("{PREFIX}") && string.IsNullOrEmpty(Input.Prefix))
            {
                ModelState.AddModelError("Input.Prefix", "当模板包含前缀时请输入前缀值");
                return Page();
            }

            var existingRule = _context.InnerBoxRules.Any(r => r.WorkOrder == Input.WorkOrder);

            if (existingRule)
            {
                ModelState.AddModelError(string.Empty, "该工单号已存在");
                return Page();
            }

            var rule = new InnerBoxRule
            {
                WorkOrder = Input.WorkOrder ?? string.Empty,
                Template = Input.Template,
                Prefix = Input.Prefix ?? string.Empty,
                PrefixLength = Input.PrefixLength > 0 ? Input.PrefixLength : 4,
                Constant = Input.Constant ?? string.Empty,
                SequenceLength = Input.SequenceLength > 0 ? Input.SequenceLength : 4,
                SequenceStart = Input.SequenceStart > 0 ? Input.SequenceStart : 1,
                CreatedAt = DateTime.Now
            };

            Console.WriteLine($"保存规则: WorkOrder={rule.WorkOrder}, Template={rule.Template}, Constant={rule.Constant}");

            _context.InnerBoxRules.Add(rule);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"规则创建成功";
            return RedirectToPage("./Index");
        }
    }
}
