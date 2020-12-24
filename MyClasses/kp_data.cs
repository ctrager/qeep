using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

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
            string data_folder = kp_config.get(kp_config.DataFolder);

            var lines = File.ReadLines(data_folder + "/users.txt");

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

                if (!Directory.Exists(data_folder + "/" + username))
                {
                    Directory.CreateDirectory(data_folder + "/" + username);
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

        public bool user_note_data_exists(string username)
        {
            string data_folder = kp_config.get(kp_config.DataFolder);

            string path = data_folder + "/" + username + "/metadata.txt";

            return System.IO.File.Exists(path);
        }

        public NoteData read_note_data(string username)
        {
            string data_folder = kp_config.get(kp_config.DataFolder);
            string path = data_folder + "/" + username + "/metadata.txt";
            string json = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<NoteData>(json);

        }

        public void save_note_data(string username, NoteData note_data)
        {
            string data_folder = kp_config.get(kp_config.DataFolder);

            foreach (Note note in note_data.notes)
            {
                string note_path = data_folder + "/" + username + "/note_" + note.id;
                System.IO.File.WriteAllText(note_path, note.text);
            }

            string path = data_folder + "/" + username + "/metadata.txt";

            var json = JsonConvert.SerializeObject(note_data, Formatting.Indented);
            kp_util.log(json);
            System.IO.File.WriteAllText(path, json);

            run_command("git", "add .");
            run_command("git", "commit -m \""
                + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd_HHmmss_fff")
                + "\"");
            run_command("git", "diff HEAD^ HEAD");
        }

        string run_command(string command, string args)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = kp_config.get(kp_config.DataFolder),
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            kp_util.log("command line output: " + command + " " + args);
            kp_util.log(output);
            kp_util.log(error);

            process.WaitForExit();

            if (string.IsNullOrEmpty(error))
            {
                return output;
            }
            else
            {
                return error;
            }
        }

    } // end class
}