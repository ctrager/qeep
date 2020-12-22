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
            public override string ToString()
            {
                return id
                + " | " + position
                + " | " + color
                + " | " + text;
            }
        };

        class Payload
        {
            public string result { get; set; }
            public string error_reason { get; set; }
        }

        public JsonResult OnGet()
        {
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

            return new JsonResult(list.ToArray());
        }


        public JsonResult OnPost([FromBody] Note[] notes)
        {
            var list = new List<Note>();

            //kp_util.log("notes len:" + notes.Length.ToString());

            // foreach (Note note in notes)
            // {
            //     kp_util.log(note.ToString());
            // }

            var payload = new Payload();
            payload.result = "ok";
            payload.error_reason = notes.Length.ToString();

            return new JsonResult(payload);
        }
    }
}
