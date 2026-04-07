using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.InnerBoxRules
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            public int Id { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var rule = await _context.InnerBoxRules.FindAsync(id);
            if (rule == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = rule.Id,
                WorkOrder = rule.WorkOrder,
                Template = rule.Template,
                Prefix = rule.Prefix,
                PrefixLength = rule.PrefixLength,
                Constant = rule.Constant,
                SequenceLength = rule.SequenceLength,
                SequenceStart = rule.SequenceStart
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"=== OnPostAsync Debug ===");
            Console.WriteLine($"Input.Id: {Input.Id}");
            Console.WriteLine($"Input.WorkOrder: {Input.WorkOrder}");
            Console.WriteLine($"Input.Template: {Input.Template}");
            Console.WriteLine($"Input.Prefix: {Input.Prefix}");
            Console.WriteLine($"Input.Constant: '{Input.Constant}'");
            Console.WriteLine($"ModelState.IsValid (before): {ModelState.IsValid}");

            ModelState.Clear();

            if (string.IsNullOrEmpty(Input.Template))
            {
                ModelState.AddModelError("Input.Template", "规则模板不能为空");
            }

            if (string.IsNullOrEmpty(Input.WorkOrder))
            {
                ModelState.AddModelError("Input.WorkOrder", "工单号不能为空");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine($"ModelState.IsValid (after): {ModelState.IsValid}");
                return Page();
            }

            var rule = await _context.InnerBoxRules.FindAsync(Input.Id);
            if (rule == null)
            {
                return NotFound();
            }

            rule.WorkOrder = Input.WorkOrder;
            rule.Template = Input.Template;
            rule.Prefix = Input.Prefix;
            rule.PrefixLength = Input.PrefixLength > 0 ? Input.PrefixLength : 4;
            rule.Constant = Input.Constant ?? string.Empty;
            rule.SequenceLength = Input.SequenceLength > 0 ? Input.SequenceLength : 4;
            rule.SequenceStart = Input.SequenceStart > 0 ? Input.SequenceStart : 1;
            rule.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "规则更新成功";
            return RedirectToPage("./Index");
        }
    }
}