using System;
using System.Threading.Tasks;
using IronJournal.Util;
using IronJournal.Services;
using System.Collections.Generic;
using IronJournal.Models;

namespace IronJournal.ViewModels.Lists
{
    public class CCListsPageModel : BaseViewModel
    {
        private readonly IDialogs _dialogs;
        private readonly IDataService _dataService;
        private readonly IAuthHelper _authHelper;
        private FirebaseUser user;

        public List<Models.CCListWrapper> ConflictChamberLists { get; private set; }

        public CCListsPageModel(IDialogs dialogs, IDataService dataService, IAuthHelper authHelper)
        {
            _dialogs = dialogs;
            _dataService = dataService;
            _authHelper = authHelper;
        }

        public override async Task InitializeAsync()
        {
            this.user = await _authHelper.GetCurrentUser();

            var lists = await _dataService.GetLists();

            this.ConflictChamberLists = new List<Models.CCListWrapper>(lists);
        }

        public async Task DeleteList(CCListWrapper item)
        {
            if (await _dialogs.ShowConfirm("Are you sure you want to delete this list?"))
            {
                var index = this.ConflictChamberLists.IndexOf(item);
                if (index >= 0)
                {
                    await _dataService.DeleteList(item);

                    this.ConflictChamberLists.RemoveAt(index);
                    RaisePropertyChanged(nameof(ConflictChamberLists));
                }
            }
        }

        public async Task AddNewList()
        {
            var link = await _dialogs.ShowPrompt("Enter a conflict chamber list");
            if (string.IsNullOrEmpty(link))
                return;

            var name = await _dialogs.ShowPrompt("Enter a name for this list");
            if (string.IsNullOrEmpty(name))
                return;


            string ccId = null;
            Uri uri = default(Uri);
            try
            {
                uri = new Uri(link);
                var pathAndQuery = uri.PathAndQuery;
                ccId = pathAndQuery.Substring(2);
            }
            catch (Exception)
            {
                await _dialogs.ShowAlert("Link entered was invalid.");
                return;
            }

            // new index is whatever the "count" of the list is, from 0 based indexing
            var created = await _dataService.CreateList(name, ccId, this.ConflictChamberLists.Count);

            this.ConflictChamberLists.Add(created);
            RaisePropertyChanged(nameof(ConflictChamberLists));
        }
    }
}