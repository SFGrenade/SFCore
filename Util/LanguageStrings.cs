using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SFCore.Utils
{
    /// <summary>
    ///     Helper class to load language strings from a JSON resource.
    ///     Supports multiple languages.
    /// </summary>
    public class LanguageStrings
    {
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _jsonDict;

        /// <summary>
        ///     The constructor. Loads the JSON resource.
        /// </summary>
        /// <param name="asm">The assembly to load the resource from</param>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="encoding">The encoding of the resource</param>
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

        /// <summary>
        ///     The constructor. Loads the given JSON.
        /// </summary>
        /// <param name="json">The JSON to load</param>
        public LanguageStrings(string json)
        {
            _jsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
        }

        /// <summary>
        ///     Get a language string according to a given key and sheet.
        /// </summary>
        /// <param name="key">The language key</param>
        /// <param name="sheet">The sheet that contains the key</param>
        /// <returns>The language string.</returns>
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

        /// <summary>
        ///     Check if a language string according to a given key and sheet is present.
        /// </summary>
        /// <param name="key">The language key</param>
        /// <param name="sheet">The sheet that contains the key</param>
        /// <returns>True if the language string is present, False if not.</returns>
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