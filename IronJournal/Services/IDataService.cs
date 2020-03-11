using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using IronJournal.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IronJournal.Services
{
    public interface IDataService
    {
        // Task<Models.CCInfoResponse> GetConflictChamberList(string ccid);

        Task<Models.CCListWrapper[]> GetLists();

        Task DeleteList(CCListWrapper list);

        Task<Models.CCListWrapper> CreateList(string name, string ccid, int index);
    }

    public class DataService : IDataService
    {
        private readonly Firebase.IHttpClientFactory _httpClient;
        private readonly IAuthHelper _authHelper;

        public DataService(Firebase.IHttpClientFactory httpClientFactory, IAuthHelper authHelper)
        {
            _httpClient = httpClientFactory;
            _authHelper = authHelper;
        }

        public async Task<Models.CCListWrapper[]> GetLists()
        {
            var user = await this._authHelper.GetCurrentUser();
            var lists = await GetFirebaseLists(user);

            var modelWrappers = new List<CCListWrapper>();

            if (lists != null && lists.Length > 0)
            {
                foreach (var list in lists)
                {
                    modelWrappers.Add(await GetListDetails(list));
                }
            }

            return modelWrappers?.ToArray();
        }

        public async Task DeleteList(CCListWrapper list)
        {
            var user = await this._authHelper.GetCurrentUser();
            var lists = (await GetFirebaseLists(user)).ToList();

            var match = lists.FirstOrDefault(x => x.ListId == list.Model.ListId);

            if (match != null)
            {
                var index = lists.IndexOf(match);

                await DeleteFireBaseList(user, index);
            }
        }

        public async Task<Models.CCListWrapper> CreateList(string name, string ccid, int index)
        {
            var link = string.Format("https://conflictchamber.com/?{0}", ccid);
            var newItem = new Models.CCList
            {
                Name = name,
                ListId = ccid,
                Url = link
            };

            var user = await _authHelper.GetCurrentUser();

            var fbClient = GetFirebaseClient();
            await fbClient.Child("users")
                .Child(user.Uid)
                .Child("cc_lists")
                .Child(index.ToString())
                .PutAsync(newItem);

            return await GetListDetails(newItem);
        }

        private async Task<CCListWrapper> GetListDetails(CCList list)
        {
            var details = await this.GetConflictChamberList(list.ListId);

            var item = new CCListWrapper
            {
                // todo: internal info
                Model = list,
                ConflictChamberData = details
            };

            return item;
        }

        private async Task<CCList[]> GetFirebaseLists(FirebaseUser user)
        {
            var firebaseClient = GetFirebaseClient();
            var lists = await firebaseClient
                .Child("users")
                .Child(user.Uid)
                .Child("cc_lists")
                .OnceSingleAsync<Models.CCList[]>();

            return lists;
        }

        private async Task DeleteFireBaseList(FirebaseUser user, int index)
        {
            var fbClient = GetFirebaseClient();
            await fbClient.Child("users")
                                    .Child(user.Uid)
                                    .Child("cc_lists")
                                    .Child(index.ToString())
                                    .DeleteAsync();
        }

        private async Task<Models.CCInfoResponse> GetConflictChamberList(string ccid)
        {
            var httpClient = _httpClient.GetHttpClient(null).GetHttpClient();
            var url = $"https://api.conflictchamber.com/list/{ccid}.JSON";
            var json = await httpClient.GetStringAsync(url);

            Console.WriteLine("cc json: " + json);

            return JsonConvert.DeserializeObject<Models.CCInfoResponse>(json);
        }

        FirebaseClient GetFirebaseClient()
        {
            var firebaseClient = new FirebaseClient(
                    "https://iron-journal.firebaseio.com/",
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => _authHelper.GetUserIdToken(),
                        HttpClientFactory = this._httpClient
                    });

            return firebaseClient;
        }
    }
}