using System.Collections.Generic;

namespace Telegram.Bot.Core
{
    public class CommandsUsersBase
    {
        public Dictionary<long, Command> UsersCommands { get; private set; }

        public CommandsUsersBase()
        {
            UsersCommands = new Dictionary<long, Command>();
        }

        public Command GetCommandForUser(long id)
        {
            bool found = UsersCommands.TryGetValue(id, out Command command);

            return found ? command : null;
        }

        public void SetCommandForUser(long id, Command command)
        {
            bool found = UsersCommands.TryGetValue(id, out _);

            if (found)
            {
                UsersCommands[id] = command;
            }
            else
            {
                UsersCommands.Add(id, command);
            }
        }

    }
}
