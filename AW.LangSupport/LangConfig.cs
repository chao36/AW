using System.Collections.Generic;
using System.Linq;

namespace AW.LangSupport
{
    /// <summary>
    /// lang config
    /// </summary>
    [AWSerializable]
    public partial class LangConfig
    {
        /// <summary>
        /// Current lang config
        /// </summary>
        [AWIgnore]
        public static LangConfig Instane { get; set; } = new LangConfig();

        /// <summary>
        /// Current lang
        /// </summary>
        [AWReference]
        public Lang CurrentLang { get; set; }

        /// <summary>
        /// Langs
        /// </summary>
        public List<Lang> Langs { get; set; } = new List<Lang>();

        /// <summary>
        /// Words
        /// </summary>
        public List<Word> Words { get; set; } = new List<Word>();

        /// <summary>
        /// Add lang
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Lang AddLang(string name)
        {
            if (Langs.Any(l => l.Name.ToLower() == name.ToLower()))
                return Langs.FirstOrDefault(l => l.Name.ToLower() == name.ToLower());

            Langs.Add(new Lang { Id = Langs.Count != 0 ? Langs.Last().Id + 1 : 1, Name = name });
            return Langs.Last();
        }

        /// <summary>
        /// Remove lang
        /// </summary>
        /// <param name="lang"></param>
        public void RemoveLang(Lang lang)
        {
            Langs.Remove(lang);

            if (CurrentLang == lang)
                CurrentLang = Langs.FirstOrDefault();

            foreach (var word in Words)
                word.Values?.Remove(lang.Id);
        }

        /// <summary>
        /// Add word
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Word AddWord(string key)
        {
            if (Words.Any(w => w.Key.ToLower() == key.ToLower()))
                return Words.FirstOrDefault(w => w.Key.ToLower() == key.ToLower());

            Words.Add(new Word { Key = key, Values = new Dictionary<int, string>() });
            return Words.Last();
        }

        /// <summary>
        /// Remove word
        /// </summary>
        /// <param name="word"></param>
        public void RemoveWord(Word word)
            => Words.Remove(word);

        /// <summary>
        /// Set word value for lang
        /// </summary>
        /// <param name="wordKey"></param>
        /// <param name="langName"></param>
        /// <param name="value"></param>
        public void SetValue(string wordKey, string langName, string value)
            => SetValue(
                Words.FirstOrDefault(w => w.Key.ToLower() == wordKey.ToLower()),
                Langs.FirstOrDefault(l => l.Name.ToLower() == langName.ToLower()),
                value);

        /// <summary>
        /// Set word value for lang
        /// </summary>
        /// <param name="word"></param>
        /// <param name="lang"></param>
        /// <param name="value"></param>
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

        /// <summary>
        /// Get value for key by current lang
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
            => GetValue(Words.FirstOrDefault(w => w.Key.ToLower() == key.ToLower()));

        /// <summary>
        /// Get value for word by current lang
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string GetValue(Word word)
        {
            if (word == null || !word.Values.ContainsKey(CurrentLang.Id))
                return null;

            return word.Values[CurrentLang.Id];
        }
    }
}
