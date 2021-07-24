using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool SpamProtectEnabled { get; set; }

        public SpamProtectSettings SpamProtectSettings { get; }

        private Dictionary<long, List<DateTime>> _userTimeMessages;

        private List<UserBlocking> _usersBlockings;

        /// <summary>
        /// Конструктор класса <see cref="AccessCommandHandler"/>
        /// </summary>
        /// <param name="usersBase">База пользователей, созданная пустой или загруженная из файла</param>
        /// <param name="spamProtectEnabled"><see langword="true"/>, если хотите включить защиту от спама. Защита настраивается свойством <see cref="SpamProtectSettings"/></param>
        public AccessCommandHandler(UsersBase usersBase, bool spamProtectEnabled = false) : base()
        {
            Users = usersBase;
            _userTimeMessages = new Dictionary<long, List<DateTime>>();
            _usersBlockings = new List<UserBlocking>();
            SpamProtectSettings = new SpamProtectSettings()
            {
                BlockingDuration = TimeSpan.FromMinutes(10),
                DetectingDuration = TimeSpan.FromSeconds(2),
                MaxMessagesCount = 5
            };
        }

        /// <summary>
        /// Ответ бота, если пользователь ввел неизвестную команду. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string UnknownCommandResponse { get; set; } = "Unknown command";

        /// <summary>
        /// Ответ бота, если пользователь не имеет достаточных прав для выполнения команды. Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        public string NotEnoughtPermissionsResponse { get; set; } = "Not enought permissions";

        private bool CheckForSpam(long userId)
        {
            if (_userTimeMessages.TryGetValue(userId, out List<DateTime> times))
            {
                if (times == null)
                {
                    _userTimeMessages[userId] = new List<DateTime>() { DateTime.UtcNow };
                    return true;
                }

                _userTimeMessages[userId].Add(DateTime.UtcNow);
                
                if (times.Count < SpamProtectSettings.MaxMessagesCount)
                {
                    return true;
                }

                return (_userTimeMessages[userId][times.Count - 1] - _userTimeMessages[userId][times.Count - SpamProtectSettings.MaxMessagesCount]) > SpamProtectSettings.DetectingDuration;
            }
            else
            {
                _userTimeMessages[userId] = new List<DateTime>() { DateTime.UtcNow };
                return true;
            }
        }

        protected override bool IsUserBlocked(long userId)
        {
            bool blocked = false;

            var user = _usersBlockings.LastOrDefault(x => x.UserId == userId);

            if (user != null)
                blocked = user.BlockStartTime + user.BlockDuration >= DateTime.UtcNow;

            if (blocked)
                return true;

            if (!CheckForSpam(userId))
            {
                _usersBlockings.Add(new UserBlocking() { UserId = userId, BlockStartTime = DateTime.UtcNow, BlockDuration = SpamProtectSettings.BlockingDuration });
                blocked = true;
            }

            return blocked;
        }

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
