using System.Threading.Tasks;
using Telegram.Bot.Core.Access;
using Telegram.Bot.Core.Callback;

namespace Telegram.Bot.Core.Example
{
    [CommandName("test")]
    [AccessLevel(1)]
    class TestCallbackCommand : CallbackCommand
    {
        protected override CommandPartAsyncAction[] parts => new CommandPartAsyncAction[]
        {
            First
        };

        private async Task First(CommandContext context)
        {
            await Respond(context, context.Message.Text);
        }

        public async override Task Execute(CallbackCommandContext context)
        {
            await Respond(context, "inline response");
            await AnswerCallback(context);
        }
    }
}
