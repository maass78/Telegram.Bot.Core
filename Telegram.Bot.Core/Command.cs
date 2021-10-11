using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Core
{
    public abstract class Command
    {
        public bool IsCompleted { get; protected set; }

        public bool IsSuccess { get; protected set; }

        public object Tag { get; protected set; }

        protected abstract CommandPartAsyncAction[] parts { get; }
        protected int partIndex = 0;

        public async Task ExecutePartAsync(CommandContext context)
        {
            if (parts.Length < 1)
            {
                IsCompleted = true;
                return;
            }

            await parts[partIndex++](context);

            if (partIndex >= parts.Length)
                IsCompleted = true;
        }

        protected async Task Respond(BaseCommandContext context, string message, IReplyMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Html, bool disableWebPreview = true)
        {
            await context.BotClient.SendTextMessageAsync(context.Chat, message, parseMode, replyMarkup: replyMarkup, disableWebPagePreview: disableWebPreview);
        }
    }

    public delegate Task CommandPartAsyncAction(CommandContext context);
}
