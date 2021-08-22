using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SFCore.Utils
{
    public class LanguageStrings
    {
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _jsonDict;

        public LanguageStrings(Assembly asm, string resourceName, Encoding encoding = null)
        {
            using (Stream s = asm.GetManifestResourceStream(resourceName))
            {
                if (s == null) return;

                byte[] buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);
                s.Dispose();

                string json = (encoding == null ? Encoding.Default : encoding).GetString(buffer);

                _jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
            }
        }

        public LanguageStrings(string json)
        {
            _jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
        }

        public string Get(string key, string sheet)
        {
            GlobalEnums.SupportedLanguages lang = GameManager.instance.gameSettings.gameLanguage;
            string ret;
            if (!_jsonDict.ContainsKey(lang.ToString()))
            {
                // `lang` doesn't exist in dict
                ret = _jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet][key];
            }
            else if (!_jsonDict[lang.ToString()].ContainsKey(sheet))
            {
                // `lang` exists, but `sheet` doesn't
                ret = _jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet][key];
            }
            else
            {
                // `lang` and `sheet` exist
                ret = _jsonDict[lang.ToString()][sheet][key];
            }
            return ret.Replace("<br>", "\n");
        }

        public bool ContainsKey(string key, string sheet)
        {
            GlobalEnums.SupportedLanguages lang = GameManager.instance.gameSettings.gameLanguage;
            if (!_jsonDict.ContainsKey(lang.ToString()))
            {
                // `lang` doesn't exist in dict
                if (!_jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()].ContainsKey(sheet))
                    return false;
                return _jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet].ContainsKey(key);
            }
            if (!_jsonDict[lang.ToString()].ContainsKey(sheet))
            {
                // `lang` exists, but `sheet` doesn't
                if (!_jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()].ContainsKey(sheet))
                    return false;
                return _jsonDict[GlobalEnums.SupportedLanguages.EN.ToString()][sheet].ContainsKey(key);
            }
            // `lang` and `sheet` exist
            return _jsonDict[lang.ToString()][sheet].ContainsKey(key);
        }
    }
}