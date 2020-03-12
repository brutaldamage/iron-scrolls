using System;
using System.Linq;
using System.Threading.Tasks;
using IronJournal.Util;
using IronJournal.Services;
using System.Collections.Generic;
using IronJournal.Models;
using Microsoft.AspNetCore.Components;

namespace IronJournal.ViewModels.Lists
{
    public class CCListsPageModel : BaseViewModel
    {
        readonly IDialogs _dialogs;
        readonly IDataService _dataService;
        readonly NavigationManager _navigation;

        private UserModel user;

        public List< ListItemModel> ConflictChamberLists { get; private set; }

        public CCListsPageModel(IDialogs dialogs, IDataService dataService, NavigationManager navigation)
        {
            _dialogs = dialogs;
            _dataService = dataService;
            _navigation = navigation;
        }

        public override async Task InitializeAsync()
        {
            var lists = await _dataService.GetLists();

            var factions = await this._dataService.GetFactions();
            var casters = await this._dataService.GetModels(typeIds: new int[] { 2, 7, 15 });

            var data = lists?.Select(l => 
                new ListItemModel(
                    l, 
                    factions?.FirstOrDefault(x => x.Id == l.FactionId), 
                    casters?.FirstOrDefault(x => x.Id == l.CasterId))
                );

            this.ConflictChamberLists = new List<ListItemModel>(data ?? new ListItemModel[0]);
        }

        public async Task DeleteList(ListItemModel item)
        {
            if (await _dialogs.ShowConfirm("Are you sure you want to delete this list?"))
            {
                var index = this.ConflictChamberLists.IndexOf(item);
                if (index >= 0)
                {
                    await _dataService.DeleteList(index);

                    this.ConflictChamberLists.RemoveAt(index);
                    RaisePropertyChanged(nameof(ConflictChamberLists));
                }
            }
        }

        public Task AddNewList()
        {
            var index = this.ConflictChamberLists.Count;

            _navigation.NavigateTo($"/cc-lists/edit/{index}");

            return Task.CompletedTask;
            // try {
            //     var link = await _dialogs.ShowPrompt("Enter a conflict chamber list");
            //     if (string.IsNullOrEmpty(link))
            //         return;

            //     var name = await _dialogs.ShowPrompt("Enter a name for this list");
            //     if (string.IsNullOrEmpty(name))
            //         return;

            //     string ccId = null;
            //     Uri uri = default(Uri);
            //     try
            //     {
            //         uri = new Uri(link);
            //         ccId = CCHelper.GetListId(uri);
            //     }
            //     catch (Exception)
            //     {
            //         await _dialogs.ShowAlert("Link entered was invalid.");
            //         return;
            //     }

            //     // new index is whatever the "count" of the list is, from 0 based indexing
            //     var created = await _dataService.CreateList(name, ccId, this.ConflictChamberLists.Count);

            //     this.ConflictChamberLists.Add(created);
            //     RaisePropertyChanged(nameof(ConflictChamberLists));
            // }
            // catch(ValidationException ex)
            // {
            //     Console.WriteLine(ex.Message);
            //     await this._dialogs.ShowAlert(ex.Message);   
            // }
        }

        public class ListItemModel : NotifyPropertyChangedBase
        {
            public string ConflictChamberLink { get; }
            public string Name { get; }
            public string Faction { get; }
            public string Caster { get; }
            public int Points { get; }
            public string Theme { get; }

            public ListItemModel(CCListItem item, FactionModel faction, ModelModel caster)
            {
                ConflictChamberLink = item.Url;
                Name = item.Name;
                Faction = faction?.Name;
                Caster = caster?.Name;
                Points = item.Points;
                Theme = item.Theme;
            }
        }
    }
}
