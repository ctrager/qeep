using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Data;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.WebUtilities;

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
            kp_util.log(request.username);
            kp_util.log(request.notes.Length.ToString());

            var response = new PayloadOut();

            response.result = "ok";
            response.reason = "";

            var list = new List<Note>();

            for (int i = 0; i < 3; i++)
            {
                var note = new Note();
                note.id = DateTime.Now.ToLongTimeString();
                note.color = "Cyan";
                note.text = "From Server " + i.ToString();
                note.position = 1;
                list.Add(note);
            }

            response.notes = list.ToArray();

            return new JsonResult(response);
        }
    }
}
