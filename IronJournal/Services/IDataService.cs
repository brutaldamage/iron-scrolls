using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IronJournal.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IronJournal.Services
{
    public interface IDataService
    {
        Task<Models.CCInfoResponse> GetConflictChamberList(string ccid);
    }

    public class DataService : IDataService
    {
        private readonly HttpClient _httpClient;

        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Models.CCInfoResponse> GetConflictChamberList(string ccid)
        {
            var url = $"https://api.conflictchamber.com/list/{ccid}.JSON";
            var json = await _httpClient.GetStringAsync(url);

            Console.WriteLine("cc json: " + json);

            return JsonConvert.DeserializeObject<Models.CCInfoResponse>(json);
        }
    }
}