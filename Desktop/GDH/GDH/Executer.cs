using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDH
{
    public class Executer
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="command">The name of the command</param>
        private static void ExecuteCommand(string command)
        {
            string[] commandWithParams = command.Split(' ');
            string[] options = commandWithParams.Skip(1).ToArray();
            command = commandWithParams[0];

            if (string.IsNullOrEmpty(command))
                return;

            switch (command)
            {
                case "echo":
                    Commands.Echo(options);
                    break;
                case "userdel":
                    Commands.commands[command].Item2(options);
                    break;
                case "changepw":
                    Commands.commands[command].Item2(options);
                    break;
                default:
                    try
                    {
                        if (!Commands.commands.ContainsKey(command))
                        {
                            throw new Exception("Unknown command: " + command);
                        }

                        Commands.commands[command].Item2(null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    break;
            }
        }

        public static void Execute(string command)
        {
            string commandName = command.Split(' ')[0];

            try
            {
                bool rightPermissions = false;

                if (string.IsNullOrEmpty(commandName))
                {
                    return;
                }
                if (!Commands.commands.ContainsKey(commandName))
                {
                    throw new Exception("Unknown command: " + commandName);
                }

                rightPermissions = GDH.getPermissions() >= Commands.commands[commandName].PermissionLevel;

                if (!rightPermissions)
                {
                    throw new Exception("You don't have the permissions to execute this command.");
                }

                ExecuteCommand(command);
            }
            catch (Exception ex)
            {
                Displayer.DisplayError(ex.Message);
            }
        }
    }
}
