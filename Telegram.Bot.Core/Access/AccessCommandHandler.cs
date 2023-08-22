using System;
using Telegram.Bot.Core.Callback;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Core.Access
{
    /// <summary>
    /// Стандартная реализация <see cref="CommandHandler"/> 
    /// с поддержкой уровней доступа к командам
    /// </summary>
    public class AccessCommandHandler<T> : CommandHandler where T : UserInfo, new()
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
        /// Ответ бота, если пользователь кликнул на кнопку, для которой не определена команда. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string UnknownCallbackResponse { get; set; } = null;

        /// <summary>
        /// Ответ бота, если пользователь не имеет достаточных прав для выполнения команды. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string NotEnoughtPermissionsResponse { get; set; } = "Not enought permissions";

        protected override bool IsUserBlocked(BaseCommandContext context)
        {
            return Users.IsUserBlocked(context);
        }

        protected override bool CanExecute(BaseCommandContext context, Command command)
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

        protected override async void OnCannotExecuteCommand(CommandContext context)
        {
            try
            {
                if (NotEnoughtPermissionsResponse != null)
                    await context.BotClient.SendTextMessageAsync(context.Chat, NotEnoughtPermissionsResponse, parseMode: ParseMode.Html, replyMarkup: Keyboard);
            }
            catch { }
        }

        protected override async void OnCannotExecuteCallback(CallbackCommandContext context)
        {
            try
            {
                if (NotEnoughtPermissionsResponse != null)
                    await context.BotClient.SendTextMessageAsync(context.Chat, NotEnoughtPermissionsResponse, parseMode: ParseMode.Html, replyMarkup: Keyboard);

                await context.BotClient.AnswerCallbackQueryAsync(context.CallbackQuery.Id);
            }
            catch { }
        }

        protected override async void OnUnknownCommand(CommandContext context)
        {
            try
            {
                if (UnknownCommandResponse != null)
                    await context.BotClient.SendTextMessageAsync(context.Chat, UnknownCommandResponse, parseMode: ParseMode.Html, replyMarkup: Keyboard);
            }
            catch { }
        }

        protected override async void OnUnknownCallback(CallbackCommandContext context)
        {
            try
            {
                if (UnknownCallbackResponse != null)
                    await context.BotClient.SendTextMessageAsync(context.Chat, UnknownCallbackResponse, parseMode: ParseMode.Html, replyMarkup: Keyboard);
            }
            catch { }
        }
    }
}
