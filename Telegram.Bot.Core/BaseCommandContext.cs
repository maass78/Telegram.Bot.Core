using Telegram.Bot.Types;

namespace Telegram.Bot.Core
{
    /// <summary>
    /// Класс, содержащий информацию о пользователе, который выполняет команду, и <see cref="TelegramBotClient"/>, выполняющий эту команду
    /// </summary>
    public class BaseCommandContext
    {
        public BaseCommandContext(Chat chat, User from, TelegramBotClient bot)
        {
            From = from;
            BotClient = bot;
            Chat = chat;
        }

        public Chat Chat { get; set; }

        public User From { get; set; }

        public TelegramBotClient BotClient { get; set; }
    }
}
