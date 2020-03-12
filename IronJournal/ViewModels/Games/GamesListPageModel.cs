using IronJournal.Models.CC;
using IronJournal.Services;
using Microsoft.AspNetCore.Components;
using IronJournal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronJournal.ViewModels.Games
{
    public class GamesListPageModel : BaseViewModel
    {
        readonly IDataService _dataService;
        readonly NavigationManager _navigation;

        public List<GameListItemViewModel> Games { get; private set; }

        public GamesListPageModel(IDataService dataService, NavigationManager navigation)
        {
            _dataService = dataService;
            _navigation = navigation;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            IsLoading = true;

            try
            {
                Console.WriteLine("Loading Games");
                
                var scenarios = await this._dataService.GetScenarios();
                var results = await this._dataService.GetGameResults();
                var games = (await this._dataService.GetGames()).ToList();

                List<GameListItemViewModel> gamesItemList = new List<GameListItemViewModel>();

                foreach (var game in games)
                {
                    var opponentList = await _dataService.GetConflictChamberList(game.OpponentListId);
                    // var list = await _dataService.GetConflictChamberList(game.ListId);
                    var index = games.IndexOf(game);

                    gamesItemList.Add(new GameListItemViewModel(index, list, opponentList, game.Date, results.FirstOrDefault(x => x.Id == game.GameResultId)?.Name, scenarios.FirstOrDefault(x => x.Id == game.ScenarioId)?.Name));
                }

                this.Games = new List<GameListItemViewModel>(gamesItemList);
                Console.WriteLine("Games Loaded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Loading Games" + " " + ex.Message);
            }
            IsLoading = false;
        }

        public Task AddGame()
        {
            var index = this.Games.Count;

            _navigation.NavigateTo($"/games/edit{index}");

            return Task.CompletedTask;
        }
    }

    public class GameListItemViewModel : NotifyPropertyChangedBase
    {
        public int Index { get; }
        public DateTime Date { get; }
        public string Result { get; }
        public string Scenario { get; }
        public string Faction { get; }
        public string Caster { get; }
        public string OpponentFaction { get; }
        public string OpponentCaster { get; }

        public GameListItemViewModel(int index, CCListItem list, CCInfoResponse opponentList, DateTime date, string result, string scenario)
        {
            Index = index;
            this.Date = date;
            this.Result = result;
            this.Scenario = scenario;
            this.Faction = list.Faction;
            this.Caster = list.Lists[0].Caster;
            this.OpponentFaction = opponentList.Faction;
            this.OpponentCaster = opponentList.Lists[0].Caster;
        }
    }
}