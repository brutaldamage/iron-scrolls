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

		Task<Models.CCListDataModel[]> GetLists();

		Task DeleteList(CCListDataModel list);

		Task<Models.CCListDataModel> CreateList(string name, string ccid, int index);

		Task<Models.FactionModel[]> GetFactions();

		Task<Models.ModelModel[]> GetModels(int? id = null, int? factionId = null, int? typeId = null);

		Task<Models.GameModel[]> GetGames();

		Task AddGame(Models.GameModel model);

		Task<Models.GameModel> GetGame(int index);

		Task<Models.ScenarioModel[]> GetScenarios();

		Task<Models.InitiativeModel[]> GetInitiatives();

		Task<Models.GameResultModel[]> GetGameResults();

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

		public async Task<Models.CCListDataModel[]> GetLists()
		{
			var user = await this._authHelper.GetCurrentUser();
			var lists = await GetFirebaseLists(user);

			var modelWrappers = new List<CCListDataModel>();

			if (lists != null && lists.Length > 0)
			{
				foreach (var list in lists)
				{
					modelWrappers.Add(await GetListDetails(list));
				}
			}

			return modelWrappers?.ToArray();
		}

		public async Task DeleteList(CCListDataModel list)
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

		public async Task<Models.CCListDataModel> CreateList(string name, string ccid, int index)
		{
			var link = string.Format("https://conflictchamber.com/?{0}", ccid);
			var newItem = new Models.CCListItem
			{
				Name = name,
				ListId = ccid,
				Url = link
			};

			var user = await _authHelper.GetCurrentUser();
			var listDetails = await GetListDetails(newItem);

			if (listDetails.ConflictChamberData.Lists.Length > 1)
			{
				throw new Util.ValidationException("This Conflict Chamber list contains more than 1 caster. Lists with only 1 caster are currently supported.");
			}

			var fbClient = GetFirebaseClient();
			await fbClient.Child("users")
				.Child(user.Uid)
				.Child("cc_lists")
				.Child(index.ToString())
				.PutAsync(newItem);

			return listDetails;
		}

		public async Task<Models.FactionModel[]> GetFactions()
		{
			var json = await GetHttpClient().GetStringAsync("/data/factions.json");

			return JsonConvert.DeserializeObject<FactionModel[]>(json);
		}

		public async Task<Models.ModelModel[]> GetModels(int? id = null, int? factionId = null, int? typeId = null)
		{
			var json = await GetHttpClient().GetStringAsync("/data/factions.json");
			var models = JsonConvert.DeserializeObject<ModelModel[]>(json);

			if (id.HasValue)
			{
				return models.Where(x => x.Id == id.Value).ToArray();
			}
			else
			{
				IEnumerable<ModelModel> filter = models;

				if (factionId.HasValue)
					filter = filter.Where(x => x.FactionId == factionId.Value);

				if (typeId.HasValue)
					filter = filter.Where(x => x.ModelType == typeId.Value);

				return filter.ToArray();
			}
		}

		public async Task<Models.GameModel[]> GetGames()
		{
			var user = await _authHelper.GetCurrentUser();
			var firebaseClient = GetFirebaseClient();
			var games = await firebaseClient
				.Child("users")
				.Child(user.Uid)
				.Child("games")
				.OnceSingleAsync<Models.GameModel[]>();

			return games ?? new GameModel[0];
		}

		public async Task AddGame(Models.GameModel model)
		{
			var user = await _authHelper.GetCurrentUser();

			// make sure games exists:
			var games = await GetGames();
			int index = 0;
			if (games != null)
			{
				index = games.Length;
			}

			var firebaseClient = GetFirebaseClient();
			await firebaseClient
				.Child("users")
				.Child(user.Uid)
				.Child("games")
				.Child(index.ToString())
				.PutAsync(model);

		}

		public async Task<Models.GameModel> GetGame(int index)
		{
			var user = await _authHelper.GetCurrentUser();


			var firebaseClient = GetFirebaseClient();
			return await firebaseClient
				.Child("users")
				.Child(user.Uid)
				.Child("games")
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
				.Child("users")
				.Child(user.Uid)
				.Child("cc_lists")
				.OnceSingleAsync<Models.CCListItem[]>();

			return lists;
		}

		private async Task DeleteFireBaseList(UserModel user, int index)
		{
			var fbClient = GetFirebaseClient();
			await fbClient.Child("users")
									.Child(user.Uid)
									.Child("cc_lists")
									.Child(index.ToString())
									.DeleteAsync();
		}

		public async Task<CCInfoResponse> GetConflictChamberList(string ccid)
		{
			var httpClient = GetHttpClient();
			var url = $"https://api.conflictchamber.com/list/{ccid}.JSON";
			var json = await httpClient.GetStringAsync(url);

			Console.WriteLine("cc json: " + json);

			return JsonConvert.DeserializeObject<CCInfoResponse>(json);
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

		HttpClient GetHttpClient() => this._httpClient.GetHttpClient(null).GetHttpClient();

		#endregion
	}
}