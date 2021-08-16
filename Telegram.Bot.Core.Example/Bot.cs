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

        private AccessCommandHandler<Info> _commandHandler;

        public Bot()
        {
            UsersBase<Info>.Current = System.IO.File.Exists("users.json") ? UsersBase<Info>.LoadFromJson(System.IO.File.ReadAllText("users.json")) : new UsersBase<Info>();
            UsersBase<Info>.Current.StartAutoSave("users.json", TimeSpan.FromSeconds(20), new CancellationToken());
            
            _bot = new TelegramBotClient("1918792358:AAF305hQFag9kB6gP1rjuHqO-e-YDtrZQZY");
            _bot.StartReceiving(this);
            _commandHandler = new AccessCommandHandler<Info>(UsersBase<Info>.Current) { UnknownCommandResponse = "Неизвестная команда", NotEnoughtPermissionsResponse = "Недостаточно прав" };
            
            _commandHandler.UnhandledException += (s, e) =>
            {
                LogError($"{e.Exception.GetType()}: {e.Exception.Message}");
            };

            _commandHandler.NewMessage += (s, e) =>
            {
                LogInfo($"[{e.Message.From.Id} {e.Message.From.Username}]: {e.Message.Text ?? "null"}");
            };
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now}] [ERROR] {message}");
        }

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{DateTime.Now}] [INFO] {message}");
        }

        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now}] [WARNING] {message}");
        }
    }
}
