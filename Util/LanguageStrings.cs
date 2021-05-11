using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SFCore.Utils
{
    public class LanguageStrings
    {
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> jsonDict;

        public LanguageStrings(Assembly asm, string resourceName, Encoding encoding = null)
        {
            using (Stream s = asm.GetManifestResourceStream(resourceName))
            {
                if (s == null) return;

                byte[] buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);
                s.Dispose();

                string json = (encoding == null ? Encoding.Default : encoding).GetString(buffer);

                jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
            }
        }

        public LanguageStrings(string json)
        {
            jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
        }

        public string Get(string key, string sheet)
        {
            GlobalEnums.SupportedLanguages lang = GameManager.instance.gameSettings.gameLanguage;
            string ret;
            if (!jsonDict.ContainsKey(lang.ToString()))
            {
                // `lang` doesn't exist in dict
                ret = jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet][key];
            }
            else if (!jsonDict[lang.ToString()].ContainsKey(sheet))
            {
                // `lang` exists, but `sheet` doesn't
                ret = jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet][key];
            }
            else
            {
                // `lang` and `sheet` exist
                ret = jsonDict[lang.ToString()][sheet][key];
            }
            return ret.Replace("<br>", "\n");
        }

        public bool ContainsKey(string key, string sheet)
        {
            GlobalEnums.SupportedLanguages lang = GameManager.instance.gameSettings.gameLanguage;
            if (!jsonDict.ContainsKey(lang.ToString()))
            {
                // `lang` doesn't exist in dict
                if (!jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()].ContainsKey(sheet))
                    return false;
                return jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet].ContainsKey(key);
            }
            if (!jsonDict[lang.ToString()].ContainsKey(sheet))
            {
                // `lang` exists, but `sheet` doesn't
                if (!jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()].ContainsKey(sheet))
                    return false;
                return jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet].ContainsKey(key);
            }
            // `lang` and `sheet` exist
            return jsonDict[lang.ToString()][sheet].ContainsKey(key);
        }
    }
}