namespace IronJournal.Models
{ 
    public class CCListItem 
    {
        public string ListId { get;set;}
        public string Name { get;set;}
        public string Url { get; set; }

        public int FactionId { get; set;}
        public int CasterId { get; set; }
        public int Points { get; set; }

public string Theme { get; set;}
        public string Notes { get; set; }
    }
}