using System.Threading.Tasks;

namespace Telegram.Bot.Core.Callback
{
    public abstract class CallbackCommand : Command
    {
        public abstract Task Execute(CallbackCommandContext context);

        public bool OnlyResponse => parts == null || parts.Length == 0;

        protected async Task AnswerCallback(CallbackCommandContext context, string text = null, bool showAlert = false, string url = null, int cacheTime = 0)
        {
            await context.BotClient.AnswerCallbackQueryAsync(context.CallbackQuery.Id, text, showAlert, url, cacheTime);
        }
    }
}
