using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Core.Attributes;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;

namespace Telegram.Bot.Core
{
    /// <summary>
    /// Базовый класс для всех команд
    /// </summary>
    public abstract class Command
    {
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
            return await context.BotClient.SendTextMessageAsync(context.Chat, message, parseMode, replyMarkup: replyMarkup, disableWebPagePreview: disableWebPreview);
        }

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
                    if (Regex.Replace(message, "<.*?>", string.Empty) == messageToEdit.Text)
                        return messageToEdit;
                }
                catch { }
            }

            return await context.BotClient.EditMessageTextAsync(messageToEdit.Chat, messageToEdit.MessageId, message, parseMode: parseMode, disableWebPagePreview: disableWebPreview, replyMarkup: replyMarkup);
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
