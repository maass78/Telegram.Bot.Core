using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Core.Access
{
    /// <summary>
    /// Стандартная реализация <see cref="CommandHandler"/> 
    /// с поддержкой уровней доступа к командам
    /// </summary>
    public class AccessCommandHandler : CommandHandler
    {
        /// <summary>
        /// База пользователей
        /// </summary>
        public UsersBase Users { get; }

        /// <summary>
        /// Конструктор класса <see cref="AccessCommandHandler"/>
        /// </summary>
        /// <param name="usersBase">База пользователей, созданная пустой или загруженная из файла</param>
        public AccessCommandHandler(UsersBase usersBase) : base()
        {
            Users = usersBase;
        }

        /// <summary>
        /// Ответ бота, если пользователь ввел неизвестную команду. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string UnknownCommandResponse { get; set; } = "Unknown command";

        /// <summary>
        /// Ответ бота, если пользователь не имеет достаточных прав для выполнения команды. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string NotEnoughtPermissionsResponse { get; set; } = "Not enought permissions";

        protected override bool CanExecute(long userId, Command command)
        {
            int requestedLevel = -1;

            foreach (Attribute attribute in command.GetType().GetCustomAttributes(true))
            {
                if (attribute is AccessLevelAttribute accessLevelAttribute)
                {
                    requestedLevel = accessLevelAttribute.Level;
                }
            }

            UserInfo settings = Users.GetById(userId);

            return Users.CanUseCommand(settings.Key, userId, requestedLevel);
        }

        protected override async void OnCannotExecute(CommandContext context)
        {
            try
            {
                await context.BotClient.SendTextMessageAsync(context.Message.Chat.Id, NotEnoughtPermissionsResponse, ParseMode.Html);
            }
            catch { }
        }

        protected override async void OnUnknownCommand(TelegramBotClient client, Message message)
        {
            try
            {
                await client.SendTextMessageAsync(message.Chat.Id, UnknownCommandResponse, ParseMode.Html);
            }
            catch { }
        }
    }
}
