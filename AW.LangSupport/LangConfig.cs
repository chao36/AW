using System.Collections.Generic;
using System.Linq;

namespace AW.LangSupport
{
    [AWSerializable]
    public partial class LangConfig
    {
        [AWIgnore]
        public static LangConfig Instane { get; set; } = new LangConfig();

        [AWReference]
        public Lang CurrentLang { get; set; }

        public List<Lang> Langs { get; set; } = new List<Lang>();
        public List<Word> Words { get; set; } = new List<Word>();


        public Lang AddLang(string name)
        {
            if (Langs.Any(l => l.Name.ToLower() == name.ToLower()))
                return Langs.FirstOrDefault(l => l.Name.ToLower() == name.ToLower());

            Langs.Add(new Lang { Id = Langs.Count != 0 ? Langs.Last().Id + 1 : 1, Name = name });
            return Langs.Last();
        }


        public void RemoveLang(Lang lang)
        {
            Langs.Remove(lang);

            if (CurrentLang == lang)
                CurrentLang = Langs.FirstOrDefault();

            foreach (var word in Words)
                word.Values?.Remove(lang.Id);
        }


        public Word AddWord(string key)
        {
            if (Words.Any(w => w.Key.ToLower() == key.ToLower()))
                return Words.FirstOrDefault(w => w.Key.ToLower() == key.ToLower());

            Words.Add(new Word { Key = key, Values = new Dictionary<int, string>() });
            return Words.Last();
        }


        public void RemoveWord(Word word)
            => Words.Remove(word);


        public void SetValue(string wordKey, string langName, string value)
            => SetValue(
                Words.FirstOrDefault(w => w.Key.ToLower() == wordKey.ToLower()),
                Langs.FirstOrDefault(l => l.Name.ToLower() == langName.ToLower()),
                value);


        public void SetValue(Word word, Lang lang, string value)
        {
            if (word == null || lang == null)
                return;

            if (word.Values == null)
                word.Values = new Dictionary<int, string>();

            if (word.Values.ContainsKey(lang.Id))
            {
                if (value.IsNull())
                    word.Values.Remove(lang.Id);
                else
                    word.Values[lang.Id] = value;
            }
            else if (!value.IsNull())
                word.Values.Add(lang.Id, value);
        }


        public string GetValue(string key)
            => GetValue(Words.FirstOrDefault(w => w.Key.ToLower() == key.ToLower()));


        public string GetValue(Word word)
        {
            if (word == null || !word.Values.ContainsKey(CurrentLang.Id))
                return null;

            return word.Values[CurrentLang.Id];
        }
    }
}
