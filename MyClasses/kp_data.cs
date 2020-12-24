using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace keep
{

    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class kp_data
    {
        public Dictionary<string, string> users_dict = new Dictionary<string, string>();

        public void load_users()
        {
            var lines = File.ReadLines("data/users.txt");
            foreach (string line in lines)
            {
                if (line.StartsWith("#"))
                {
                    continue;
                }
                int pos = line.IndexOf(":");
                string username = line.Substring(0, pos);
                string password = line.Substring(pos + 1);
                //Console.WriteLine(username + "," + password);
                users_dict[username] = password;

                if (!Directory.Exists("data/" + username))
                {
                    Directory.CreateDirectory("data/" + username);
                }
            }
        }

        public bool user_exists(string username)
        {
            return users_dict.ContainsKey(username);
        }

        public bool password_correct(string username, string password)
        {
            return users_dict[username] == password;
        }

    } // end class
}