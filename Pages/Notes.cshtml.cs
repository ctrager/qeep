using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace keep.Pages
{

    /*
    This "IgnoreAntiforgeryToken" allows the screenshot app to post
    */
    [IgnoreAntiforgeryToken]
    public class NotesModel : PageModel
    {
        public class Note
        {
            public string id { get; set; }
            public string text { get; set; }
            public int position { get; set; }
            public string color { get; set; }
            //public string timestamp { get; set; }
            public override string ToString()
            {
                return id
                + " | " + position
                + " | " + color
                + " | " + text;
            }
        };

        public class NoteData
        {
            public string timestamp { get; set; }
            public Note[] notes { get; set; }
        }

        public class PayloadIn
        {
            public string username { get; set; }
            public string password { get; set; }
            public string server_timestamp { get; set; }
            public string new_timestamp { get; set; }
            public Note[] notes { get; set; }
        }

        public class PayloadOut
        {
            public string result { get; set; }
            public string reason { get; set; }
            public string timestamp { get; set; }
            public Note[] notes { get; set; }
        }

        public JsonResult OnPost([FromBody] PayloadIn request)
        {

            //kp_util.log(JsonConvert.SerializeObject(request));

            var response = new PayloadOut();

            var user_data = new kp_data();
            user_data.load_users();

            //kp_util.log(request.username);

            // username
            if (!user_data.user_exists(request.username))
            {
                response.result = "failure";
                response.reason = "username not found";
                return new JsonResult(response);
            }

            // password
            if (!user_data.password_correct(request.username, request.password))
            {
                response.result = "failure";
                response.reason = "wrong password";
                return new JsonResult(response);
            }

            //kp_util.log("array len: " + request.notes.Length.ToString());

            response.result = "ok";
            response.reason = "";

            string path = kp_config.get(kp_config.DataFolder)
                + "/"
                + request.username
                + "/metadata.txt";

            NoteData note_data = null;

            if (System.IO.File.Exists(path))
            {
                //kp_util.log("existing file");

                // read data from file
                string json = System.IO.File.ReadAllText(path);
                note_data = JsonConvert.DeserializeObject<NoteData>(json);

                // if client didn't change anything
                // and if client's timestamp matches server
                // then do nothing
                if (request.server_timestamp.CompareTo(request.new_timestamp) == 0
                && request.server_timestamp.CompareTo(note_data.timestamp) == 0)
                {
                    // pass client data back to client
                    response.timestamp = request.new_timestamp;
                    response.notes = request.notes;
                }
                // client updated something, and client was working with data in sync with server
                else if (request.server_timestamp.CompareTo(note_data.timestamp) == 0)
                {
                    kp_util.log(JsonConvert.SerializeObject(request));

                    kp_util.log("using client data");

                    // save client data as new server data
                    note_data.timestamp = request.new_timestamp; // move forward
                    note_data.notes = (Note[])request.notes.Clone();

                    WriteToFile(path, note_data);

                    // pass client data back to client
                    response.timestamp = request.new_timestamp;
                    response.notes = request.notes;
                }
                else // client had an out-of-date timestamp because somebody else updated data
                {

                    kp_util.log("using server data, telling client to resync");
                    response.result = "resync";
                    response.timestamp = note_data.timestamp;
                    response.notes = note_data.notes;
                }
            }
            else
            {
                kp_util.log(JsonConvert.SerializeObject(request));

                kp_util.log("file not found, creating new file");
                // create new file
                note_data = new NoteData();
                // save
                note_data.timestamp = request.new_timestamp; // moving forward
                note_data.notes = (Note[])request.notes.Clone();

                WriteToFile(path, note_data);

                response.timestamp = request.new_timestamp;
                response.notes = request.notes;
            }

            return new JsonResult(response);
        }

        void WriteToFile(string path, NoteData note_data)
        {
            var json = JsonConvert.SerializeObject(note_data, Formatting.Indented);
            kp_util.log(json);
            System.IO.File.WriteAllText(path, json);

            RunCommand("git", "add .");
            RunCommand("git", "commit -m \""
                + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd_HHmmss_fff")
                + "\"");
            RunCommand("git", "diff HEAD^ HEAD");
        }

        string RunCommand(string command, string args)
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
    }
}
