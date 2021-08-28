using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Core.Callback
{
    public abstract class CallbackCommand
    {
        public abstract Task Execute(CallbackCommandContext commandContext);

        protected async Task Respond(CommandContext context, string message, IReplyMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Html, bool disableWebPreview = true)
        {
            await context.BotClient.SendTextMessageAsync(context.Message.Chat, message, parseMode, replyMarkup: replyMarkup, disableWebPagePreview: disableWebPreview);
        }

        protected async Task AnswerCallback(CallbackCommandContext context, string text = null, bool showAlert = false, string url = null, int cacheTime = 0)
        {
            await context.BotClient.AnswerCallbackQueryAsync(context.CallbackQuery.Id, text, showAlert, url, cacheTime);
        }
    }
}
