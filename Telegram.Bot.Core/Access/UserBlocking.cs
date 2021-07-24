using System;

namespace Telegram.Bot.Core.Access
{
    public class UserBlocking
    {
        public long UserId { get; set; }
        public DateTime BlockStartTime { get; set; }
        public TimeSpan BlockDuration { get; set; }
    }
}
