using System;

namespace Telegram.Bot.Core.Callback
{
    public class CallbackCommandNameAttribute : Attribute
    {
        public string Name { get; }

        public CallbackCommandNameAttribute() { }
      
        public CallbackCommandNameAttribute(string name)
        {
            Name = name;
        }
    }
}
