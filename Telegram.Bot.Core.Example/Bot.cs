using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Core.Access;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Core.Example
{
    class Bot : IUpdateHandler, ILogger
    {
        private TelegramBotClient _bot;

        private AccessCommandHandler _commandHandler;

        public Bot()
        {
            UsersBase.Current = System.IO.File.Exists("users.json") ? UsersBase.LoadFromJson(System.IO.File.ReadAllText("users.json")) : new UsersBase();

            _bot = new TelegramBotClient("1918792358:AAF305hQFag9kB6gP1rjuHqO-e-YDtrZQZY");
            _bot.StartReceiving(this);
            _commandHandler = new AccessCommandHandler(UsersBase.Current, true) { UnknownCommandResponse = "Неизвестная команда", NotEnoughtPermissionsResponse = "Недостаточно прав", Logger = this };
        }

        public UpdateType[] AllowedUpdates => new[] { UpdateType.Message };

        public async Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is Message message)
            {
                await _commandHandler.HandleAsync(_bot, message);
            }
        }

        public void LogError(string message)
        {
            Console.WriteLine(message);
        }

        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            Console.WriteLine(message);
        }
    }
}
