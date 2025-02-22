using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using GDH.database;

namespace GDH.Managers
{
    public class Commands
    {
        private const string bigSpace = "   ";

        public static Dictionary<string, (int PermissionLevel, Action<string[]>)> commands = new Dictionary<string, (int, Action<string[]>)>
        {
            { "help", (1, args => Help()) },
            { "?", (1, args => Help()) },
            { "exit", (1, args => Exit()) },
            { "gdf", (1, args => GDF()) },
            { "clear", (1, args => Console.Clear()) },
            { "echo", (1, args => Echo(args)) },
            { "logout", (1, args => GDH.Logout()) },
            { "userdel", (2, args => {
                string username = args.ElementAtOrDefault(0) ?? string.Empty;
                UserDel(username);
            })},
            { "userlist", (2, args => UserList()) },
            { "changepw", (2, args => changePw(args))},
            { "sudo", (1, args => Sudo(args))},
            { "changeperms", (2, args => ChangePerms(args))},
            { "ping", (2, args => Ping(args))},
        };

        /// <summary>
        /// Execute the sudo command (super user do).
        /// </summary>
        /// <param name="args">The args of the command</param>
        public static void Sudo(string[] args)
        {
            string command = args.ElementAtOrDefault(0) ?? string.Empty;
            bool rightPassword = false;

            if (args.Count() == 0)
            {
                Displayer.DisplayError("No arguments specified, try [ sudo --help ] for more infomations.");
                return;
            }
            if (args[0] == "--help")
            {
                Displayer.DisplayUsage("sudo [COMMAND]", "Execute a command with elevated permissions.");
                return;
            }
            if (GDH.User.Username == "root")
            {
                Executer.ExecuteAsRoot(string.Join(" ", args));
                return;
            }
            if (!GDH.User.RootAccess) {
                rightPassword = GDH.askExistingPassword("root");
            }
            else {
                rightPassword = true;
            }

            if (rightPassword)
            {
                GDH.User.RootAccess = true;
                Executer.ExecuteAsRoot(string.Join(" ", args));
            }
        }

        /// <summary>
        /// Delete an user from the database.
        /// </summary>
        /// <param name="username">The username of the user</param>
        public static void UserDel(string username)
        {
            string error = string.Empty;
            string userPassword = SQLiteConnection.GetPasswd(username);
            bool rightPassword = false;

            if (username == null)
            {
                error = "Please enter an username";
            }
            else if (username == GDH.User.Username)
            {
                error = "You can't delete yourself !";
            }
            else if (!User.UserAlreadyExists(username))
            {
                error = "User does not exist";
            }
            else if (username == "root")
            {
                error = "You can't delete the root account !";
            }
            else
            {
                if (GDH.getPermissions() >= 2)
                    rightPassword = true;

                else
                    rightPassword = GDH.askExistingPassword(username);

                if (!rightPassword)
                {
                    error = "Too many tries";
                }
            }

            if (error != string.Empty)
            {
                Displayer.DisplayError(error);
            }
            else
            {
                SQLiteConnection.DeleteUser(username);
                Displayer.displayConfirmation(username + "'s account has been succefully deleted !");
            }
        }

