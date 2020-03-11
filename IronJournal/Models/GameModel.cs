using Newtonsoft.Json;
using System;

namespace IronJournal.Models
{
    public class GameModel
    {
        public string Id { get; set; }

        public string ListId { get; set; }

        public string OpponentListId { get; set; }

        public DateTime Date { get; set; }

        public int? ScenarioId { get; set; }

        public int? InitiativeId { get; set; }

        public int? GameResultId { get; set; }

        public int? ControlPoints { get; set; }

        public int? OpponentControlPoints { get; set; }

        public string Comments { get; set; }

        public string Opponent { get; set;}
    }
}