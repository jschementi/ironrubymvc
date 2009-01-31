namespace IronRubyMvc
{
    public static class StringExtensions
    {
        public static bool IsNullOrBlank(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }

        public static bool IsNotNullOrBlank(this string value)
        {
            return !value.IsNullOrBlank();
        }
    }
}