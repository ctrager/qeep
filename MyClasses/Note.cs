namespace keep
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
    }

    public class NoteData
    {
        public string timestamp { get; set; }
        public Note[] notes { get; set; }
    }
}