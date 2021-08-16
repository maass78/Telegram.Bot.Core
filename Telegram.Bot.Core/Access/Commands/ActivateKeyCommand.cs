using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Core.Access.Commands
{
    public abstract class ActivateKeyCommand<T> : Command
    {
        /// <summary>
        /// "Введите ключ". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string enterKeyResponse { get; }
        /// <summary>
        /// "Неверный ключ активации". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string invalidKeyResponse { get; }
        /// <summary>
        /// "Ключ активирован". Поддерживается <see cref="ParseMode.Html"/>
        /// </summary>
        protected abstract string keyActivatedResponse { get; }

        /// <summary>
        /// Клавиатура, показывающаяся пользователю после выполнения команды. Чтобы ничего не отправлять, укажите <see langword="null"/>
        /// </summary>
        protected abstract IReplyMarkup replyMarkup { get; }

        protected override CommandPartAsyncAction[] parts => new CommandPartAsyncAction[]
        {
            EnterKey, Activate
        };

        private async Task EnterKey(CommandContext context)
        {
            await Respond(context, enterKeyResponse);
        }

        private async Task Activate(CommandContext context)
        {
            string key = context.Message.Text;

            AccessKey accessKey = UsersBase<T>.Current.Keys.FirstOrDefault(x => x.Key == key && x.AttachedUserId == 0);

            if (accessKey == null)
            {
                await Respond(context, invalidKeyResponse, replyMarkup);
                IsCompleted = true;
                return;
            }

            accessKey.StartTime = DateTime.UtcNow;
            accessKey.AttachedUserId = context.Message.From.Id;

            await Respond(context, keyActivatedResponse, replyMarkup);
            IsCompleted = true;
        }
    }
}
