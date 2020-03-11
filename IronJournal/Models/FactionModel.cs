using Newtonsoft.Json;
using System;

namespace IronJournal.Models
{
    public class FactionModel
    {
        public int Id { get; set; }

        public int Sub { get; set; }

        public string Name { get; set; }

        public string Short { get; set; }
    }
}