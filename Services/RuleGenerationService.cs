using System.Text.RegularExpressions;
using System.Web;
using CreateRule.Models;
using CreateRule.Utils;

namespace CreateRule.Services
{
    public static class RuleGenerationService
    {
        private static readonly Regex ComponentRegex = new Regex(@"\{(PREFIX|VAR|CONST|DATE|SEQ)\}", RegexOptions.Compiled);

        public static string GenerateRule(InnerBoxRule rule, DateTime date, int sequence)
        {
            if (string.IsNullOrEmpty(rule.Template))
                return string.Empty;

            string result = rule.Template;

            result = result.Replace("{PREFIX}", FormatPrefix(rule.Prefix, rule.PrefixLength));
            result = result.Replace("{VAR}", rule.WorkOrder);
            result = result.Replace("{CONST}", rule.Constant ?? string.Empty);
            result = result.Replace("{DATE}", DateCodeHelper.GenerateDateCode(date));
            result = result.Replace("{SEQ}", sequence.ToString().PadLeft(rule.SequenceLength, '0'));

            return result;
        }

        public static string GeneratePreview(InnerBoxRule rule, DateTime date)
        {
            return GenerateRule(rule, date, rule.SequenceStart);
        }

        private static string FormatPrefix(string prefix, int length)
        {
            if (string.IsNullOrEmpty(prefix))
                return string.Empty;

            if (prefix.Length >= length)
                return prefix.Substring(0, length);

            return prefix.PadLeft(length, '0');
        }

        public static List<string> ParseTemplate(string template)
        {
            var components = new List<string>();
            var matches = ComponentRegex.Matches(template);

            foreach (Match match in matches)
            {
                components.Add(match.Value);
            }

            return components.Distinct().ToList();
        }

        public static string InsertComponent(string template, string component)
        {
            if (string.IsNullOrEmpty(template))
                return component;

            return template + component;
        }
    }
}
