using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using IronJournal.Models;
using IronJournal.Services;
using IronJournal.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IronJournal.Pages
{
    [Authorize]
    public partial class CCListsPage
    {
        private FirebaseUser user;

        [Inject]
        public Firebase.IHttpClientFactory HttpClient { get; set; }

        [Inject]
        public IAuthHelper AuthHelper { get; set; }

        [Inject]
        public IDialogs Dialogs { get; set; }

        [Inject]
        public IDataService DataService { get; set; }

        public bool IsLoading { get; private set; }

        private List<Models.CCListWrapper> ConflictChamberLists { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            this.user = await AuthHelper.GetCurrentUser();
            if (this.ConflictChamberLists == null)
            {
                var lists = await this.DataService.GetLists();

                this.ConflictChamberLists = new List<Models.CCListWrapper>(lists);
                this.StateHasChanged();
            }
        }

        async Task DeleteList(CCListWrapper model)
        {
            if (await Dialogs.ShowConfirm("Are you sure you want to delete this list?"))
            {
                var index = this.ConflictChamberLists.IndexOf(model);
                if (index >= 0)
                {
                    await DataService.DeleteList(model);

                    this.ConflictChamberLists.RemoveAt(index);
                    this.StateHasChanged();
                }
            }
        }

        async Task AddNewList()
        {
            var link = await this.Dialogs.ShowPrompt("Enter a conflict chamber list");
            if (string.IsNullOrEmpty(link))
                return;

            var name = await this.Dialogs.ShowPrompt("Enter a name for this list");
            if (string.IsNullOrEmpty(name))
                return;


            string ccId = null;
            Uri uri = default(Uri);
            try
            {
                uri = new Uri(link);
                var pathAndQuery = uri.PathAndQuery;
                Console.WriteLine("cc pq:" + pathAndQuery);
                ccId = pathAndQuery.Substring(2);
            }
            catch (Exception)
            {
                await this.Dialogs.ShowAlert("Link entered was invalid.");
                return;
            }

            // new index is whatever the "count" of the list is, from 0 based indexing
            var created = await this.DataService.CreateList(name, ccId, this.ConflictChamberLists.Count);

            this.ConflictChamberLists.Add(created);
            this.StateHasChanged();
        }
    }
}