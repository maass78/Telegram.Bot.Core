using Telegram.Bot.Types;

namespace Telegram.Bot.Core
{
    public class CommandContext : BaseCommandContext
    {
        public Message Message { get; }

        public CommandContext(Message message, TelegramBotClient bot, CommandHandler handler) : base(message.Chat, message.From, bot, handler)
        {
            Message = message;
        }
    }
}
