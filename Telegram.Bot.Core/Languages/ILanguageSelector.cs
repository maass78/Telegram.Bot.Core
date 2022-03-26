namespace Telegram.Bot.Core.Languages
{
    /// <summary>
    /// Интерфейс для класса, выбирающего язык
    /// </summary>
    public interface ILanguageSelector
    {
        /// <summary>
        /// Парсит код языка (при дефолтной реализации <see cref="LanguageSelector"/> .IETF Language Code) в <see cref="LanguageCode"/> 
        /// </summary>
        /// <param name="languageCode">Код в формате <code>en</code> или <code>en-US</code> (при дефолтной реализации <see cref="LanguageSelector"/>)</param>
        /// <returns><see cref="LanguageCode"/>, соответствующий <paramref name="languageCode"/>. В случае, если совпадение не найдено, возвращает <see cref="LanguageCode.Undefined"/></returns>
        LanguageCode GetLanguageCode(string languageCode);
    }
}
