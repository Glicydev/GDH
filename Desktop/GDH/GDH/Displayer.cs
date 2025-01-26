using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDH
{
    public class Displayer
    {
        private const string bigSpace = "   ";

        /// <summary>
        /// Show an unknown command message.
        /// </summary>
        /// <param name="command"></param>
        public static void Unknown(string command)
        {
            DisplayError($"Unknown command: {command}");
        }

        /// <summary>
        /// Display an error message.
        /// </summary>
        /// <param name="error">The error message</param>
        public static void DisplayError(string error)
        {
            GDH.FColor(GDH.ErrColor);
            Console.WriteLine(error);
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Display a message with a color.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="c">The color of the message</param>
        public static void displayWithColor(string message, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.Write(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Ask for a password.
        /// </summary>
        /// <returns></returns>
        public static string askPassword()
        {
            char hidden = '*';
            string remove = "\b \b";
            string password = string.Empty;
            bool isBack = false;
            ConsoleKeyInfo key = Console.ReadKey(true);

            while (key.Key != ConsoleKey.Enter)
            {
                isBack = key.Key == ConsoleKey.Backspace;

                if (isBack)
                {
                    Console.Write(remove);
                    password = password.Remove(password.Length - 1);
                }
                else
                {
                    password += key.KeyChar;
                    Console.Write(hidden);
                }

                key = Console.ReadKey(true);
            }

            Console.WriteLine();
            return password;
        }

        /// <summary>
        /// Display a confirmation message.
        /// </summary>
        /// <param name="message">The message</param>
        public static void displayConfirmation(string message)
        {
            GDH.FColor(GDH.ConfirmColor);
            Console.WriteLine();
            Console.WriteLine("> " + message);
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Display a usage message.
        /// </summary>
        /// <param name="usage">The usage of the command</param>
        /// <param name="description"The description of the command</param>
        public static void displayUsage(string usage, string description)
        {
            Console.WriteLine(bigSpace + "> Usage: " + usage);
            Console.WriteLine(bigSpace + "  " + description);
            Console.WriteLine();
        }
    }
}
