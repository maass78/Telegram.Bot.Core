using Telegram.Bot.Types;

namespace Telegram.Bot.Core.Callback
{
    public class CallbackCommandContext : CommandContext
    {
        public CallbackQuery CallbackQuery { get; }

        public CallbackCommandContext(CallbackQuery callbackQuery, TelegramBotClient bot) : base(callbackQuery.Message, bot)
        {
            CallbackQuery = callbackQuery;
        }
    }
}
