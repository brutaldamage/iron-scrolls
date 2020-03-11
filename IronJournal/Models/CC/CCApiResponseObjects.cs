using System.Linq;

namespace IronJournal.Models.CC
{
    public class CCInfoResponse
    {
        public string Url { get; set; }
        public string Faction { get; set; }
        public CCListInfoResponse[] Lists { get; set; }

        public string Id {
            get {
                try {
                    return Util.CCHelper.GetListId(new System.Uri(Url));
                }
                catch { return null;}
            }
        } 
    }

    public class CCListInfoResponse
    {
        public string Caster => this.Models?.FirstOrDefault(x => x.Type == "Warcaster" || x.Type == "Warlock" || x.Type == "Infernal Master")?.Name;

        public CCModelInfoResponse[] Models { get; set; }
        public string Theme { get; set; }
        public int Points { get; set; }
    }

    public class CCModelInfoResponse
    {
        public string Desc { get; set; }
        public string Name { get; set; }
        public string Cost { get; set; }
        public string Type { get; set; }

        public CCAttachedModelInfoResponse[] Attached { get; set; }
    }

    public class CCAttachedModelInfoResponse
    {
        public string Name { get; set; }
        public string Cost { get; set; }
        public string Type { get; set; }
    }
}