using System;

namespace Telegram.Bot.Core.Access
{
    /// <summary>
    /// Настройки защиты от спама
    /// </summary>
    public class SpamProtectSettings
    {
        /// <summary>
        /// Время, на которое блокируется пользователь, если замечен спам с его стороны
        /// </summary>
        public TimeSpan BlockingDuration { get; set; }

        /// <summary>
        /// Количество сообщений за промежуток времени <see cref="DetectingDuration"/>, при достижении которого пользователь блокируется
        /// </summary>
        public int MaxMessagesCount { get; set; }

        /// <summary>
        /// Если за этот промежуток времени достигнуто максимальное количество сообщений <see cref="MaxMessagesCount"/>, то пользователь блокируется
        /// </summary>
        public TimeSpan DetectingDuration { get; set; }
    }

}
