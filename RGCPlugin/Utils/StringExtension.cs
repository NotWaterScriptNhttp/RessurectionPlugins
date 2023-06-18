namespace RGCPlugin.Utils
{
    public static class StringExtensions
    {
        public static int NumberOfChars(this string str, string target) => str.Length - str.Replace(target, " ").Length;
        public static string Shift(this string str, int places, out string current)
        {
            current = str.Substring(places);
            return str.Substring(0, places);
        }
        public static string Replicate(this string str, int copies)
        {
            string res = "";
            for (int i = 0; i < copies; i++)
                res += str;

            return res;
        }
    }
}
