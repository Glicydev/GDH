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
        };

        public static void UserDel(string username)
        {
            GDH.UserDel(username);
            Displayer.displayConfirmation(username + "'s account has been succefully deleted !");
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
            if (args == null)
            {
                Displayer.DisplayError("No arguments specified, please try again.");
                return;
            }

            username = args.ElementAtOrDefault(0) ?? String.Empty;

            // Parameter cannot be empty and user cannot exist
            if (string.IsNullOrEmpty(username))
            {
                Displayer.DisplayError("No username specified, please try again.");
            }
            else if (!User.UserAlreadyExists(username))
            {
                Displayer.DisplayError("User does not exist, please try again.");
            }
            else
            {
                rightPassword = GDH.askExistingPassword(username);

                if (!rightPassword)
                    return;

                while (true)
                {
                    Console.WriteLine();
                    Console.Write("New password (empty for default): ");
                    newPassword = Displayer.askPassword();
                    Console.Write("Confirm new password (empty for default): ");
                    newPasswordConfirm = Displayer.askPassword();

                    if (newPassword == newPasswordConfirm)
                    {
                        User.ChangePassword(username, newPassword);
                        Displayer.displayConfirmation("Password changed successfully !");
                        break;
                    }
                    else
                    {
                        Displayer.DisplayError("Passwords do not match, please try again.");
                    }
                }
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

            PrintCenteredText("help -> Manual of GDH.", totalLineLength);
            PrintCenteredText("clear -> Clear the console.", totalLineLength);
            PrintCenteredText("exit -> Exit the application.", totalLineLength);
            PrintCenteredText("gdf -> Display the GDF.", totalLineLength);
            PrintCenteredText("echo -> Echo somethings.", totalLineLength);
            PrintCenteredText("logout -> Logout from the actual user.", totalLineLength);
            PrintCenteredText("userdel -> Delete an user.", totalLineLength);
            PrintCenteredText("userlist -> Get the list of all the users", totalLineLength);
            PrintCenteredText("changepw -> Change the password of an user.", totalLineLength);

            Console.WriteLine(bigSpace + "|".PadRight(totalLineLength - 1, '-') + "|");
        }

        /// <summary>
        /// Print a centered text
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="text"></param>
        /// <param name="totalLineLength"></param>
        static void PrintCenteredText(string text, int totalLineLength)
        {
            int padding = (totalLineLength - text.Length) / 2 - 1;
            int rightPadding = padding;

            if (text.Length % 2 == 0)
                rightPadding -= 1;

            string paddingString = new string(' ', padding);
            string rightPaddingString = new string(' ', rightPadding);
            string centeredText = paddingString + text + rightPaddingString;

            Console.WriteLine(bigSpace + "|" + centeredText + " |");
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
        public static void Echo(string[] options)
        {
            const string messageStart = "  >> ";
            int nbOptions = options.Length;
            string message = String.Empty;
            string option = string.Empty;

            if (nbOptions == 0)
            {
                Displayer.DisplayError("No options specified, [ echo --help ] for more informations");
                return;
            }
            if (nbOptions == 1)
            {
                Console.WriteLine(messageStart + options[0]);
                return;
            }
            if (options[0].StartsWith('"') && options.Last().EndsWith('"'))
            {
                message = string.Join(" ", options);
                message = message.Substring(1, message.Length - 2);
                Console.WriteLine(messageStart + message);
                return;
            }
            message = options.Last();

            for (int i = 0; i < nbOptions - 1; i++)
            {
                option = options[i];
                if (i < nbOptions - 2 && options.ElementAt(i + 1) == null)
                {
                    Displayer.DisplayError("Invalid command, [ echo --help ] for more informations");
                    return;
                }

                switch (option)
                {
                    case "-s":
                        message = message.ToLower();
                        break;
                    case "-b":
                        message = message.ToUpper();
                        break;
                    default:
                        Displayer.DisplayError("Unknown option: " + option);
                        return;
                }
            }
            Console.WriteLine(messageStart + message);
        }
    }
}
