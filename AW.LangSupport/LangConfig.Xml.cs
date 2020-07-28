using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AW.Base.Log;
using AW.Base.Serializer.Common;

namespace AW.LangSupport
{
    public partial class LangConfig
    {
        private const string CurrentLangFile = "current.lang";
        private const string WordsFile = "words.lang";

        internal static ILogger Logger { get; }

        static LangConfig()
        {
            Logger = new Logger("Xml", "lang_xml.log");
        }

        public void SaveAsXmlResource(string langFolder)
        {
            try
            {
                if (Directory.Exists(langFolder))
                    Directory.Delete(langFolder, true);

                Directory.CreateDirectory(langFolder);

                if (CurrentLang == null)
                    CurrentLang = Langs.FirstOrDefault();

                SerializerHelper.SaveText(CurrentLang.Id.ToString(), Path.Combine(langFolder, CurrentLangFile));

                foreach (Lang lang in Langs)
                {
                    string path = Path.Combine(langFolder, $"{lang.Id}.{lang.Name}");
                    Directory.CreateDirectory(path);

                    string text = "";
                    foreach (Word word in Words)
                        text += $"<{word.Key}>{word.Values[lang.Id]}</{word.Key}>{Environment.NewLine}";

                    SerializerHelper.SaveText(text, Path.Combine(path, WordsFile));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public bool LoadFromXmlResource(string langFolder)
        {
            try
            {
                if (!Directory.Exists(langFolder))
                    return false;

                if (int.TryParse(SerializerHelper.LoadText(Path.Combine(langFolder, CurrentLangFile)), out int landId))
                {
                    Langs.Clear();
                    Words.Clear();

                    foreach (string lang in Directory.GetDirectories(langFolder))
                    {
                        string name = new DirectoryInfo(lang).Name;

                        if (int.TryParse(name.Substring(0, name.IndexOf('.')), out int id))
                        {
                            name = name.Substring(name.IndexOf('.') + 1);

                            Langs.Add(new Lang
                            {
                                Id = id,
                                Name = name
                            });

                            string text = SerializerHelper.LoadText(Path.Combine(lang, WordsFile));

                            while (!string.IsNullOrEmpty(text))
                            {
                                if (text[0] == '<')
                                {
                                    string key = text.Substring(1, text.IndexOf('>') - 1);
                                    string value = text.Substring(text.IndexOf('>') + 1, text.IndexOf($"</{key}>") - $"</{key}>".Length + 1);

                                    Word word = Words.FirstOrDefault(w => w.Key == key);
                                    
                                    if (word == null)
                                    {
                                        word = new Word
                                        {
                                            Key = key,
                                            Values = new Dictionary<int, string>()
                                        };

                                        Words.Add(word);
                                    }

                                    if (word.Values.ContainsKey(id))
                                        word.Values[id] = value;
                                    else
                                        word.Values.Add(id, value);

                                    text = text.Substring(key.Length * 2 + value.Length + 5).TrimStart('\r', '\n');
                                }
                            }
                        }
                    }

                    CurrentLang = Langs.FirstOrDefault(l => l.Id == landId) ?? Langs.FirstOrDefault();
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return false;
        }
    }
}
