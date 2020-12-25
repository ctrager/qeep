using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System;
using Newtonsoft.Json;

namespace keep.Pages
{

    /*
    This "IgnoreAntiforgeryToken" allows the client app to post
    */
    [IgnoreAntiforgeryToken]
    public class NotesModel : PageModel
    {

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

        kp_data db = new kp_data();

        public JsonResult OnPost([FromBody] PayloadIn request)
        {

            //kp_util.log(JsonConvert.SerializeObject(request));

            var response = new PayloadOut();

            db.load_users();

            //kp_util.log(request.username);
            // username
            if (!db.user_exists(request.username))
            {
                response.result = "failure";
                response.reason = "username not found";
                return new JsonResult(response);
            }

            // password
            if (!db.password_correct(request.username, request.password))
            {
                response.result = "failure";
                response.reason = "wrong password";
                return new JsonResult(response);
            }

            NoteData note_data = null;

            if (db.user_note_data_exists(request.username))
            {
                //kp_util.log("existing file");

                // read data from file

                note_data = db.read_note_data(request.username);

                // if client didn't change anything
                // and if client's timestamp matches server
                // then do nothing
                if (request.server_timestamp.CompareTo(request.new_timestamp) == 0
                && request.server_timestamp.CompareTo(note_data.timestamp) == 0)
                {
                    response.result = "ok";
                    response.reason = "no change";
                    // because client is going to ignore this part of response,
                    // don't bother using the bandwidth
                    response.timestamp = "";
                    response.notes = null;
                }
                else if (request.server_timestamp.CompareTo(note_data.timestamp) == 0)
                {
                    kp_util.log(JsonConvert.SerializeObject(request));
                    kp_util.log("a normal save using client data");

                    response.result = "ok";
                    response.reason = "normal save";

                    // save client data as new server data
                    note_data.timestamp = request.new_timestamp; // move forward
                    note_data.notes = (Note[])request.notes.Clone();
                    db.save_note_data(request.username, note_data);

                    // Pass client data back to client
                    response.timestamp = request.new_timestamp;
                    response.notes = request.notes;
                }
                else // client had an out-of-date timestamp because somebody else updated data
                {
                    kp_util.log(JsonConvert.SerializeObject(request));
                    kp_util.log("using server data, telling client to resync");

                    response.result = "resync";
                    response.reason = "stale client timestamp";

                    // return server data
                    response.timestamp = note_data.timestamp;
                    response.notes = note_data.notes;
                }
            }
            else
            {
                kp_util.log(JsonConvert.SerializeObject(request));
                kp_util.log("file not found, creating new file using client data");

                response.result = "ok";
                response.reason = "new file";

                // create new file and save
                note_data = new NoteData();
                note_data.timestamp = request.new_timestamp; // moving forward
                note_data.notes = (Note[])request.notes.Clone();
                db.save_note_data(request.username, note_data);

                response.timestamp = request.new_timestamp;
                response.notes = request.notes;
            }

            return new JsonResult(response);
        }
    }
}

