using System.Threading.Tasks;
using Telegram.Bot.Core.Access;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Core.Example
{
    [CommandName("/start")]
    [AccessLevel(-1)]
    class StartCommand : Command
    {
        protected override CommandPartAsyncAction[] parts => new CommandPartAsyncAction[]
        {
            First
        };

        private async Task First(CommandContext context)
        {
            await RespondPhrase(context, "start", new InlineKeyboardMarkup(new InlineKeyboardButton("test") { CallbackData = "test" }));
        }
    }
}
