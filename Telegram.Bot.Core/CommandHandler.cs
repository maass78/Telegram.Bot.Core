using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Core.Attributes;
using Telegram.Bot.Core.Callback;
using Telegram.Bot.Types;

namespace Telegram.Bot.Core
{
    /// <summary>
    /// Класс, предоставляющий методы для обработки сообщений пользователя
    /// </summary>
    public class CommandHandler
    {
        /// <summary>
        /// Информация о командах пользователей
        /// </summary>
        public CommandsUsersBase UsersCommands { get; }

        private List<Type> _commands;
        private List<Type> _callbackCommands;

        /// <summary>
        /// Вызывается при выбрасывании необрабатываемого исключения
        /// </summary>
        public event UnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// Вызывается при получении нового сообщения
        /// </summary>
        public event NewMessageEventHandler NewMessage;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public CommandHandler()
        {
            UsersCommands = new CommandsUsersBase();
            _commands = new List<Type>();
            _callbackCommands = new List<Type>();
        }

        /// <summary>
        /// Обработать новое сообщение
        /// </summary>
        /// <param name="client">Экземпляр класса <see cref="TelegramBotClient"/>, с помощью которого необходимо обработать новое сообщение</param>
        /// <param name="message">Сообщение, которое необходимо обработать</param>
        public async Task HandleAsync(TelegramBotClient client, Message message)
        {
            try
            {
                CommandContext context = new CommandContext(message, client);

                NewMessage?.Invoke(this, new NewMessageEventArgs(message));

                if (IsUserBlocked(context))
                    return;

                Command command = FindCommandForExecute(message.From.Id, message, client);

                if (command == null)
                {
                    OnUnknownCommand(context);
                    return;
                }
                
                try
                {
                    if (!CanExecute(context, command))
                    {
                        UsersCommands.SetCommandForUser(message.From.Id, null);
                        OnCannotExecuteCommand(context);
                        return;
                    }

                    await command.ExecutePartAsync(context);
                
                    if (command.IsCompleted)
                    {
                        UsersCommands.SetCommandForUser(message.From.Id, null);
                        OnCommandCompleted(context, command);
                    }
                }
                catch (Exception ex)
                {
                    UsersCommands.SetCommandForUser(message.From.Id, null);
                    OnCommandCompleted(context, command);
                    OnUnhandledException(ex, context);
                }
           }
           catch (Exception ex) { OnUnhandledException(ex, null); }
        }

        /// <summary>
        /// Обработать новое callback-сообщение
        /// </summary>
        /// <param name="client">Экземпляр класса <see cref="TelegramBotClient"/>, с помощью которого необходимо обработать новое callback-сообщение</param>
        /// <param name="callback">Callback-cообщение, которое необходимо обработать</param>
        public async Task HandleCallbackAsync(TelegramBotClient client, CallbackQuery callback)
        {
            try
            {
                CallbackCommandContext callbackContext = new CallbackCommandContext(callback, client);

                if (IsUserBlocked(callbackContext))
                    return;

                CallbackCommand command = FindCallbackCommandForExecute(callback.From.Id, callback, client);

                if (command == null)
                {
                    OnUnknownCallback(callbackContext);
                    return;
                }

                try
                {
                    if (!CanExecute(callbackContext, command))
                    {
                        if (!command.OnlyResponse)
                            UsersCommands.SetCommandForUser(callback.From.Id, null);

                        OnCannotExecuteCallback(callbackContext);

                        return;
                    }

                    await command.Execute(callbackContext);
                }
                catch (Exception ex)
                {
                    if (!command.OnlyResponse)
                        UsersCommands.SetCommandForUser(callback.From.Id, null);

                    OnUnhandledException(ex, callbackContext);
                }
            }
            catch (Exception ex) { OnUnhandledException(ex, null); }
        }

        /// <summary>
        /// Добавить новую команду
        /// </summary>
        /// <typeparam name="T">Класс, производный от <see cref="Command"/></typeparam>
        public void AddCommand<T>() where T : Command
        {
            _commands.Add(typeof(T));
        }

        /// <summary>
        /// Добавить новую callback-команду
        /// </summary>
        /// <typeparam name="T">Класс, производный от <see cref="CallbackCommand"/></typeparam>
        public void AddCallbackCommand<T>() where T : CallbackCommand
        {
            _callbackCommands.Add(typeof(T));
        }

        /// <summary>
        /// Вызывается после окончания выполнения команды
        /// </summary>
        /// <param name="context">Контекст завершенной команды</param>
        /// <param name="command">Команда, которая завершила своё выполнение</param>
        protected virtual void OnCommandCompleted(CommandContext context, Command command)
        {

        }

