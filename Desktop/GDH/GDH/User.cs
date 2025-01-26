using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SQLite;
using System.Runtime.CompilerServices;

namespace GDH
{
    public class User
    {
        private string _username;
        private int _permissionLevel;

        public string Username { get => _username; }
        public int PermissionLevel {
            get { return _permissionLevel; }
            set { _permissionLevel = value; }
        }

        public User(string username, string password)
        {
            GetAcc(username, password);
        }

        private void GetAcc(string username, string password)
        {
            if (UserAlreadyExists(username))
                SignIn(username, password);

            else
                SignUp(username, password);
        }

        private void SignUp(string username, string password)
        {
            string commandString = "INSERT INTO users(userName, password) VALUES(@username, @passwd)";

            if (string.IsNullOrEmpty(password))
                password = "gdh";

            using (SQLiteCommand command = new SQLiteCommand(commandString, SQLiteConnection.Connection))
            {

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@passwd", password);
                command.ExecuteNonQuery();

                SignIn(username, password);
            }
        }

        private void SignIn(string username, string password)
        {
            string commandString = "SELECT userName, permissions FROM users WHERE userName = @username";

            using (SQLiteCommand command = new SQLiteCommand(commandString, SQLiteConnection.Connection))
            {
                command.Parameters.AddWithValue("@username", username);

                // Si on fait un SELECT, on utilise ExecuteReader pour lire les données
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Récupérer les informations pour l'utilisateur existant
                        _username = reader.GetString(0);
                        _permissionLevel = reader.GetInt32(1);
                    }
                }
            }
        }

        /// <summary>
        /// Check if an user already exists in the database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool UserAlreadyExists(string username)
        {
            using (SQLiteCommand command = new SQLiteCommand("SELECT id FROM users WHERE userName = @username", SQLiteConnection.Connection))
            {
                command.Parameters.AddWithValue("@username", username);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        /// <summary>
        /// Change the password of an user in the database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="newPassword"></param>
        public static void ChangePassword(string username, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                newPassword = "gdh";
            }

            using (SQLiteCommand command = new SQLiteCommand("UPDATE users SET password = @newPassword WHERE userName = @username", SQLiteConnection.Connection))
            {
                command.Parameters.AddWithValue("@newPassword", newPassword);
                command.Parameters.AddWithValue("@username", username);
                command.ExecuteNonQuery();
            }
        }
    }
}
