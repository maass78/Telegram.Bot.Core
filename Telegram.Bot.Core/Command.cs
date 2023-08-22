using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Core.Attributes;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Core.Languages;
using Telegram.Bot.Core.Languages.Exceptions;
using System.Linq;

namespace Telegram.Bot.Core
{
    /// <summary>
    /// Базовый класс для всех команд
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Статический экземпляр <see cref="ILanguageSelector"/> для удобного использования
        /// </summary>
        public static ILanguageSelector LanguageSelector { get; set; } = new LanguageSelector();

        /// <summary>
        /// Массив аргументов команды. Равен <see langword="null"/>, если у команды отсутствует атрибут <see cref="CommandWithArgsAttribute"/> или в него передано значение <see langword="false"/>
        /// </summary>
        public string[] Args { get; set; }

        /// <summary>
        /// Завершена ли команда
        /// </summary>
        public bool IsCompleted { get; protected set; }

        /// <summary>
        /// Успешно ли завершена команда
        /// </summary>
        public bool IsSuccess { get; protected set; }

        /// <summary>
        /// Вспомогательный объект. Суйте сюда что угодно
        /// </summary>
        public object Tag { get; protected set; }

        /// <summary>
        /// Массив частей команды. При новом сообщении вызывается следующая часть.
        /// </summary>
        protected abstract CommandPartAsyncAction[] parts { get; }

        /// <summary>
        /// Текущий индекс выполнения команды
        /// </summary>
        protected int partIndex = 0;

        /// <summary>
        /// Выполнить часть команды
        /// </summary>
        /// <param name="context">Контекст команды</param>
        public async Task ExecutePartAsync(CommandContext context)
        {
            if (parts.Length == 0)
            {
                IsCompleted = true;
                return;
            }

            await parts[partIndex++](context);

            if (partIndex >= parts.Length)
                IsCompleted = true;
        }

        /// <summary>
        /// Ответить пользователю
        /// </summary>
        /// <param name="context">Контекст команды</param>
        /// <param name="message">Сообщение</param>
        /// <param name="replyMarkup">Клавиатура</param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        /// <returns>Отправленное сообщение</returns>
        protected async Task<Message> Respond(BaseCommandContext context, string message, IReplyMarkup replyMarkup = null, ParseMode? parseMode = ParseMode.Html, bool disableWebPreview = true)
        {
            return await context.BotClient.SendTextMessageAsync(context.Chat, message, parseMode: parseMode, replyMarkup: replyMarkup, disableWebPagePreview: disableWebPreview);
        }

        /// <summary>
        /// Ответить пользователю, используя <see cref="PhrasesDictionary"/>
        /// </summary>
        /// <remarks>
        /// Метод берет язык пользователя c помощью <see cref="LanguageSelector"/> (он не должен быть равен <see langword="null"/>) и выбирает фразу из <see cref="PhrasesDictionary"/>.
        /// Если фраза не найдена, выбрасывает <see cref="PhraseNotFoundException"/>
        /// </remarks>
        /// <param name="context">Контекст команды</param>
        /// <param name="message">Код фразы</param>
        /// <param name="replyMarkup">Клавиатура</param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        /// <returns>Отправленное сообщение</returns>
        /// <exception cref="PhraseNotFoundException"/>
        protected async Task<Message> RespondPhrase(BaseCommandContext context, string phraseCode, IReplyMarkup replyMarkup = null, ParseMode? parseMode = ParseMode.Html, bool disableWebPreview = true)
            => await Respond(context, PhrasesDictionary.GetPhrase(phraseCode, context.From.GetUserLanguage(LanguageSelector)), replyMarkup, parseMode, disableWebPreview);

