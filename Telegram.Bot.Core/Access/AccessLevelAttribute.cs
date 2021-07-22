using System;

namespace Telegram.Bot.Core.Access
{
    /// <summary>
    /// Атрибут уровня доступа к команде
    /// </summary>
    public class AccessLevelAttribute : Attribute
    {
        /// <summary>
        /// Уровень доступа. Уровень доступа выше позволяет выполнять любые команды с более низким уровнем
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Создание атрибута уровня доступа к команде
        /// </summary>
        /// <param name="level">Уровень доступа к команде</param>
        public AccessLevelAttribute(int level)
        {
            Level = level;
        }
    }
}
