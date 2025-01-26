using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GDH
{
    public class Commands
    {
        private const string bigSpace = "   ";

        public static Dictionary<string, (int PermissionLevel, Action<string[]>)> commands = new Dictionary<string, (int, Action<string[]>)>
        {
            { "exit", (1, args => Exit()) },
            { "gdf", (1, args => GDF()) },
            { "help", (1, args => Help()) },
            { "clear", (1, args => Console.Clear()) },
            { "echo", (1, args => Echo(args)) },
            { "logout", (1, args => GDH.Logout()) },
            { "userdel", (2, args => {
                string username = args.ElementAtOrDefault(0) ?? String.Empty;
                UserDel(username);
            })},
            { "userlist", (2, args => UserList()) },
            { "changepw", (2, args => changePw(args))},
            { "sudo", (0, args => Sudo(args))},
        };

        /// <summary>
        /// Execute the sudo command (super user do).
        /// </summary>
        /// <param name="args">The args of the command</param>
        public static void Sudo(string[] args)
        {
            string command = args.ElementAtOrDefault(0) ?? String.Empty;
            bool rightPassword = false;

            if (args.Count() == 0)
            {
                Displayer.DisplayError("No arguments specified, try [ sudo --help ] for more infomations.");
                return;
            }
            if (args[0] == "--help")
            {
                Displayer.displayUsage("sudo [COMMAND]", "Execute a command with elevated permissions.");
                return;
            }
            if (GDH.User.Username == "root")
            {
                Executer.ExecuteAsRoot(string.Join(" ", args));
                return;
            }

            rightPassword = GDH.askExistingPassword("root");

            if (rightPassword)
            {
                Executer.ExecuteAsRoot(string.Join(" ", args));
            }
        }

        public static void UserDel(string username)
        {
            GDH.UserDel(username);
        }

        /// <summary>
        /// Change the password of an user.
        /// </summary>
        /// <param name="username"></param>
        public static void changePw(string[] args)
        {
            bool rightPassword = false;
            string newPassword = String.Empty;
            string newPasswordConfirm = String.Empty;
            string username = String.Empty;

            // Parameter cannot be null
            if (args.Count() == 0)
            {
                Displayer.DisplayError("No arguments specified, try [ changepw --help ] for more infomations.");
                return;
            }
            if (args[0] == "--help")
            {
                Displayer.displayUsage("changepw [USERNAME]", "Change the password of an user.");
                return;
            }

            username = args.ElementAtOrDefault(0) ?? String.Empty;

            // Parameter cannot be empty and user cannot exist
            if (string.IsNullOrEmpty(username))
            {
                Displayer.DisplayError("No username specified, try [ changepw --help ] for more infomations.");
            }
            else if (!User.UserAlreadyExists(username))
            {
                Displayer.DisplayError("User does not exist, try [ changepw --help ] for more infomations.");
            }
            else
            {
                rightPassword = GDH.askExistingPassword(username);

                if (!rightPassword)
                    return;

                // Ask the new password then change it
                Console.WriteLine();
                newPassword = GDH.AskNewPassword();
                User.ChangePassword(username, newPassword);
                Displayer.displayConfirmation("Password changed successfully !");
            }
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        public static void Exit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Logout the user.
        /// </summary>
        public static void Logout()
        {
            GDH.Logout();
        }

        public static void GDF()
        {
            const string bigSpace = "       ";
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan uptime = now - GDH.StartTime;

            string[] lines =
            {
                "\u001b[38;5;34m  _______   \u001b[38;5;34m________    \u001b[38;5;34m__    \u001b[38;5;34m__",
                "\u001b[38;5;35m /\" _   \"| \u001b[38;5;35m|\"       \"\\ \u001b[38;5;35m/\" |  | \"\\" + bigSpace + " Version: " + GDH.version,
                "\u001b[38;5;36m(: ( \\___) \u001b[38;5;36m(.  ___  :)(:  (__)  :)" + bigSpace + "Uptime: " + uptime.ToString(@"hh\:mm\:ss"),
                "\u001b[38;5;37m \\/ \\      \u001b[38;5;37m|: \\   ) || \u001b[38;5;37m\\/      \\/",
                "\u001b[38;5;38m //  \\ ___ \u001b[38;5;38m(| (___\\ || \u001b[38;5;38m//  __  \\\\",
                "\u001b[38;5;39m(:   _(  _||:       :)(:  (  )  :) ",
                "\u001b[38;5;40m \\_______) \u001b[38;5;40m(________/  \\__|  |__/  \u001b[0m"
            };

            // Afficher les lignes avec un dégradé de couleur
            foreach (var line in lines)
            {
                Console.WriteLine(line); // Chaque ligne a son propre dégradé de couleur
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Display the manual of GDH.
        /// </summary>
        public static void Help()
        {
            int consoleWidth = Console.WindowWidth;
            int totalLineLength = 50;

            // Help displaying
            Console.WriteLine(bigSpace + "|".PadRight(totalLineLength - 1, '-') + "|");

            PrintLeftText("help -> Manual of GDH.", totalLineLength);
            PrintLeftText("clear -> Clear the console.", totalLineLength);
            PrintLeftText("exit -> Exit the application.", totalLineLength);
            PrintLeftText("gdf -> Display the GDF.", totalLineLength);
            PrintLeftText("echo -> Echo somethings.", totalLineLength);
            PrintLeftText("logout -> Logout from the actual user.", totalLineLength);
            PrintLeftText("userdel -> Delete an user.", totalLineLength);
            PrintLeftText("userlist -> Get the list of all the users", totalLineLength);
            PrintLeftText("changepw -> Change the password of an user.", totalLineLength);
            PrintLeftText("sudo -> Execute an command as administrator.", totalLineLength);

            Console.WriteLine(bigSpace + "|".PadRight(totalLineLength - 1, '-') + "|");
        }

        /// <summary>
        /// Print a centered text
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="text"></param>
        /// <param name="totalLineLength"></param>
        static void PrintLeftText(string text, int totalLineLength)
        {
            int padding = totalLineLength - text.Length - 4;

            string paddingString = new string(' ', padding);
            string centeredText = text + paddingString;

            Console.WriteLine(bigSpace + "| " + centeredText + " |");
        }

        /// <summary>
        /// List all the users with their permissions.
        /// </summary>
        public static void UserList()
        {
            List<List<string>> users = SQLiteConnection.GetUsers();

            Console.WriteLine(Environment.NewLine + "List of all the users:");

            for (int i = 0; i < users[0].Count; i++)
            {
                Console.WriteLine("|" + bigSpace + "> Username: " + users[0][i] + "\n|" + bigSpace + "Permission level: " + users[1][i]);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Echo an message
        /// </summary>
        /// <param name="options">The options of the command</param>
        public static void Echo(string[] optionsArray)
        {
            List<string> options = optionsArray.ToList();
            const string messageStart = "  >> ";
            bool big = false;
            bool small = false;
            int nbOptions = options.Count;
            string message = String.Empty;
            string option = string.Empty;
            Console.WriteLine(message);

            // If no options are specified then we display an error
            if (nbOptions == 0)
            {
                Displayer.DisplayError("No options specified, [ echo --help ] for more informations");
                return;
            }
            // We need to check --help first or it will be detected as text
            if (options[0] == "--help")
            {
                Console.WriteLine("Usage: echo [OPTION]... [STRING]...");
                Console.WriteLine("Echo the STRING(s) to standard output.");
                Console.WriteLine();
                Console.WriteLine("  -s        output in lowercase");
                Console.WriteLine("  -b        output in uppercase");
                Console.WriteLine("  --help    display this help and exit");
                return;
            }

            // If only one option is specified then we display it
            if (nbOptions == 1)
            {
                Console.WriteLine(messageStart + options[0]);
                return;
            }
            message = options.Last();

            // Check all options
            for (int i = 0; i < nbOptions - 1; i++)
            {
                option = options[i];

                // If the option is not followed by a value then it's false
                if (i < nbOptions - 2 && options.ElementAt(i + 1) == null)
                {
                    Displayer.DisplayError("Invalid command, [ echo --help ] for more informations");
                    return;
                }

                // Detect string text
                if (options[0].StartsWith('"') && options.Last().EndsWith('"'))
                {
                    message = string.Join(" ", options);
                    message = message.Substring(1, message.Length - 2);
                    break;
                }

                // Check the option
                switch (option)
                {
                    case "-s":
                        small = true;
                        message = message.ToLower();
                        break;
                    case "-b":
                        big = true;
                        message = message.ToUpper();
                        break;
                    default:
                        Displayer.DisplayError("Unknown option: " + option);
                        return;
                }

                // Remove the option beacause we already checked it
                options.RemoveAt(0);
            }

            if (small)
            {
                message = message.ToLower();
            }
            else if (big)
            {
                message = message.ToUpper();
            }

            Console.WriteLine(messageStart + message);
        }
    }
}
