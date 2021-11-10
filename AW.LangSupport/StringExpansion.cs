using AW.LangSupport;

namespace AW
{
    public static class StringExpansion
    {
        /// <summary>
        /// Переводит слово для текущего языка
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Translate(this string str)
            => !str.IsNull()
                ? LangConfig.Instane.GetValue(str) ?? str
                : str;
    }
}
