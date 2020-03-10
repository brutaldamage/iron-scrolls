using System;
using System.Net.Http;
using Firebase;

namespace IronJournal.Util
 {
     public class FirebaseHttpClientFactory : Firebase.IHttpClientFactory, Firebase.IHttpClientProxy
     {
         readonly HttpClient _httpClient;
        public FirebaseHttpClientFactory(HttpClient httpClient)
        {
            _httpClient= httpClient;
        }

         public IHttpClientProxy GetHttpClient(TimeSpan? duration) => this;

          public HttpClient GetHttpClient()
        {
            return _httpClient;
        }

        public void Dispose()
        {
            // _httpClient?.Dispose();
        }
     }
 }