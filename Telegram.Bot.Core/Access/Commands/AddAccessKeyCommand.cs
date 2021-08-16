using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Core.Access.Commands
{
    public abstract class AddAccessKeyCommand<T> : Command
    {
        /// <summary>
        /// Ключ - название длительности, значение - сама длительность
        /// </summary>
        protected abstract Dictionary<string, TimeSpan> keysDurations { get; }

        /// <summary>
        /// Ключ - название уровня доступа, значение - сама уровень
        /// </summary>
        protected abstract Dictionary<string, int> keysAccessLevels { get; }

        /// <summary>
        /// "Выберите длительность действия ключа". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string selectDurationResponse { get; }
        /// <summary>
        /// "Неверный период действия ключа". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string invalidDurationResponse { get; }
        /// <summary>
        /// "Выберите права доступа". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string selectAccessLevelResponse { get; }
        /// <summary>
        /// "Неверные права доступа". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string invalidAccessLevelResponse { get; }
        /// <summary>
        /// "Добавлен новый ключ: {0} | Срок действия: {1} | Права доступа: {2}". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string keyAddedResponse { get; }

        /// <summary>
        /// Клавиатура, показывающаяся пользователю после выполнения команды. Чтобы ничего не отправлять, укажите <see langword="null"/>
        /// </summary>
        protected abstract IReplyMarkup replyMarkup { get; }

        /// <summary>
        /// Длина генерируемого ключа
        /// </summary>
        protected abstract int keysLength { get; }

        private AccessKey _keyToAdd;
        private string _durationString;

        protected override CommandPartAsyncAction[] parts => new CommandPartAsyncAction[]
        {
            SelectDuration, SelectAccessLevel, End
        };

        private async Task SelectDuration(CommandContext context)
        {
            List<KeyboardButton> buttons = new List<KeyboardButton>();

            foreach (var duration in keysDurations)
            {
                buttons.Add(duration.Key);
            }

            await Respond(context, selectDurationResponse, new ReplyKeyboardMarkup(buttons, true, true));
        }

        private async Task SelectAccessLevel(CommandContext context)
        {
            string text = context.Message.Text;
          
            bool isDurationValid = keysDurations.TryGetValue(text, out TimeSpan duration);
            if(!isDurationValid)
            {
                await Respond(context, invalidDurationResponse, replyMarkup);
                IsCompleted = true;
                return;
            }

            _durationString = text;
            _keyToAdd = new AccessKey()
            {
                KeyDuration = duration,
                Key = RandomStringGenerator.AllSymbolsGenerator.Generate(keysLength)
            };

            List<KeyboardButton> buttons = new List<KeyboardButton>();
            foreach (var accessLevel in keysAccessLevels)
            {
                buttons.Add(accessLevel.Key);
            }

            await Respond(context, selectAccessLevelResponse, new ReplyKeyboardMarkup(buttons, true, true));
        }

        private async Task End(CommandContext context)
        {
            string text = context.Message.Text;

            bool isAccessLevelValid = keysAccessLevels.TryGetValue(text, out int accessLevel);

            if (!isAccessLevelValid)
            {
                await Respond(context, invalidAccessLevelResponse, replyMarkup);
                IsCompleted = true;
                return;
            }

            _keyToAdd.AccessLevel = accessLevel;

            UsersBase<T>.Current.Keys.Add(_keyToAdd);

            await Respond(context, string.Format(keyAddedResponse, _keyToAdd.Key,_durationString, text), replyMarkup);
        }
    }
}
