using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            DisplayStatus("Error", GDH.ErrColor);

            Console.ResetColor();
            Console.WriteLine(error);
            Console.WriteLine();
        }

        private static void DisplayStatus(string status, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write($"{bigSpace}[{status}] ");
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
            Console.WriteLine();
            DisplayStatus("Success", GDH.ConfirmColor);
            Console.WriteLine(message);
            Console.WriteLine();
        }

        /// <summary>
        /// Display a usage message.
        /// </summary>
        /// <param name="usage">The usage of the command</param>
        /// <param name="description"The description of the command</param>
        public static void DisplayUsage(string usage, string description)
        {
            Console.WriteLine(bigSpace + "> Usage: " + usage);
            Console.WriteLine(bigSpace + "  " + description);
            Console.WriteLine();
        }
    }
}