        /// <summary>
        /// Change the password of an user.
        /// </summary>
        /// <param name="username"></param>
        public static void changePw(string[] args)
        {
            bool rightPassword = false;
            string newPassword = string.Empty;
            string newPasswordConfirm = string.Empty;
            string username = string.Empty;

            // Parameter cannot be null
            if (args.Count() == 0)
            {
                Displayer.DisplayError("No arguments specified, try [ changepw --help ] for more infomations.");
                return;
            }
            if (args[0] == "--help")
            {
                Displayer.DisplayUsage("changepw [USERNAME]", "Change the password of an user.");
                return;
            }

            username = args.ElementAtOrDefault(0) ?? string.Empty;

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
                if (GDH.getPermissions() >= 2)
                {
                    rightPassword = true;
                }
                else
                {
                    rightPassword = GDH.askExistingPassword(username);
                }

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
        /// Check for the ping to be right structured
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool PingIsRight(string[] args)
        {
            return args.Count() == 0 || (args.Count() > 1 && (args.Count() != 3 || !int.TryParse(args[2], out _)));
        }

        /// <summary>
        /// Ping an server and tell if it's on or off
        /// </summary>
        /// <param name="args"></param>
        public static void Ping(string[] args)
        {
            string hostname = String.Empty;
            int port = 80;

            // Parameter cannot be null
            if (PingIsRight(args))
            {
                Displayer.DisplayError("Bad arguments, try [ changepw --help ] for more infomations.");
                return;
            }
            if (args[0] == "--help")
            {
                Displayer.DisplayUsage("ping [HOSTNAME] -p [PORT]", "Ping an hostmane with an port");
                return;
            }

            hostname = args[0];

            if (args.Count() > 1 && args[1] == "-p")
            {
                port = Convert.ToInt32(args[2]);
            }

            try
            {
                using (TcpClient client = new TcpClient(hostname, port)) {
                    Displayer.displayWithColor($"> Client is on ({hostname}:{port})", GDH.ConfirmColor);
                }
            }
            catch (SocketException ex)
            {
                Displayer.displayWithColor($"> Client is off ({hostname}:{port})", GDH.ErrColor);
            }
            Console.WriteLine();
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

        public static void ChangePerms(string[] args)
        {
            string username = "";
            string permissionLevelString = args.ElementAtOrDefault(1);
            int permissionLevel = 0;

            if (args.Count() == 0 || !int.TryParse(permissionLevelString, out permissionLevel) || permissionLevel < 1 || permissionLevel > 2)
            {
                Displayer.DisplayError("Invalid arguments, try [ changeperms --help ] for help");
                return;
            }
            if (args[0] == "--help")
            {
                Displayer.DisplayUsage("changeperms [ USERNAME ] [ PERMISSION LEVEL ]", "The permission level must be 1 for normal user or 2 for admin");
                return;
            }

            username = args[0];

            if (!User.UserAlreadyExists(username))
            {
                Displayer.DisplayError($"user \"{username}\" does not exist");
            }
            else if (username == "root")
            {
                Displayer.DisplayError("You can't change the root's password !");
            }
            else
            {
                User.ChangePermissionLevel(username, permissionLevel);
                Displayer.displayConfirmation($"{username}'s permission has been succefully set to {permissionLevel}");
            }
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
                "\u001b[38;5;36m(: ( \\___) \u001b[38;5;36m(.  ___  :)(:  (__)  :)" + bigSpace + "Uptime: " + uptime.ToString(@"hh\h\ mm\m\ ss\s"),
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
            const int totalWidthLeft = 12;
            const string arrow = "->";
            int consoleWidth = Console.WindowWidth;

            // Help displaying
            Console.WriteLine();
            PrintRightText("help ",  arrow + " Manual of GDH.", totalWidthLeft);
            PrintRightText("clear ", arrow + " Clear the console.", totalWidthLeft);
            PrintRightText("exit ", arrow + " Exit the application.", totalWidthLeft);
            PrintRightText("gdf ", arrow + " Display the GDF.", totalWidthLeft);
            PrintRightText("logout ", arrow + " Logout from the actual user.", totalWidthLeft);
            PrintRightText("userdel ", arrow + " Delete an user.", totalWidthLeft);
            PrintRightText("userlist ", arrow + " Get the list of all the users", totalWidthLeft);
            PrintRightText("changepw ", arrow + " Change the password of an user.", totalWidthLeft);
            PrintRightText("sudo ", arrow + " Execute an command as administrator.", totalWidthLeft);
        }

        /// <summary>
        /// Print a centered text
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="text"></param>
        /// <param name="totalLineLength"></param>
        static void PrintRightText(string text, string desc,  int totalWidthLeft)
        {
            Console.WriteLine(text.PadLeft(totalWidthLeft, ' ') + desc);
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
            string message = string.Empty;
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
