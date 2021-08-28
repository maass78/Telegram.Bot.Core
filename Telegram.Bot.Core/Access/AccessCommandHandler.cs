using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Core.Access
{
    /// <summary>
    /// Стандартная реализация <see cref="CommandHandler"/> 
    /// с поддержкой уровней доступа к командам
    /// </summary>
    public class AccessCommandHandler<T> : CommandHandler
    {
        /// <summary>
        /// База пользователей
        /// </summary>
        public UsersBase<T> Users { get; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="usersBase">База пользователей, созданная пустой или загруженная из файла</param>
        public AccessCommandHandler(UsersBase<T> usersBase) : base()
        {
            Users = usersBase;
        }

        /// <summary>
        /// Клавиатура, отправляемая пользователю в случае, если тот ввел неизвестную команду или не имеет прав на выполнение команды
        /// </summary>
        public IReplyMarkup Keyboard { get; set; }

        /// <summary>
        /// Ответ бота, если пользователь ввел неизвестную команду. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string UnknownCommandResponse { get; set; } = "Unknown command";

        /// <summary>
        /// Ответ бота, если пользователь не имеет достаточных прав для выполнения команды. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string NotEnoughtPermissionsResponse { get; set; } = "Not enought permissions";

        protected override bool IsUserBlocked(CommandContext commandContext)
        {
            return Users.IsUserBlocked(commandContext);
        }

        protected override bool CanExecute(CommandContext context, Command command)
        {
            int requestedLevel = -1;

            foreach (Attribute attribute in command.GetType().GetCustomAttributes(true))
            {
                if (attribute is AccessLevelAttribute accessLevelAttribute)
                {
                    requestedLevel = accessLevelAttribute.Level;
                }
            }

            return Users.CanUseCommand(context, requestedLevel);
        }

        protected override async void OnCannotExecute(CommandContext context)
        {
            try
            {
                await context.BotClient.SendTextMessageAsync(context.Message.Chat.Id, NotEnoughtPermissionsResponse, ParseMode.Html, replyMarkup: Keyboard);
            }
            catch { }
        }

        protected override async void OnUnknownCommand(TelegramBotClient client, Message message)
        {
            try
            {
                await client.SendTextMessageAsync(message.Chat.Id, UnknownCommandResponse, ParseMode.Html, replyMarkup: Keyboard);
            }
            catch { }
        }
    }
}
