using System;
using System.Text;
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
        private Encoding _encoding;

        /// <summary>
        /// Логгировать ли в файл
        /// </summary>
        public bool LogInFile { get; set; }

        /// <summary>
        /// Логгировать ли в консоль
        /// </summary>
        public bool LogInConsole { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="logFileName">Имя файла лога</param>
        /// <param name="errorsFileName">Имя файла ошибок</param>
        /// <param name="loggerEncoding">Кодировка для логгирования в консоль и записи в файл. По умолчанию <see cref="Encoding.UTF8"/></param>
        public Logger(string logFileName, string errorsFileName, bool logInFile = true, bool logInConsole = true, Encoding loggerEncoding = null)
        {
            LogInFile = logInFile;
            LogInConsole = logInConsole;

            _logFileName = logFileName;
            _errorsFileName = errorsFileName;

            if (loggerEncoding == null)
                _encoding = Encoding.UTF8;
            else
                _encoding = loggerEncoding;

            if (logInConsole)
                Console.OutputEncoding = _encoding;
        }

        private void Log(string message)
        {
            string log = $"[{DateTime.Now}] {message}";

            if (LogInConsole)
            {
                Console.WriteLine(log);
                Console.ResetColor();
            }

            if (LogInFile)
            {
                lock (_lockObj)
                {
                    File.AppendAllText(_logFileName, log + "\n", _encoding);
                }
            }
        }

        /// <summary>
        /// Записать в лог сообщение (белый цвет в консоли)
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения, указывается в квадратных скобках</param>
        public void LogInfo(string message, string type = "INFO")
        {
            if (LogInConsole)
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
            if (LogInConsole)
                Console.ForegroundColor = ConsoleColor.Green;

            Log($"[{type}] {message}");
        }

        /// <summary>
        /// Записать в лог сообщение от пользователя (белый цвет в консоли)
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        public void LogMessage(Message message)
        {
            if (LogInConsole)
                Console.ForegroundColor = ConsoleColor.White;

            Log($"[NEW MESSAGE] [{message.From.Id} {message.From.Username ?? ""} | {message.From.FirstName ?? ""} {message.From.LastName ?? ""}]: {message.Text ?? "null"}");
        }

        /// <summary>
        /// Записать в лог callback-сообщение от пользователя (белый цвет в консоли)
        /// </summary>
        /// <param name="callback">Сообщение пользователя</param>
        public void LogCallback(CallbackQuery callback)
        {
            if (LogInConsole)
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
            if (LogInConsole)
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
