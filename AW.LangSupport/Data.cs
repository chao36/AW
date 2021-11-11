using System.Collections.Generic;

namespace AW.LangSupport
{
    /// <summary>
    /// Lang 
    /// </summary>
    [AWSerializable]
    public class Lang : IReference
    {
        /// <inheritdoc/>
        public int ReferenceId { get; set; }

        /// <summary>
        /// Lang id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Lang name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Word
    /// </summary>
    [AWSerializable]
    public class Word
    {
        /// <summary>
        /// Word key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Word values for langs
        /// </summary>
        public Dictionary<int, string> Values { get; set; }
    }
}
