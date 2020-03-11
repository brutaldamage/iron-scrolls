using System;
using System.Linq;
using System.Threading.Tasks;
using IronJournal.Util;
using IronJournal.Services;
using System.Collections.Generic;
using IronJournal.Models;

namespace IronJournal.ViewModels.Players
{
    public class PlayerListPageModel : BaseViewModel
    {
        readonly IDataService _dataService;

        public List<PlayerInfoModel> Players { get; } = new List<PlayerInfoModel>();

        public PlayerListPageModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            IsLoading = true;

            try
            {
                var players = (await this._dataService.GetPlayerInfos()) ?? new PlayerInfoModel[0];

                this.Players.AddRange(players);
                RaisePropertyChanged(nameof(this.Players));
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("error loading players: " + ex.Message);
            }

            IsLoading = false;
        }
    }
}