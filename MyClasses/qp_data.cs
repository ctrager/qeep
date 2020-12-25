using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace qeep
{

    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class qp_data
    {
        Dictionary<string, string> users_dict = new Dictionary<string, string>();

        Object my_lock = new object();

        public void load_users()
        {
            string data_folder = qp_config.get(qp_config.DataFolder);

            var lines = File.ReadLines(data_folder + "/users.txt");

            foreach (string s in lines)
            {
                string line = s.Trim();
                if (line.StartsWith("#") || string.IsNullOrEmpty(line))
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

            string data_folder = qp_config.get(qp_config.DataFolder);
            string path = data_folder + "/" + username + "/metadata.txt";

            lock (my_lock)
            {
                return System.IO.File.Exists(path);
            }
        }

        public NoteData read_note_data(string username)
        {
            string data_folder = qp_config.get(qp_config.DataFolder);
            string path = data_folder + "/" + username + "/metadata.txt";

            lock (my_lock)
            {
                string json = System.IO.File.ReadAllText(path);
                return JsonConvert.DeserializeObject<NoteData>(json);
            }

        }

        public void save_note_data(string username, NoteData note_data)
        {
            string data_folder = qp_config.get(qp_config.DataFolder);

            // It would be bad if multiple threads were writing to the 
            // file system at the same time. 

            lock (my_lock)
            {
                string dir = data_folder + "/" + username;
                string[] files = System.IO.Directory.GetFiles(dir, "note*");
                var file_dict = new Dictionary<string, bool>();
                foreach (string file in files)
                {
                    // qp_util.log(file);
                    file_dict[file] = false; // false means delete, don't keep
                }

                string path = dir + "/metadata.txt";

                var json = JsonConvert.SerializeObject(note_data, Formatting.Indented);
                qp_util.log(json);
                System.IO.File.WriteAllText(path, json);

                foreach (Note note in note_data.notes)
                {
                    string note_path = dir + "/note_" + note.id;
                    System.IO.File.WriteAllText(note_path, note.text);
                    file_dict[note_path] = true; // keep
                }

                foreach (string file in file_dict.Keys)
                {
                    if (!file_dict[file])
                    {
                        System.IO.File.Delete(file);
                    }
                }

                // also commit change to git, so that we have history, data recovery
                qp_util.run_command("git", "add .");
                qp_util.run_command("git", "commit -m \""
                    + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd_HHmmss_fff")
                    + "\"");
                qp_util.run_command("git", "diff HEAD^ HEAD");
            }
        }


    } // end class
}