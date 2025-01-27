using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static System.Net.Mime.MediaTypeNames;
using System.Data;

namespace GDH.database
{
    public class SQLiteConnection
    {
        public static System.Data.SQLite.SQLiteConnection Connection = new("Data Source=../../../gdh.db", true);

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public static void Start()
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
        }

        /// <summary>
        /// Get the password of an user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetPasswd(string username)
        {
            string commandString = "SELECT password FROM users WHERE userName = @username";
            using (SQLiteCommand command = new SQLiteCommand(commandString, Connection))
            {
                command.Parameters.AddWithValue("@username", username);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Delete an user in the database.
        /// </summary>
        /// <param name="username"></param>
        public static void DeleteUser(string username)
        {
            using (SQLiteCommand command = new SQLiteCommand("DELETE FROM users WHERE userName = @username", Connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.ExecuteNonQuery();
            }
        }

        public static List<List<string>> GetUsers()
        {
            List<List<string>> users = new List<List<string>>();
            List<string> userNames = new List<string>();
            List<string> permissionLevel = new List<string>();

            using (SQLiteCommand command = new SQLiteCommand("SELECT userName, permissions FROM users", Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userNames.Add(reader["userName"].ToString());
                        permissionLevel.Add(reader["permissions"].ToString());
                    }
                }
            }
            users.Add(userNames);
            users.Add(permissionLevel);

            return users;
        }
    }
}
