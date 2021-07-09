using System.Collections.Generic;

using AW.Base.Serializer.Common;

namespace AW.LangSupport
{
    [AWSerializable]
    public class Lang : IReference
    {
        public int ReferenceId { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
    }


    [AWSerializable]
    public class Word
    {
        public string Key { get; set; }
        public Dictionary<int, string> Values { get; set; }
    }
}
