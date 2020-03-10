using System.Collections.Generic;

namespace IronJournal.Models
{
    public class CCListWrapper
    {
        public CCList Model { get; set; }

        public CCInfoResponse ConflictChamberData { get;set;}
    }

    public class CCListData
    {
        public string Caster { get; set; }

        public string Theme { get; set; }

        public int Points { get; set; }
    }
}