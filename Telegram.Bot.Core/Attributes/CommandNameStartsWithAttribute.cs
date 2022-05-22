using System;

namespace Telegram.Bot.Core.Attributes
{
    /// <summary>
    /// Атрибут, означающий, что имя команды должно начинаться с указанного текста
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandNameStartsWithAttribute : CommandNameAttribute
    {
        /// <summary>
        /// Создание атрибута
        /// </summary>
        /// <param name="name">Текст, с которого должно начинаться имя команды</param>
        public CommandNameStartsWithAttribute(string name) : base(name) { }

        public override bool Compare(string text, BaseCommandContext context) => text.StartsWith(Name);
    }
}