        /// <summary>
        /// Вызывается, если пользователь не прошел проверку <see cref="CanExecute(BaseCommandContext, Command)"/> при выполнении команды
        /// </summary>
        /// <param name="context">Контекст команды</param>
        protected virtual async void OnCannotExecuteCommand(CommandContext context)
        {
            await context.BotClient.SendTextMessageAsync(context.Chat, "Not enought permissions");
        }

        /// <summary>
        /// Вызывается, если пользователь не прошел проверку <see cref="CanExecute(BaseCommandContext, Command)"/> при выполнении callback-команды
        /// </summary>
        /// <param name="context">Контекст callback-команды</param>
        protected virtual async void OnCannotExecuteCallback(CallbackCommandContext context)
        {
            await context.BotClient.SendTextMessageAsync(context.Chat, "Not enought permissions");
            await context.BotClient.AnswerCallbackQueryAsync(context.CallbackQuery.Id);
        }

        /// <summary>
        /// Вызывается, если пользователь ввел неизвестную команду
        /// </summary>
        /// <param name="context">Контекст команды</param>
        protected virtual async void OnUnknownCommand(CommandContext context)
        {
            await context.BotClient.SendTextMessageAsync(context.Chat, "Unknown command");
        }

        /// <summary>
        /// Вызывается, если пользователь ввел неизвестную callback-команду
        /// </summary>
        /// <param name="context">Контекст callback-команды</param>
        protected virtual async void OnUnknownCallback(CallbackCommandContext context)
        {
            await context.BotClient.SendTextMessageAsync(context.Chat, "Unknown command");
        }

        /// <summary>
        /// Вызывается при выбрасывании необрабатываемого исключения
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="context">Контекст команды</param>
        protected virtual void OnUnhandledException(Exception ex, BaseCommandContext context)
        {
            UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, context));
        }

        /// <summary>
        /// Может ли пользователь выполнить команду
        /// </summary>
        /// <param name="context">Контекст команды</param>
        /// <param name="command">Команда, которую пользователь хочет выполнить</param>
        /// <returns><see langword="true"/>, если пользователю разрешено выполнять команду, иначе - <see langword="false"/></returns>
        protected virtual bool CanExecute(BaseCommandContext context, Command command)
        {
            return true;
        }

        /// <summary>
        /// Заблокирован ли пользователь
        /// </summary>
        /// <param name="context">Контекст команды</param>
        /// <returns><see langword="true"/>, если пользователь заблокирован, иначе - <see langword="false"/></returns>
        protected virtual bool IsUserBlocked(BaseCommandContext context)
        {
            return false;
        }

        private Command FindCommandForExecute(long userId, Message message, TelegramBotClient botClient)
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
                        if (attribute is CommandNameAttribute commandName)
                        {
                            if (commandName.Compare(message.Text, new BaseCommandContext(message.Chat, message.From, botClient)))
                            {
                                command = (Command)Activator.CreateInstance(type);

                                command.Args = GetCommandArgs(type, message.Text);

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

        private CallbackCommand FindCallbackCommandForExecute(long userId, CallbackQuery callback, TelegramBotClient botClient)
        {
            foreach (Type type in _callbackCommands)
            {
                foreach (Attribute attribute in type.GetCustomAttributes(true))
                {
                    if (attribute is CommandNameAttribute commandName)
                    {
                        if (commandName.Compare(callback.Data, new BaseCommandContext(callback.Message.Chat, callback.From, botClient)))
                        {
                            var command = (CallbackCommand)Activator.CreateInstance(type);

                            command.Args = GetCommandArgs(type, callback.Data);

                            if (!command.OnlyResponse)
                                UsersCommands.SetCommandForUser(userId, command);

                            return command;
                        }
                    }
                }
            }

            return null;
        }

        private string[] GetCommandArgs(Type commandType, string userInput)
        {
            foreach (Attribute attribute in commandType.GetCustomAttributes(true))
            {
                if (attribute is CommandWithArgsAttribute commandArgs)
                {
                    if (commandArgs.WithArgs && !string.IsNullOrWhiteSpace(userInput))
                    {
                        var parts = userInput.Split(new string[] { commandArgs.Separator }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length > 1)
                        {
                            parts = parts.Skip(1).ToArray();
                            return parts;
                        }
                    }
                }
            }

            return null;
        }
    }

    public delegate void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs eventArgs);

    public class UnhandledExceptionEventArgs : EventArgs
    {
        public UnhandledExceptionEventArgs(Exception exception, BaseCommandContext commandContext)
        {
            Exception = exception;
            CommandContext = commandContext;
        }

        /// <summary>
        /// Исключение
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Контекст команды
        /// </summary>
        public BaseCommandContext CommandContext { get; }
    }

    public delegate void NewMessageEventHandler(object sender, NewMessageEventArgs eventArgs);

    public class NewMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Новое сообщение
        /// </summary>
        public Message Message { get; }

        public NewMessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
