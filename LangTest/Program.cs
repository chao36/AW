using System;

using AW.LangSupport;

namespace LangTest
{
    class Program
    {
        static LangConfig settings;
        static Lang lang;

        static void Main(string[] args)
        {
            settings = new LangConfig();

            settings.LoadFromXmlResource(@"D:\Users\Akuma\Desktop\langs");

            lang = settings.AddLang("English");

            settings.CurrentLang = lang;

            AddGeneralWords();
            AddMenuWords();

            settings.SaveAsXmlResource(@"D:\Users\Akuma\Desktop\langs");
        }

        private static void AddGeneralWords()
        {
            Add("history", "History");
            Add("rename", "Rename");
            Add("new_name", "New name");
            Add("add", "Add");
            Add("remove", "Remove");
            Add("ok", "Ok");
            Add("cancel", "Cancel");
            Add("add_folder", "Add folder");

            Add("file", "File");
            Add("edit", "Edit");
            {
                Add("undo", "Undo");
                Add("redo", "Redo");
            }

            Add("name", "Name");
            Add("search", "Search");
        }

        private static void AddMenuWords()
        {
            Add("solution", "Solution");
            Add("add", "Add");
            Add("page", "Page");
            Add("scheme", "Scheme");
            Add("model", "Model");
            Add("template", "Template");
            Add("items", "Items");
            Add("templates", "Templates");
        }

        private static void Add(string key, string value)
        {
            Word word = settings.AddWord(key);
            settings.SetValue(word, lang, value);
        }
    }
}
