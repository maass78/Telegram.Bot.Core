using System;
using System.Collections.Generic;

namespace Telegram.Bot.Core.Languages
{
    public static class PhrasesDictionary
    {
        public static readonly Dictionary<LanguageCode, Dictionary<string, string>> LanguagePhrases = new Dictionary<LanguageCode, Dictionary<string, string>>()
        {
           
        };

        static PhrasesDictionary()
        {
            LanguagePhrases = new Dictionary<LanguageCode, Dictionary<string, string>>();

            foreach (var item in Enum.GetValues(typeof()))
            {

            }

        }

        public static string GetPhrase(string phraseCode, LanguageCode language)
        {

        }
    }
}
