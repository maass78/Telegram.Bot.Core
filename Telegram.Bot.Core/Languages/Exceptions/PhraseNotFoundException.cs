using System;

namespace Telegram.Bot.Core.Languages.Exceptions
{
    internal class PhraseNotFoundException : Exception
    {
        public PhraseNotFoundException(string codePhrase, LanguageCode languageCode) 
            : base($"Coudn't find phrase '{codePhrase}' in language '{languageCode}'") { }
    }
}
