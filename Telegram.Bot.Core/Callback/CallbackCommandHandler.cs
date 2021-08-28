using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Core.Callback
{
    public class CallbackCommandHandler
    {
        public event UnhandledExceptionEventHandler UnhandledException;
        private List<Type> _commands;

        public CallbackCommandHandler()
        {
            _commands = new List<Type>();
        }

        public void AddCommand<T>() where T : CallbackCommand
        {
            _commands.Add(typeof(T));
        }

        public async Task HandleAsync(TelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            CallbackCommandContext commandContext = new CallbackCommandContext(callbackQuery, botClient);

            try
            {
                foreach (Type type in _commands)
                {
                    foreach (Attribute attribute in type.GetCustomAttributes(true))
                    {
                        if (attribute is CallbackCommandNameAttribute commandName)
                        {
                            if (commandName.Name == callbackQuery.Data)
                            {
                                CallbackCommand command = (CallbackCommand)Activator.CreateInstance(type);
                                await command.Execute(commandContext);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnUnhandledException(ex, commandContext);
            }
           
        }

        protected virtual void OnUnhandledException(Exception exception, CallbackCommandContext commandContext)
        {
            UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(exception, commandContext));
        }
    }
}
