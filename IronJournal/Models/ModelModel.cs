using Newtonsoft.Json;
using System;

namespace IronJournal.Models
{
    public class ModelModel
    {
        public int Id { get;set;}

        public string Name { get; set; }

        public string Alias { get; set;}

        public string Keyword { get; set;}

        public int ModelTypeId { get; set;}

        public int FactionId  { get; set;}
    }
}