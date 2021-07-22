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
    public class UsersBase
    {
        private readonly static object _lockObj = new object();

        public static UsersBase Current { get; set; }

        /// <summary>
        /// Массив ключей доступа
        /// </summary>
        public List<AccessKey> Keys { get; set; }

        /// <summary>
        /// Коллекция ключ-значение, где ключ - Id пользователей, значение - информация о пользователе
        /// </summary>
        public Dictionary<long, UserInfo> Users { get; set; }

        /// <summary>
        /// Массив идентификаторов всех пользователей, когда либо писавших в бота
        /// </summary>
        public List<long> AllUsersIds { get; set; }

        /// <summary>
        /// Создает экземпляр класса <see cref="UsersBase"/> с пустым массивом пользователей
        /// </summary>
        public UsersBase()
        {
            Users = new Dictionary<long, UserInfo>();
            AllUsersIds = new List<long>();
            Keys = new List<AccessKey>();
        }

        /// <summary>
        /// Создает экземпляр класса <see cref="UsersBase"/> с заданным массивом пользователей
        /// </summary>
        /// <param name="users">Массив пользователей, которых необходимо загрузить в базу</param>
        public UsersBase(Dictionary<long, UserInfo> users)
        {
            Users = users;
            AllUsersIds = new List<long>();
            Keys = new List<AccessKey>();
            foreach (var item in Users)
            {
                AllUsersIds.Add(item.Key);
            }
        }

        /// <summary>
        /// Возвращает <see cref="UserInfo"/> по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns><see cref="UserInfo"/> по идентификатору пользователя</returns>
        public UserInfo GetById(long userId)
        {
            if (!AllUsersIds.Contains(userId))
                AllUsersIds.Add(userId);

            bool constains = Users.TryGetValue(userId, out UserInfo user);

            if (constains)
                return user;

            var newUser = new UserInfo();
            Users.Add(userId, newUser);
            return newUser;
        }

        /// <summary>
        /// Устанавливает указанный <see cref="UserInfo"/> по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="userInfo"><see cref="UserInfo"/>, который необходимо установить</param>
        public void SetById(long userId, UserInfo userInfo)
        {
            Users[userId] = userInfo;
        }

        /// <summary>
        /// Возвращает json-строку, представляющую данный экземпляр <see cref="UsersBase"/>
        /// </summary>
        /// <returns>Json-строка, представляющая данный экземпляр <see cref="UsersBase"/></returns>
        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Создает <see cref="UsersBase"/> из json-строки
        /// </summary>
        /// <param name="jsonString">Строка json</param>
        /// <returns>Новый экземпляр <see cref="UsersBase"/>, загруженный из json-строки</returns>
        public static UsersBase LoadFromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<UsersBase>(jsonString);
        }

        /// <summary>
        /// Начинает автоматическое сохранение в файл в отдельном потоке
        /// </summary>
        /// <param name="fileName">Путь к файлу, в который необходимо сохранять данные</param>
        /// <param name="delay">Задержка между сохранениями</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <remarks>Данный метод не блокирует поток! Для отмены используйте <see cref="CancellationToken"/></remarks>
        public void StartAutoSave(string fileName, TimeSpan delay, CancellationToken cancellationToken)
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

        public bool CanUseCommand(string key, long userId, int requestedLevel)
        {
            if (requestedLevel < 0)
                return true;

            AccessKey accessKey = Keys.FirstOrDefault(x => x.Key == key && x.AttachedUserId == userId);

            if (accessKey == null)
                return false;

            if (requestedLevel <= accessKey.AccessLevel)
            {
                if (accessKey.KeyDuration == TimeSpan.MaxValue)
                    return true;

                return accessKey.StartTime + accessKey.KeyDuration > DateTime.UtcNow;
            }
            else return false;
        }
    }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Ключ доступа. Если не уканан - <see langword="null"/>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Коллекция ключ-значение, где ключ - имя параметра, а его значение, как ни странно, значение. Используйте для хранения нужной вам информации о пользователе 
        /// </summary>
        public Dictionary<string, object> CustomFields { get; set; }

        /// <summary>
        /// Конструктор класса <see cref="UserInfo"/>
        /// </summary>
        public UserInfo()
        {
            CustomFields = new Dictionary<string, object>();
        }
    }
}
