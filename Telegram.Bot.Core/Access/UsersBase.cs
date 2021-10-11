using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Telegram.Bot.Core.Access
{
    /// <summary>
    /// Класс, позволяющий управлять базой пользователей
    /// </summary>
    public class UsersBase<T>
    {
        private readonly static object _lockObj = new object();

        public static UsersBase<T> Current { get; set; }

        /// <summary>
        /// Массив ключей доступа
        /// </summary>
        public List<AccessKey> Keys { get; set; }

        /// <summary>
        /// Коллекция ключ-значение, где ключ - Id пользователей, значение - информация о пользователе
        /// </summary>
        public List<UserInfo<T>> Users { get; set; }

        /// <summary>
        /// Массив идентификаторов заблокированных пользователей
        /// </summary>
        public List<long> BlockedUsers { get; set; }

        /// <summary>
        /// Создает экземпляр класса <see cref="UsersBase{T}"/> с пустым массивом пользователей
        /// </summary>
        public UsersBase()
        {
            Users = new List<UserInfo<T>>();
            Keys = new List<AccessKey>();
            BlockedUsers = new List<long>();
        }

        /// <summary>
        /// Создает экземпляр класса <see cref="UsersBase{T}"/> с заданным массивом пользователей
        /// </summary>
        /// <param name="users">Массив пользователей, которых необходимо загрузить в базу</param>
        /// <param name="keys">Массив ключей доступа, которых необходимо загрузить в базу</param>
        public UsersBase(List<UserInfo<T>> users, List<AccessKey> keys)
        {
            Users = users;
            Keys = keys;
            BlockedUsers = new List<long>();
        }

        /// <summary>
        /// Возвращает <see cref="UserInfo{T}"/> по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        public UserInfo<T> GetById(long userId)
        {
            var user = Users.FirstOrDefault(x => x.Id == userId);

            if (user != null)
                return user;

            var newUser = new UserInfo<T>() { Id = userId };
            Users.Add(newUser);
            return newUser;
        }

        /// <summary>
        /// Возвращает <see cref="UserInfo{T}"/> по контексту команды
        /// </summary>
        /// <param name="context">Контекст команды</param>
        public UserInfo<T> GetById(BaseCommandContext context)
        {
            var user = Users.FirstOrDefault(x => x.Id == context.From.Id);

            if (user != null)
                return user;

            var newUser = new UserInfo<T>() { Id = context.From.Id, Username = context.From.Username };
            Users.Add(newUser);
            return newUser;
        }

        /// <summary>
        /// Возвращает json-строку, представляющую данный экземпляр <see cref="UsersBase{T}"/>
        /// </summary>
        /// <returns>Json-строка, представляющая данный экземпляр <see cref="UsersBase{T}"/></returns>
        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Создает <see cref="UsersBase{T}"/> из json-строки
        /// </summary>
        /// <param name="jsonString">Строка json</param>
        /// <returns>Новый экземпляр <see cref="UsersBase{T}"/>, загруженный из json-строки</returns>
        public static UsersBase<T> LoadFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<UsersBase<T>>(jsonString);
        }

        /// <summary>
        /// Начинает автоматическое сохранение в файл в отдельном потоке
        /// </summary>
        /// <param name="fileName">Путь к файлу, в который необходимо сохранять данные</param>
        /// <param name="delay">Задержка между сохранениями</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <remarks>Данный метод не блокирует поток! Для отмены используйте <see cref="CancellationToken"/></remarks>
        public virtual void StartAutoSave(string fileName, TimeSpan delay, CancellationToken cancellationToken)
        {
            new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    lock (_lockObj)
                    {
                        File.WriteAllText(fileName, GetJsonString());
                    }

                    Thread.Sleep(delay);
                }

            }).Start();
        }

        public virtual bool IsUserBlocked(BaseCommandContext commandContext) => BlockedUsers.Contains(commandContext.From.Id);

        public virtual bool CanUseCommand(BaseCommandContext commandContext, int requestedLevel)
        {
            var user = GetById(commandContext);

            AccessKey accessKey = Keys.FirstOrDefault(x => x.AttachedUserId == commandContext.From.Id);

            if (accessKey == null || accessKey.StartTime + accessKey.KeyDuration <= DateTime.UtcNow)
            {
                user.AccessLevel = -1;
            }
            else
            {
                user.AccessLevel = accessKey.AccessLevel;
            }

            return user.AccessLevel >= requestedLevel;
        }
    }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class UserInfo<T>
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public int AccessLevel { get; set; }

        /// <summary>
        /// Коллекция ключ-значение, где ключ - имя параметра, а его значение, как ни странно, значение. Используйте для хранения нужной вам информации о пользователе 
        /// </summary>
        public T CustomInfo { get; set; }

        /// <summary>
        /// Конструктор класса <see cref="UserInfo{T}"/>
        /// </summary>
        public UserInfo()
        {
            AccessLevel = -1;
        }
    }
}