        /// <summary>
        /// Ответить пользователю, используя <see cref="PhrasesDictionary"/> с использованием языка, указанного в аргументы
        /// </summary>
        /// <remarks>
        /// Метод берет язык пользователя c помощью <see cref="LanguageSelector"/> (он не должен быть равен <see langword="null"/>) и выбирает фразу из <see cref="PhrasesDictionary"/>.
        /// Если фраза не найдена, выбрасывает <see cref="PhraseNotFoundException"/>
        /// </remarks>
        /// <param name="context">Контекст команды</param>
        /// <param name="message">Код фразы</param>
        /// <param name="replyMarkup">Клавиатура</param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        /// <returns>Отправленное сообщение</returns>
        /// <exception cref="PhraseNotFoundException"/>
        protected async Task<Message> RespondPhrase(BaseCommandContext context, string phraseCode, LanguageCode language, IReplyMarkup replyMarkup = null, ParseMode? parseMode = ParseMode.Html, bool disableWebPreview = true)
            => await Respond(context, PhrasesDictionary.GetPhrase(phraseCode, language), replyMarkup, parseMode, disableWebPreview);


        /// <summary>
        /// Ответить пользователю, используя <see cref="PhrasesDictionary"/> и <see cref="string.Format(string, object[])"/>
        /// </summary>
        /// <remarks>
        /// Метод берет язык пользователя c помощью <see cref="LanguageSelector"/> (он не должен быть равен <see langword="null"/>) и выбирает фразу из <see cref="PhrasesDictionary"/>.
        /// Если фраза не найдена, выбрасывает <see cref="PhraseNotFoundException"/>
        /// </remarks>
        /// <param name="context">Контекст команды</param>
        /// <param name="message">Код фразы</param>
        /// <param name="replyMarkup">Клавиатура</param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        /// <returns>Отправленное сообщение</returns>
        /// <exception cref="PhraseNotFoundException"/>
        protected async Task<Message> RespondFormatPhrase(BaseCommandContext context, string phraseCode, IReplyMarkup replyMarkup = null, ParseMode? parseMode = ParseMode.Html, bool disableWebPreview = true, params object[] format)
            => await Respond(context, string.Format(PhrasesDictionary.GetPhrase(phraseCode, context.From.GetUserLanguage(LanguageSelector)), format), replyMarkup, parseMode, disableWebPreview);

        /// <summary>
        /// Ответить пользователю, используя <see cref="PhrasesDictionary"/> и <see cref="string.Format(string, object[])"/>, с использованием языка, указанного в аргументы
        /// </summary>
        /// <remarks>
        /// Метод берет язык пользователя c помощью <see cref="LanguageSelector"/> (он не должен быть равен <see langword="null"/>) и выбирает фразу из <see cref="PhrasesDictionary"/>.
        /// Если фраза не найдена, выбрасывает <see cref="PhraseNotFoundException"/>
        /// </remarks>
        /// <param name="context">Контекст команды</param>
        /// <param name="message">Код фразы</param>
        /// <param name="replyMarkup">Клавиатура</param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        /// <returns>Отправленное сообщение</returns>
        /// <exception cref="PhraseNotFoundException"/>
        protected async Task<Message> RespondFormatPhrase(BaseCommandContext context, string phraseCode, LanguageCode languageCode, IReplyMarkup replyMarkup = null, ParseMode? parseMode = ParseMode.Html, bool disableWebPreview = true, params object[] format)
            => await Respond(context, string.Format(PhrasesDictionary.GetPhrase(phraseCode, languageCode), format), replyMarkup, parseMode, disableWebPreview);

