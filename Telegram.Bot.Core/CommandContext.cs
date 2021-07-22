using Telegram.Bot.Types;

namespace Telegram.Bot.Core
{
    public class CommandContext
    {
        public Message Message { get; }
        public TelegramBotClient BotClient { get; }

        public CommandContext(Message message, TelegramBotClient bot)
        {
            Message = message;
            BotClient = bot;
        }
    }
}
