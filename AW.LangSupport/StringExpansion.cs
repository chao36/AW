using AW.LangSupport;

namespace AW
{
    public static class StringExpansion
    {
        public static string Translate(this string str)
            => !str.IsNull()
                ? LangConfig.Instane.GetValue(str) ?? str
                : str;
    }
}
