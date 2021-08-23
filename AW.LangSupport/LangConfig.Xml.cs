using AW.Log;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

                foreach (var lang in Langs)
                {
                    var path = Path.Combine(langFolder, $"{lang.Id}.{lang.Name}");
                    Directory.CreateDirectory(path);

                    var text = "";
                    foreach (var word in Words)
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

                if (SerializerHelper.LoadText(Path.Combine(langFolder, CurrentLangFile)).TryInt(out int landId))
                {
                    Langs.Clear();
                    Words.Clear();

                    foreach (var lang in Directory.GetDirectories(langFolder))
                    {
                        var name = new DirectoryInfo(lang).Name;

                        if (name.Substring(0, name.IndexOf('.')).TryInt(out int id))
                        {
                            name = name.Substring(name.IndexOf('.') + 1);

                            Langs.Add(new Lang
                            {
                                Id = id,
                                Name = name
                            });

                            var text = SerializerHelper.LoadText(Path.Combine(lang, WordsFile));

                            while (!text.IsNull())
                            {
                                if (text[0] == '<')
                                {
                                    var key = text.Substring(1, text.IndexOf('>') - 1);
                                    var value = text.Substring(text.IndexOf('>') + 1, text.IndexOf($"</{key}>") - $"</{key}>".Length + 1);

                                    var word = Words.FirstOrDefault(w => w.Key == key);
                                    
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
