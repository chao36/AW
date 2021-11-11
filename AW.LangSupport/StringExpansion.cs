using AW.LangSupport;

namespace AW
{
    /// <summary>
    /// Translate expansion
    /// </summary>
    public static class StringExpansion
    {
        /// <summary>
        /// Translate word for current lang in config
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Translate(this string str)
            => !str.IsNull()
                ? LangConfig.Instane.GetValue(str) ?? str
                : str;
    }
}
