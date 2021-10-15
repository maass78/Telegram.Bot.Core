using System.Threading.Tasks;
using Telegram.Bot.Core.Attributes;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
        protected async Task Respond(BaseCommandContext context, string message, IReplyMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Html, bool disableWebPreview = true)
        {
            await context.BotClient.SendTextMessageAsync(context.Chat, message, parseMode, replyMarkup: replyMarkup, disableWebPagePreview: disableWebPreview);
        }

        /// <summary>
        /// Изменяет сообщение
        /// </summary>
        /// <param name="context">Контекст команды</param>
        /// <param name="toEdit">Сообщение для изменения</param>
        /// <param name="message">Текст, на который необходимо заменить</param>
        /// <param name="replyMarkup">Клавиатура (только Inline)</param>
        /// <param name="parseMode">Разметка</param>
        /// <param name="disableWebPreview">Отключать ли превью у ссылок</param>
        protected async Task Edit(BaseCommandContext context, Message toEdit, string message, InlineKeyboardMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Html, bool disableWebPreview = true)
        {
            await context.BotClient.EditMessageTextAsync(toEdit.Chat, toEdit.MessageId, message, parseMode: parseMode, disableWebPagePreview: disableWebPreview, replyMarkup: replyMarkup);
        }
    }

    public delegate Task CommandPartAsyncAction(CommandContext context);
}
