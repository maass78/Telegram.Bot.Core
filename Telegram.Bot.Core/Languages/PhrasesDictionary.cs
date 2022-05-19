using System;
using System.Collections.Generic;
using Telegram.Bot.Core.Languages.Exceptions;

namespace Telegram.Bot.Core.Languages
{
    public static class PhrasesDictionary
    {
        public static Dictionary<LanguageCode, Dictionary<string, string>> LanguagePhrases { get; }

        /// <summary>
        /// Язык по умолчанию. По умолчанию равен <see cref="LanguageCode.Russian"/>
        /// </summary>
        public static LanguageCode DefaultLanguage { get; set; }

        static PhrasesDictionary()
        {
            LanguagePhrases = new Dictionary<LanguageCode, Dictionary<string, string>>();

            foreach (LanguageCode item in Enum.GetValues(typeof(LanguageCode)))
            {
                LanguagePhrases.Add(item, new Dictionary<string, string>());
            }

            DefaultLanguage = LanguageCode.Russian;
        }

        public static void Add(LanguageCode language, string phraseCode, string phraseContent) => LanguagePhrases[language].Add(phraseCode, phraseContent);

        public static string GetPhrase(string phraseCode, LanguageCode language)
        {
            if (!LanguagePhrases[language].TryGetValue(phraseCode, out var result) && !LanguagePhrases[DefaultLanguage].TryGetValue(phraseCode, out result))
                throw new PhraseNotFoundException(phraseCode, language);

            return result;
        }
    }
}
