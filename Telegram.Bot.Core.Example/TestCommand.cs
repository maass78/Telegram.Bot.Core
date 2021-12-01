using System.Threading.Tasks;

namespace Telegram.Bot.Core.Example
{
    [CommandName("1")]
    internal class TestCommand : Command
    {
        protected override CommandPartAsyncAction[] parts => new CommandPartAsyncAction[] 
        {
            First,
            Second
        };

        private async Task First(CommandContext context)
        {
            await Respond(context, "message" + context.Message.Text ?? "null");
        }

        private async Task Second(CommandContext context)
        {
            await Task.Delay(10000);
            await Respond(context, "message" + context.Message.Text ?? "null");
        }
    }
}
