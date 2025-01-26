using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GDH
{
    public class GDH
    {
        public const string name = "GDH";
        public const string version = "1.0.0";

        // Defining the colors
        public static ConsoleColor StyleColor { get; } = ConsoleColor.Magenta;
        public static ConsoleColor ErrColor { get; } = ConsoleColor.Red;
        public static ConsoleColor WarnColor { get; } = ConsoleColor.Yellow;
        public static ConsoleColor InfoColor { get; } = ConsoleColor.Cyan;
        public static ConsoleColor ConfirmColor { get; } = ConsoleColor.Green;

        // attributes
        public static TimeSpan StartTime { get; } = DateTime.Now.TimeOfDay;
        public static User User;

        /// <summary>
        /// Start the shell.
        /// </summary>
        private static void Start()
        {
            Console.Clear();
            FColor(StyleColor);
            string command = String.Empty;
            string[] commandWithParams;
            string[] commandParams;
            Console.WriteLine("Welcome to GDH ! Type 'help' for the list of the commands");
            Console.ResetColor();
            Console.WriteLine();

            while (true && command != "logout")
            {
                Console.Write(User.Username + "@");
                Displayer.displayWithColor("GDH", ConsoleColor.DarkGreen);
                Console.Write("> ");

                command = Console.ReadLine().ToLower();

                Executer.Execute(command);
                Console.WriteLine();
            }
        }

        public static int getPermissions()
        {
            return User.PermissionLevel;
        }

        /// <summary>
        /// Delete an user from the database.
        /// </summary>
        /// <param name="username"></param>
        public static void UserDel(string username)
        {
            string error = String.Empty;
            string userPassword = SQLiteConnection.GetPasswd(username);
            bool rightPassword = false;

            if (username == null)
            {
                error = "Please enter an username";
            }
            else if (username == User.Username)
            {
                error = "You can't delete yourself !";
            }
            else if (!User.UserAlreadyExists(username))
            {
                error = "User does not exist";
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

                if (error != String.Empty)
                {
                    Displayer.DisplayError(error);
                }
                else
                {
                    SQLiteConnection.DeleteUser(username);
                    Displayer.displayConfirmation(username + "'s account has been succefully deleted !");
                }
            }
        }

        public static bool askExistingPassword(string username)
        {
            int attempts = 0;
            string userPassword = SQLiteConnection.GetPasswd(username);
            bool rightPassword = false;
            string password = String.Empty;

            while (!rightPassword && attempts < 3)
            {
                Console.Write(username + "'s password: ");
                password = Displayer.askPassword();
                rightPassword = userPassword == password;

                if (!rightPassword)
                {
                    Displayer.DisplayError("Wrong password !");
                }

                attempts++;
            }

            return rightPassword;
        }

        /// <summary>
        /// Log out the user.
        /// </summary>
        public static void Logout()
        {
            Console.Clear();
            signInOrUp();
        }

        /// <summary>
        /// Sign in or sign up the user.
        /// </summary>
        public static void signInOrUp()
        {
            SQLiteConnection.Start();

#if (DEBUG)
                User = new User("root", "gdh");

#elif (RELEASE)
            Console.Write("Please enter an username: ");

            string password = String.Empty;
            string username = Console.ReadLine();

            // If he exists sign in else sign up
            if (User.UserAlreadyExists(username))
            {
                bool rightPassword = askExistingPassword(username);

                if (!rightPassword)
                {
                    Displayer.DisplayError("Too many attempts, exiting...");
                    Environment.Exit(1);
                }
                Console.Clear();
            }
            else
            {
                password = AskNewPassword();
            }

            // It will automatically sign in/up the user
            User = new User(username, password);
#endif
            Start();
        }

        public static string AskNewPassword()
        {
            bool passwordMatches = false;
            string password = String.Empty;
            string passwordConfirm = String.Empty;

            while (!passwordMatches)
            {
                Console.Write("new password (empty for default): ");
                password = Displayer.askPassword();

                Console.Write("confing password (empty for default): ");
                passwordConfirm = Displayer.askPassword();

                if (password == passwordConfirm)
                    passwordMatches = true;
                else
                    Displayer.DisplayError("Passwords do not match");
            }

            if (password == String.Empty)
                return "gdh";
            else
                return password;
        }

        /// <summary>
        /// Change the foreground color of the console.
        /// </summary>
        /// <param name="c">The foreground color to set</param>
        public static void FColor(ConsoleColor c)
        {
            Console.ForegroundColor = c;
        }
    }
}
