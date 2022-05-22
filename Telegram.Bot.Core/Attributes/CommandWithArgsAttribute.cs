using System;

namespace Telegram.Bot.Core.Attributes
{
    /// <summary>
    /// Обозначает, может ли команда принимать аргументы (<see cref="Command.Args"/>). Если да, используйте вместе с <see cref="CommandNameStartsWithAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandWithArgsAttribute : Attribute
    {
        /// <summary>
        /// Передавать ли аргументы в команду
        /// </summary>
        public bool WithArgs { get; }

        /// <summary>
        /// Разделитель аргументов
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// Создание атрибута
        /// </summary>
        /// <param name="withArgs">Принимает ли команда аргументы</param>
        /// <param name="separator">Разделитель аргументов</param>
        public CommandWithArgsAttribute(bool withArgs, string separator = " ")
        {
            WithArgs = withArgs;
            Separator = separator;
        }
    }
}
