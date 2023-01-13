using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Core.Access;
using Telegram.Bot.Core.Languages;
using Telegram.Bot.Core.Utilities;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Core.Example
{
    class Bot : IUpdateHandler
    {
        private TelegramBotClient _bot;

        private AccessCommandHandler<Info> _commandHandler;

        public Bot()
        {
            PhrasesDictionary.Add(LanguageCode.Russian, "start", "здарова");
            PhrasesDictionary.Add(LanguageCode.English, "start", "hello");

            Logger.Current = new Logger("log.txt", "errors.txt");
            UsersBase<Info>.Current = System.IO.File.Exists("users.json") ? UsersBase<Info>.LoadFromJson(System.IO.File.ReadAllText("users.json")) : new UsersBase<Info>();
            UsersBase<Info>.Current.StartAutoSave("users.json", TimeSpan.FromSeconds(20), new CancellationToken());
            
            _bot = new TelegramBotClient("5206812391:AAG_y1tUNGtbgOMwHmOyyvOsiHnD8uVrzZ0");
           
            _commandHandler = new AccessCommandHandler<Info>(UsersBase<Info>.Current) { UnknownCommandResponse = "Неизвестная команда", NotEnoughtPermissionsResponse = "Недостаточно прав" };
            
            _commandHandler.UnhandledException += (s, e) => Logger.Current.LogException(e.Exception);

            _commandHandler.NewMessage += (s, e) => Logger.Current.LogMessage(e.Message);

            _commandHandler.AddCommand<StartCommand>();
            _commandHandler.AddCommand<TestCommand>();
            _commandHandler.AddCallbackCommand<TestCallbackCommand>();
        }

        public void Start(CancellationToken token)
        {
            _bot.StartReceiving(this, cancellationToken: token);
        }

        public UpdateType[] AllowedUpdates => new[] { UpdateType.Message, UpdateType.CallbackQuery };

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is Message message)
            {
                await _commandHandler.HandleAsync(_bot, message);
            }
            
            if (update.CallbackQuery is CallbackQuery callback)
            {
                await _commandHandler.HandleCallbackAsync(_bot, callback);
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
