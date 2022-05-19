using Telegram.Bot.Types;

namespace Telegram.Bot.Core.Languages
{
    /// <summary>
    /// Класс-расширение для получения языка пользователя
    /// </summary>
    public static class LanguageExtensions
    {
        /// <summary>
        /// Получает язык пользователя
        /// </summary>
        /// <returns>Язык пользователя в виде <see cref="LanguageCode"/>. Если не определён, возвращает <see cref="LanguageCode.Undefined"/></returns>
        public static LanguageCode GetUserLanguage(this User user, ILanguageSelector languageSelector) => languageSelector.GetLanguageCode(user.LanguageCode);
    }
}
