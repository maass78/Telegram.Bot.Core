using System;

namespace Telegram.Bot.Core.Access
{
    /// <summary>
    /// Ключ доступа
    /// </summary>
    public class AccessKey
    {
        /// <summary>
        /// Сам ключ
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Идентификатор пользователя, владеющего данным ключом
        /// </summary>
        public long AttachedUserId { get; set; }
        /// <summary>
        /// Время жизни ключа. <see cref="TimeSpan.MaxValue"/>, чтобы установить неограниченное время
        /// </summary>
        public TimeSpan KeyDuration { get; set; }
        /// <summary>
        /// Время начала использования ключа
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Уровень доступа к командам
        /// </summary>
        public int AccessLevel { get; set; }
    }
}
