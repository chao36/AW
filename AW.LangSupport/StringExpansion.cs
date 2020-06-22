namespace AW.LangSupport
{
    public static class StringExpansion
    {
        public static string Translate(this string str)
            => !string.IsNullOrEmpty(str)
                ? LangConfig.Instane.GetValue(str) ?? str
                : str;
    }
}
