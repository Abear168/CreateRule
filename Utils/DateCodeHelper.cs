namespace CreateRule.Utils
{
    public static class DateCodeHelper
    {
        private const int BaseYear = 2010;

        public static string GenerateDateCode(DateTime date)
        {
            string yearCode = ConvertYearToCode(date.Year);
            string monthCode = ConvertMonthToCode(date.Month);
            string dayCode = ConvertDayToCode(date.Day);

            return $"{yearCode}{monthCode}{dayCode}";
        }

        public static string ConvertYearToCode(int year)
        {
            int offset = year - BaseYear;

            if (offset < 0)
                throw new ArgumentException($"年份不能小于 {BaseYear}");

            if (offset < 10)
                return offset.ToString();

            return ((char)('A' + offset - 10)).ToString();
        }

        public static string ConvertMonthToCode(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentException("月份必须在1-12之间");

            if (month <= 9)
                return month.ToString();

            return ((char)('A' + month - 10)).ToString();
        }

        public static string ConvertDayToCode(int day)
        {
            if (day < 1 || day > 31)
                throw new ArgumentException("日必须在1-31之间");

            if (day <= 9)
                return day.ToString();

            char baseChar = (char)('A' + (day - 10));

            if (baseChar >= 'I') baseChar++;
            if (baseChar >= 'O') baseChar++;

            return baseChar.ToString();
        }

        public static DateTime? ParseDateFromCode(string yearCode, string monthCode, string dayCode)
        {
            try
            {
                int year = ParseYearFromCode(yearCode);
                int month = ParseMonthFromCode(monthCode);
                int day = ParseDayFromCode(dayCode);

                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }

        private static int ParseYearFromCode(string code)
        {
            if (code.Length == 1 && char.IsDigit(code[0]))
            {
                return BaseYear + int.Parse(code);
            }

            return BaseYear + (code[0] - 'A' + 10);
        }

        private static int ParseMonthFromCode(string code)
        {
            if (code.Length == 1 && char.IsDigit(code[0]))
            {
                return int.Parse(code);
            }

            return code[0] - 'A' + 10;
        }

        private static int ParseDayFromCode(string code)
        {
            if (code.Length == 1 && char.IsDigit(code[0]))
            {
                return int.Parse(code);
            }

            char baseChar = code[0];

            if (baseChar > 'I') baseChar--;
            if (baseChar > 'O') baseChar--;

            return baseChar - 'A' + 10;
        }
    }
}
