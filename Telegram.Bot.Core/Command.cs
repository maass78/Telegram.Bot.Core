using System;
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
            try
            {
                await parts[partIndex++](context);

                if (partIndex >= parts.Length)
                    IsCompleted = true;
            }
            catch (Exception)
            {

            }
        }

        protected async Task Respond(CommandContext context, string message, IReplyMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Html)
        {
            try
            {
                await context.BotClient.SendTextMessageAsync(context.Message.Chat.Id, message, parseMode, replyMarkup: replyMarkup);
            }
            catch (Exception) { }
        }
    }

    public delegate Task CommandPartAsyncAction(CommandContext context);


}