        /// <summary>
        /// Изменяет сообщение
        /// </summary>
        /// <remarks>
        /// Если исходное сообщение точно такое же, на которое хотите изменить, с проверкой <paramref name="checkForEquals"/> не будет изменять сообщение и вернет исходное. Если проверка отключена, вылетит <see cref="ApiRequestException"/>
        /// </remarks>
        /// <param name="context">Контекст команды</param>
        /// <param name="messageToEdit">ID сообщения для изменения</param>
        /// <param name="message">Текст, на который необходимо изменить</param>
        /// <param name="replyMarkup">Клавиатура (только Inline)</param>
        /// <param name="checkForEquals">Проверять, равно ли </param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        /// <returns>Изменённое сообщение</returns>
        protected async Task<Message> Edit(BaseCommandContext context, Message messageToEdit, string message, InlineKeyboardMarkup replyMarkup = null, bool checkForEquals = true, ParseMode? parseMode = ParseMode.Html, bool disableWebPreview = true)
        {
            if (checkForEquals)
            {
                try
                {
                    if (Regex.Replace(message, "<.*?>", string.Empty) == messageToEdit.Text && IsKeyboardsEquals(replyMarkup, messageToEdit.ReplyMarkup))
                        return messageToEdit;
                }
                catch { }
            }

            return await context.BotClient.EditMessageTextAsync(messageToEdit.Chat, messageToEdit.MessageId, message, parseMode: parseMode, disableWebPagePreview: disableWebPreview, replyMarkup: replyMarkup);
        }

        private bool IsKeyboardsEquals(InlineKeyboardMarkup keyboard1, InlineKeyboardMarkup keyboard2)
        {
            if (keyboard1 != null && keyboard2 != null)
            {
                var buttons1 = keyboard1.InlineKeyboard.Select(x => x.ToList()).ToList();
                var buttons2 = keyboard2.InlineKeyboard.Select(x => x.ToList()).ToList();

                if (buttons1.Count != buttons2.Count)
                    return false;

                for (int i = 0; i < buttons1.Count; i++)
                {
                    if (buttons1[i].Count != buttons2[i].Count)
                        return false;

                    for (int j = 0; j < buttons1[i].Count; j++)
                    {
                        if (buttons1[i][j].Text != buttons2[i][j].Text)
                            return false;

                        if (buttons1[i][j].CallbackData != buttons2[i][j].CallbackData)
                            return false;

                        if (buttons1[i][j].Url != buttons2[i][j].Url)
                            return false;
                    }
                }
            }
            else if (keyboard1 == null != (keyboard2 == null))
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Изменяет сообщение
        /// </summary>
        /// <remarks>
        /// Если исходное сообщение точно такое же, на которое хотите изменить, вылетит <see cref="ApiRequestException"/>
        /// </remarks>
        /// <param name="context">Контекст команды</param>
        /// <param name="messageIdToEdit">ID сообщения для изменения</param>
        /// <param name="message">Текст, на который необходимо изменить</param>
        /// <param name="replyMarkup">Клавиатура (только Inline)</param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        /// <returns>Изменённое сообщение</returns>
        protected async Task<Message> Edit(BaseCommandContext context, int messageIdToEdit, string message, InlineKeyboardMarkup replyMarkup = null, ParseMode? parseMode = ParseMode.Html, bool disableWebPreview = true)
        {
            return await context.BotClient.EditMessageTextAsync(context.Chat, messageIdToEdit, message, parseMode: parseMode, disableWebPagePreview: disableWebPreview, replyMarkup: replyMarkup);
        }

        /// <summary>
        /// Удаляет сообщение
        /// </summary>
        /// <param name="context">Контекст команды</param>
        /// <param name="messageId">ID сообщения, которое необходимо удалить (свойство <see cref="Message.MessageId"/>)</param>
        protected async Task Delete(BaseCommandContext context, int messageId)
        {
            await context.BotClient.DeleteMessageAsync(context.Chat, messageId);
        }

        /// <summary>
        /// Удаляет сообщение
        /// </summary>
        /// <param name="context">Контекст команды</param>
        /// <param name="toDelete">Сообщение, которое необходимо удалить</param>
        protected async Task Delete(BaseCommandContext context, Message toDelete) => await Delete(context, toDelete.MessageId);
    }

    public delegate Task CommandPartAsyncAction(CommandContext context);
}
