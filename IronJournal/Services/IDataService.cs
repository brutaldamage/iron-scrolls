using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using IronJournal.Models;
using IronJournal.Models.CC;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IronJournal.Services
{
    public interface IDataService
    {
        Task<Models.CC.CCInfoResponse> GetConflictChamberList(string ccid);

        Task<CCListItem[]> GetLists();

        Task DeleteList(int index);

        Task CreateList(int index, CCListItem listModel);

        Task<Models.FactionModel[]> GetFactions();

        Task<Models.ModelModel[]> GetModels(int[] factionIds = null, int[] typeIds = null);

        Task<Models.GameModel[]> GetGames();

        Task AddGame(int index, Models.GameModel model);

        Task<Models.GameModel> GetGame(int index);

        Task<Models.ScenarioModel[]> GetScenarios();

        Task<Models.InitiativeModel[]> GetInitiatives();

        Task<Models.GameResultModel[]> GetGameResults();

        Task UpdatePlayerInfo(PlayerInfoModel info);

        Task<Models.PlayerInfoModel[]> GetPlayerInfos();
    }

    public class DataService : IDataService
    {
        private readonly Firebase.IHttpClientFactory _httpClient;
        private readonly ICurrentUser _currentUser;

        public DataService(Firebase.IHttpClientFactory httpClientFactory, ICurrentUser currentUser)
        {
            _httpClient = httpClientFactory;
            _currentUser = currentUser;
        }

        public async Task<CCListItem[]> GetLists()
        {
            var user = await this._currentUser.GetCurrentUser();
            var lists = await GetFirebaseLists(user);

            return lists;
        }

        public async Task DeleteList(int listIndx)
        {
            var user = await this._currentUser.GetCurrentUser();
            
            await DeleteFireBaseList(user, listIndx);
        }

        public async Task CreateList(int index, CCListItem list)
        {
            var user = await _currentUser.GetCurrentUser();
            var fbClient = GetFirebaseClient();

            await fbClient
                .Child("cc_lists")
                .Child(user.Uid)
                .Child(index.ToString())
                .PutAsync(list);
        }

        public async Task<Models.FactionModel[]> GetFactions()
        {
            var json = await GetHttpClient().GetStringAsync("/data/factions.json");

            return JsonConvert.DeserializeObject<FactionModel[]>(json);
        }

        public async Task<Models.ModelModel[]> GetModels(int[] factionIds = null, int[] typeIds = null)
        {
            var json = await GetHttpClient().GetStringAsync("/data/models.json");
            var models = JsonConvert.DeserializeObject<ModelModel[]>(json);

            IEnumerable<Models.ModelModel> filter = models;

            if (factionIds != null && factionIds.Length > 0)
                filter = models.Where(x => factionIds.Contains(x.FactionId));

            if (typeIds != null && typeIds.Length > 0)
                filter = models.Where(x => typeIds.Contains(x.ModelTypeId));

            return filter.ToArray();
        }

        public async Task<Models.GameModel[]> GetGames()
        {
            var user = await _currentUser.GetCurrentUser();
            var firebaseClient = GetFirebaseClient();
            var games = await firebaseClient
                .Child("games")
                .Child(user.Uid)
                .OnceSingleAsync<Models.GameModel[]>();

            return games ?? new GameModel[0];
        }

        public async Task AddGame(int index, Models.GameModel model)
        {
            var user = await _currentUser.GetCurrentUser();

            var firebaseClient = GetFirebaseClient();
            await firebaseClient
                .Child("games")
                .Child(user.Uid)
                .Child(index.ToString())
                .PutAsync(model);

        }

        public async Task<Models.GameModel> GetGame(int index)
        {
            var user = await _currentUser.GetCurrentUser();


            var firebaseClient = GetFirebaseClient();
            return await firebaseClient
                .Child("games")
                .Child(user.Uid)
                .Child(index.ToString())
                .OnceSingleAsync<GameModel>();
        }

        public async Task<Models.ScenarioModel[]> GetScenarios()
        {
            var json = await GetHttpClient().GetStringAsync("/data/scenarios.json");
            var models = JsonConvert.DeserializeObject<ScenarioModel[]>(json);

            return models;
        }

        public async Task<Models.InitiativeModel[]> GetInitiatives()
        {
            var json = await GetHttpClient().GetStringAsync("/data/initiatives.json");
            var models = JsonConvert.DeserializeObject<InitiativeModel[]>(json);

            return models;
        }

        public async Task<Models.GameResultModel[]> GetGameResults()
        {
            var json = await GetHttpClient().GetStringAsync("/data/results.json");
            var models = JsonConvert.DeserializeObject<GameResultModel[]>(json);

            return models;
        }

        public async Task UpdatePlayerInfo(PlayerInfoModel info)
        {
            var firebaseClient = GetFirebaseClient();
            await firebaseClient
                .Child("players")
                .Child(info.Uid)
                .PutAsync(info);
        }

        public async Task<PlayerInfoModel[]> GetPlayerInfos()
        {
            var firebaseClient = GetFirebaseClient();
            var players = await firebaseClient
                .Child("players")
                .OnceSingleAsync<Dictionary<string, PlayerInfoModel>>();

            return players.Select(x => x.Value).ToArray();
        }

        public async Task<CCInfoResponse> GetConflictChamberList(string ccid)
        {
            var httpClient = GetHttpClient();
            var url = $"https://api.conflictchamber.com/list/{ccid}.JSON";
            var json = await httpClient.GetStringAsync(url);

            Console.WriteLine("cc json: " + json);

            return JsonConvert.DeserializeObject<CCInfoResponse>(json);
        }


        #region Private Helper Methods

        private async Task<CCListDataModel> GetListDetails(CCListItem list)
        {
            var details = await this.GetConflictChamberList(list.ListId);

            var item = new CCListDataModel
            {
                // todo: internal info
                Model = list,
                ConflictChamberData = details
            };

            return item;
        }

        private async Task<CCListItem[]> GetFirebaseLists(UserModel user)
        {
            var firebaseClient = GetFirebaseClient();
            var lists = await firebaseClient
                .Child("cc_lists")
                .Child(user.Uid)
                .OnceSingleAsync<Models.CCListItem[]>();

            return lists;
        }

        private async Task DeleteFireBaseList(UserModel user, int index)
        {
            var fbClient = GetFirebaseClient();
            await fbClient.Child("cc_lists")
                    .Child(user.Uid)
                    .Child(index.ToString())
                    .DeleteAsync();
        }

        FirebaseClient GetFirebaseClient()
        {
            var firebaseClient = new FirebaseClient(
                    "https://iron-journal.firebaseio.com/",
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => _currentUser.GetUserIdToken(),
                        HttpClientFactory = this._httpClient
                    });

            return firebaseClient;
        }

        HttpClient GetHttpClient() => this._httpClient.GetHttpClient(null).GetHttpClient();

        #endregion
    }
}