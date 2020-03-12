using System;
using System.Linq;
using System.Threading.Tasks;
using IronJournal.Util;
using IronJournal.Services;
using System.Collections.Generic;
using IronJournal.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;

namespace IronJournal.ViewModels.Lists
{
    public class AddListPageModel : BaseViewModel<int>
    {
        readonly IDataService _dataService;
        readonly IDialogs _dialogs;
        readonly NavigationManager _navigation;

        FactionModel[] factions;
        ModelModel[] casters;

        int? listIndex ;

        public string Name { get; set; }

        public string ConflictChamberLink { get; set; }

        public Dictionary<string, string> FactionOptions { get; private set; }
        public string SelectedFactionId { get; set; }

        public Dictionary<string, string> CasterOptions { get; private set; }
        public string SelectedCasterId { get; set; }

        public int PointsLevel { get; set; }

        public string Theme { get; set;}

        public AddListPageModel(IDataService dataService, IDialogs dialogs, NavigationManager  navigation)
        {
            _dataService = dataService;
            _dialogs = dialogs;
            _navigation = navigation;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            IsLoading = true;

            this.factions = await this._dataService.GetFactions();
            this.casters = await this._dataService.GetModels(typeIds: new int[] {2, 7, 15 } );

            this.FactionOptions = this.factions.ToDictionary(x => x.Id.ToString(), x => x.Name);

            IsLoading = false;
        }

        public override void Prepare(int param)
        {
            this.listIndex = param;
        }

        public async Task OnSave()
        {
            try 
            {
                // todo:
                int.TryParse(SelectedFactionId, out int factionId);
                int.TryParse(SelectedCasterId, out int casterId);
                var gameModel = new CCListItem
                {
                    ListId = CCHelper.GetListId(this.ConflictChamberLink),
                    Name = this.Name,
                    Url = this.ConflictChamberLink,

                    FactionId = factionId,
                    CasterId = casterId,
                    Points =this.PointsLevel,
                    Theme = this.Theme
                };

                await this._dataService.CreateList(this.listIndex ?? 0, gameModel);

                _navigation.NavigateTo("/cc-lists");
            }
            catch(Exception ex)
            {
                await this._dialogs.ShowAlert("Error while saving list. Save was not successful");
            }
        }

        async void OnConflictChamberLinkChanged()
        {
            if (Uri.TryCreate(this.ConflictChamberLink, UriKind.RelativeOrAbsolute, out Uri link))
            {
                var listId = CCHelper.GetListId(link);

                var listInfo = await _dataService.GetConflictChamberList(listId);

                if (listInfo == null)
                {
                    await _dialogs.ShowAlert("Unable to load Conflict Chamber list");
                    return;
                }
                else if (listInfo.Lists.Length > 1)
                {
                    await _dialogs.ShowAlert("Lists with only 1 caster are currently supported. Please enter a list url with only 1 caster.");
                    return;
                }
                else
                {
                    var faction = this.factions.FirstOrDefault(x => x.Name == listInfo.Faction || x.Short == listInfo.Faction);
                    this.SelectedFactionId = faction?.Id.ToString();

                    var caster = listInfo.Lists[0].Caster;
                    this.SelectedCasterId = this.casters?.FirstOrDefault(x => x.Name == caster || x.Alias == caster)?.Id.ToString();

                    this.Theme = listInfo.Lists[0].Theme;

                    this.PointsLevel = listInfo.Lists[0].Points;
                }
            }
        }

        void OnSelectedFactionIdChanged()
        {
            Console.WriteLine("faction changed: " + SelectedFactionId);
            if (!int.TryParse(SelectedFactionId, out int fid))
            {
                this.CasterOptions = null;
            }
            else
            {
                this.CasterOptions = this.casters.Where(x => x.FactionId == fid).ToDictionary(x => x.Id.ToString(), x => x.Name);
            }
        }
    }
}