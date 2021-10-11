using System;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace Telegram.Bot.Core.Utilities
{
    /// <summary>
    /// Класс, предоставляющий методы для логирования сообщений, предупреждений и ошибок
    /// </summary>
    public class Logger 
    {
        /// <summary>
        /// Текущий экземпляр класса <see cref="Logger"/>
        /// </summary>
        public static Logger Current;
        private static readonly object _lockObj = new object();
        private static readonly Random _random = new Random();

        private string _logFileName;
        private string _errorsFileName;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="logFileName">Имя файла лога</param>
        /// <param name="errorsFileName">Имя файла ошибок</param>
        public Logger(string logFileName, string errorsFileName)
        {
            _logFileName = logFileName;
            _errorsFileName = errorsFileName;
        }

        private void Log(string message)
        {
            string log = $"[{DateTime.Now}] {message}";

            Console.WriteLine(log);
            lock (_lockObj)
            {
                File.AppendAllText(_logFileName, log + "\n");
            }
        }

        /// <summary>
        /// Записать в лог сообщение (белый цвет в консоли)
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения, указывается в квадратных скобках</param>
        public void LogInfo(string message, string type = "INFO")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Log($"[{type}] {message}");
        }

        /// <summary>
        /// Записать в лог успешную операцию (зеленый цвет в консоли)
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения, указывается в квадратных скобках</param>
        public void LogSuccess(string message, string type = "SUCCESS")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log($"[{type}] {message}");
        }

        /// <summary>
        /// Записать в лог сообщение от пользователя (белый цвет в консоли)
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        public void LogMessage(Message message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Log($"[NEW MESSAGE] [{message.From.Id} {message.From.Username ?? ""} | {message.From.FirstName ?? ""} {message.From.LastName ?? ""}]: {message.Text ?? "null"}");
        }

        /// <summary>
        /// Записать в лог callback-сообщение от пользователя (белый цвет в консоли)
        /// </summary>
        /// <param name="callback">Сообщение пользователя</param>
        public void LogCallback(CallbackQuery callback)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Log($"[NEW MESSAGE] [{callback.From.Id} {callback.From.Username ?? ""} | {callback.From.FirstName ?? ""} {callback.From.LastName ?? ""}]: {callback.Data ?? "null"}");
        }

        /// <summary>
        /// Записать в лог ошибку (красный цвет в консоли)
        /// </summary>
        /// <param name="ex">Исключение, которое необходимо записать</param>
        /// <param name="additionalInfo">Дополнительная информация об ошибке</param>
        /// <param name="type">Тип сообщения, указывается в квадратных скобках</param>
        /// <returns>ID ошибки, по которому её можно найти в логе</returns>
        public int LogException(Exception ex, string type = "ERROR", string additionalInfo = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            int errorId = _random.Next(1000000, 9999999);

            string info = additionalInfo != null ? $"| {additionalInfo}" : string.Empty;

            Log($"[{type}] ID: {errorId} {info} | {ex.GetType()} - {ex.Message}");

            lock (_lockObj)
            {
                File.AppendAllText(_errorsFileName, $"[{DateTime.Now}] [{errorId}] {ex}\n\n");
            }

            return errorId;
        }
    }
}
