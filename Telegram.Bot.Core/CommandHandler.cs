using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Core
{
    public class CommandHandler
    {
        public ILogger Logger { get; set; }
        public CommandsUsersBase UsersCommands { get; }
        private List<Type> _commands;

        public CommandHandler()
        {
            UsersCommands = new CommandsUsersBase();
            _commands = new List<Type>();
        }

        public CommandHandler(CommandsUsersBase usersBase) : this()
        {
            UsersCommands = usersBase;
        }

        public CommandHandler(ILogger logger) : this()
        {
            Logger = logger;
        }

        public CommandHandler(CommandsUsersBase usersBase, ILogger logger) : this(usersBase)
        {
            Logger = logger;                
        }

        public async Task HandleAsync(TelegramBotClient client, Message message)
        {
            try
            {
                Command command = FindCommandForExecute(message.From.Id, message);

                if(command == null)
                {
                    OnUnknownCommand(client, message);
                }
                else
                {
                    CommandContext context = new CommandContext(message, client);
                    if(CanExecute(message.From.Id, command))
                    {
                        try
                        {
                            await command.ExecutePartAsync(context);

                            if (command.IsCompleted)
                            {
                                UsersCommands.SetCommandForUser(message.From.Id, null);
                                OnCommandCompleted(context, command, message.From.Id, command.IsSuccess, command.Tag);
                            }
                        }
                        catch (Exception e)
                        {
                            OnUnhandledException(e, context);
                        }
                    }
                    else
                    {
                        UsersCommands.SetCommandForUser(message.From.Id, null);
                        OnCannotExecute(context);
                    }
                }
            }
            catch { }
        }

        public void AddCommand<T>() where T : Command
        {
            _commands.Add(typeof(T));
        }

        protected virtual void OnCommandCompleted(CommandContext commandContext, Command command, long userId, bool success, object tag)
        {

        }

        protected virtual async void OnCannotExecute(CommandContext commandContext)
        {
            await commandContext.BotClient.SendTextMessageAsync(commandContext.Message.Chat.Id, "Not enought permissions");
        }

        protected virtual async void OnUnknownCommand(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Unknown command");
        }

        protected virtual void OnUnhandledException(Exception e, CommandContext commandContext)
        {
            Logger?.LogError($"Unhandled exception {e.GetType().Name}: {e.Message} {e.StackTrace}");
        }

        protected virtual bool CompareCommandNameForMessage(string message, string commandName)
        {
            return commandName == message;
        }

        protected virtual bool CanExecute(long userId, Command command)
        {
            return true;
        }

        private Command FindCommandForExecute(long userId, Message message)
        {
            try
            {
                Command command = UsersCommands.GetCommandForUser(userId);

                if (command != null)
                    return command;

                if (string.IsNullOrWhiteSpace(message.Text))
                    return null;

                foreach (Type type in _commands)
                {
                    foreach (Attribute attribute in type.GetCustomAttributes(true))
                    {
                        if(attribute is CommandNameAttribute commandName)
                        {
                            if(CompareCommandNameForMessage(message.Text, commandName.Name))
                            {
                                command = (Command)Activator.CreateInstance(type);

                                UsersCommands.SetCommandForUser(userId, command);

                                return command;
                            }
                        }
                    }

                }
                return null;

            }
            catch
            {
                return null;
            }
        }
    }

    public delegate void OnUnknownCommandHandler(TelegramBotClient client, Message message);
}
