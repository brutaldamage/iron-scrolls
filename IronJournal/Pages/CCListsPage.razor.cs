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

                var firebaseClient = GetFirebaseClient();
                var lists = await firebaseClient
                    .Child("users")
                    .Child(user.Uid)
                    .Child("cc_lists")
                    .OnceSingleAsync<Models.CCList[]>();
                // .OnceAsync<Models.CCList>();

                List<Models.CCListWrapper> modelWrappers = new List<Models.CCListWrapper>();
                foreach (var list in lists)
                {
                    modelWrappers.Add(await GetListDetails(list));
                }

                this.ConflictChamberLists = new List<Models.CCListWrapper>(modelWrappers);
                this.StateHasChanged();
            }
        }

        private async Task<CCListWrapper> GetListDetails(CCList list)
        {
            var details = await this.DataService.GetConflictChamberList(list.ListId);

            var item = new CCListWrapper
            {
                // todo: internal info
                Model = list,
                ConflictChamberData = details
            };

            return item;
        }

        async Task DeleteList(CCListWrapper model)
        {
            if (await Dialogs.ShowConfirm("Are you sure you want to delete this list?"))
            {
                var index = this.ConflictChamberLists.IndexOf(model);
                if (index >= 0)
                {
                    var fbClient = GetFirebaseClient();
                    var newIndex = this.ConflictChamberLists.Count;
                    await fbClient.Child("users")
                        .Child(user.Uid)
                        .Child("cc_lists")
                        .Child(index.ToString())
                        .DeleteAsync();

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
            if(string.IsNullOrEmpty(name))
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

            var newItem = new Models.CCList
            {
                Name = name,
                ListId = ccId,
                Url = link
            };

            var fbClient = GetFirebaseClient();
            var newIndex = this.ConflictChamberLists.Count;
            await fbClient.Child("users")
                .Child(user.Uid)
                .Child("cc_lists")
                .Child(newIndex.ToString())
                .PutAsync(newItem);

            var details = await GetListDetails(newItem);

            this.ConflictChamberLists.Add(details);
            this.StateHasChanged();
        }

        async Task<string> GetUserApiKey()
        {
            var user = await AuthHelper.GetCurrentUser();
            return user.ApiKey;
        }

        FirebaseClient GetFirebaseClient()
        {
            var firebaseClient = new FirebaseClient(
                    "https://iron-journal.firebaseio.com/",
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => AuthHelper.GetUserIdToken(),
                        HttpClientFactory = this.HttpClient
                    });

            return firebaseClient;
        }
    }
}