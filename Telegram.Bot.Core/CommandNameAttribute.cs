using System;

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
        public string Name { get; set; }

        /// <summary>
        /// Создание атрибута имени команды.
        /// </summary>
        public CommandNameAttribute()
        {
            Name = null;
        }

        /// <summary>
        /// Создание атрибута имени команды.
        /// </summary>
        /// <param name="name">Имя команды</param>
        public CommandNameAttribute(string name) : this()
        {
            Name = name;
        }
    }
}
