using System;
using Telegram.Bot.Core.Languages;

namespace Telegram.Bot.Core.Example
{
    class Program
    {
        static void Main()
        {
            Bot bot = new Bot();
            bot.Start(default);

            Console.ReadLine();
        }
    }
}
