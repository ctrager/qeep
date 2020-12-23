using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.VisualBasic;
using System.Linq;

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
            public string timestamp { get; set; }
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
            public string timestamp { get; set; }
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

            kp_util.log(JsonSerializer.Serialize(request));

            var response = new PayloadOut();

            var user_data = new kp_data();
            user_data.load_users();

            kp_util.log(request.username);

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

            kp_util.log("array len: " + request.notes.Length.ToString());

            response.result = "ok";
            response.reason = "";

            string path = "data/" + request.username + "/metadata.txt";

            NoteData note_data = null;

            if (System.IO.File.Exists(path))
            {
                kp_util.log("existing file");
                // read data from file
                string json = System.IO.File.ReadAllText(path);
                note_data = JsonSerializer.Deserialize<NoteData>(json);

                kp_util.log("timestamp cl:" + request.timestamp);
                kp_util.log("timestamp sv:" + note_data.timestamp);

                if (request.timestamp.CompareTo(note_data.timestamp) > 0)
                {
                    kp_util.log("using client data");
                    note_data.timestamp = request.timestamp;
                    note_data.notes = (Note[])request.notes.Clone();
                    var client_json = JsonSerializer.Serialize(note_data);
                    System.IO.File.WriteAllText(path, client_json);
                    response.timestamp = request.timestamp;
                    response.notes = request.notes;
                }
                else
                {
                    kp_util.log("using server data");
                    response.timestamp = note_data.timestamp;
                    response.notes = note_data.notes;
                }
            }
            else
            {
                kp_util.log("new file");
                // create new file
                note_data = new NoteData();
                // save
                note_data.timestamp = request.timestamp;
                note_data.notes = (Note[])request.notes.Clone();

                var json = JsonSerializer.Serialize(note_data);
                kp_util.log(json);

                System.IO.File.WriteAllText(path, json);

                response.timestamp = request.timestamp;
                response.notes = request.notes;
            }

            return new JsonResult(response);
        }
    }
}
