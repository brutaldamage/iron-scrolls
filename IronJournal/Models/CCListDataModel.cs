using System.Collections.Generic;

namespace IronJournal.Models
{
    public class CCListDataModel
    {
        public CCListItem Model { get; set; }

        public CC.CCInfoResponse ConflictChamberData { get;set;}
    }
}