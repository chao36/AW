using System;
using System.Collections.Generic;

using AW.Base.Serializer.Common;

namespace AW.Base.Serializer
{
    public partial class AWSerializer : IDisposable
    {
        private const char FirstST = '~';
        private const string StringT = "~[";

        private List<string> TypeTabel { get; set; }


        public Type GetSaveType(string name)
        {
            if (int.TryParse(name, out int index) && index < TypeTabel?.Count)
                name = TypeTabel[index];

            return SerializerHelper.GetType(name);
        }


        private int GetTypeToSave(object obj)
            => GetTypeToSave(obj.GetType());


        private int GetTypeToSave(Type t)
        {
            if (!TypeTabel.Contains(t.FullName))
                TypeTabel.Add(t.FullName);

            return TypeTabel.IndexOf(t.FullName);
        }


        public void Dispose()
        {
            Builder?.Clear();
            TypeTabel?.Clear();

            Sources?.Clear();
        }
    }
}
