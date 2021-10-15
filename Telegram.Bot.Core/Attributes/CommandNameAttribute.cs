using System;
using Telegram.Bot.Types;

namespace Telegram.Bot.Core
{
    /// <summary>
    /// Атрибут имени команды
    /// </summary>
    public class CommandNameAttribute : Attribute
    {
        /// <summary>
        /// Имя команды. <para/>
        /// Например, чтобы бот откликался на команду "/start", сюда надо ввести "/start" (логично, не правда ли?)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Создание атрибута имени команды.
        /// </summary>
        /// <param name="name">Имя команды</param>
        public CommandNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Соответствует ли имя команды введеному пользователю тексту
        /// </summary>
        /// <param name="text">Текст пользователя или <see cref="CallbackQuery.Data"/></param>
        /// <returns><see langword="true"/>, если имя команды соответствует введеному пользователю тексту, иначе - <see langword="false"/></returns>
        public virtual bool Compare(string text) => Name == text;
    }
}
