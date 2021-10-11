using Telegram.Bot.Types;

namespace Telegram.Bot.Core.Callback
{
    public class CallbackCommandContext : BaseCommandContext
    {
        public CallbackQuery CallbackQuery { get; }

        public CallbackCommandContext(CallbackQuery callback, TelegramBotClient bot) : base(callback.Message.Chat, callback.From, bot)
        {
            CallbackQuery = callback;
        }
    }
}
