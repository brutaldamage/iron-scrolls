using IronJournal.Models;
using IronJournal.Services;
using IronJournal.Util;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronJournal.ViewModels.Games
{
    public class AddGamePageModel : BaseViewModel
    {
        readonly IDataService _dataService;
        readonly NavigationManager _navigation;
        private readonly IDialogs _dialogs;
        private CCListDataModel[] lists;
        private ScenarioModel[] scenarios;
        private InitiativeModel[] initiatives;
        private GameResultModel[] gameResults;

        public Dictionary<string, string> ListOptions { get; private set; }
        public string SelectedListId { get; set; }

        public Dictionary<string, string> ScenarioOptions { get; private set; }
        public string SelectedScenarioId { get; set; }

        public Dictionary<string, string> InitiativeOptions { get; private set; }
        public string SelectedInitiativeId { get; set; }

        public Dictionary<string, string> GameResultOptions { get; private set; }
        public string SelectedGameResultId { get; set; }

        public string OpponentsListUrl { get; set; }

        public Models.CC.CCInfoResponse OpponentsConflictChamberList { get; set; }

        public DateTime Date { get; set; }

        public string Comments { get; set; }

        public string Opponent { get; set;}

        public int ControlPoints { get; set; }

        public int OpponentControlPoints { get; set; }

        public AddGamePageModel(IDataService dataService, IDialogs dialogs, NavigationManager navigation)
        {
            _dataService = dataService;
            _navigation = navigation;
            _dialogs = dialogs;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            IsLoading = true;

            this.lists = await _dataService.GetLists();
            this.scenarios = await _dataService.GetScenarios();
            this.initiatives = await _dataService.GetInitiatives();
            this.gameResults = await _dataService.GetGameResults();

            // assume only 1 caster in list, that's all that's supported right now
            this.ListOptions = this.lists.ToDictionary(x => x.Model.ListId, x => $"{x.Model.Name} | {x.ConflictChamberData.Faction}, {x.ConflictChamberData.Lists[0].Caster}");
            this.ScenarioOptions = this.scenarios.ToDictionary(x => x.Id.ToString(), x => x.Name);
            this.InitiativeOptions = this.initiatives.ToDictionary(x => x.Id.ToString(), x => x.Name);
            this.GameResultOptions = this.gameResults.ToDictionary(x => x.Id.ToString(), x => x.Name);

            this.Date = DateTime.Today;

            // var factions = await this._dataService.GetFactions();
            IsLoading = false;
        }


        public async Task OnSave()
        {
            var model = new GameModel
            {
                Date = this.Date,
                ListId = this.SelectedListId,
                ScenarioId = string.IsNullOrEmpty(this.SelectedScenarioId) ? null : (int?)int.Parse(this.SelectedScenarioId),
                InitiativeId = string.IsNullOrEmpty(this.SelectedInitiativeId) ? null : (int?)int.Parse(this.SelectedInitiativeId),
                GameResultId = string.IsNullOrEmpty(this.SelectedGameResultId) ? null : (int?)int.Parse(this.SelectedGameResultId),
                ControlPoints = this.ControlPoints,
                OpponentControlPoints = this.OpponentControlPoints,
                OpponentListId = this.OpponentsConflictChamberList?.Id,
                Comments = this.Comments,
                Opponent= this.Opponent
            };

            IsSaving = true;
            await this._dataService.AddGame(model);
            IsSaving = false;

            this._navigation.NavigateTo("/games");
        }

        async void OnOpponentsListUrlChanged()
        {
            if (!string.IsNullOrWhiteSpace(this.OpponentsListUrl))
            {
                if (Uri.TryCreate(this.OpponentsListUrl, UriKind.RelativeOrAbsolute, out Uri link))
                {
                    var ccId = Util.CCHelper.GetListId(link);

                    var ccList = await this._dataService.GetConflictChamberList(ccId);
                    if (ccList.Lists.Length > 1)
                    {
                        await this._dialogs.ShowAlert("Opponents Conflict Chamber list has more than 1 caster. Please specify a list that has only 1 caster.");
                        this.OpponentsConflictChamberList = null;
                        return;
                    }

                    this.OpponentsConflictChamberList = ccList;
                }
            }
            else
            {
                this.OpponentsConflictChamberList = null;
            }
        }
    }
}