using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using IronJournal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace IronJournal.Pages
{
    [Authorize]
    public partial class CCListsPage
    {
        [Inject]
        public Firebase.IHttpClientFactory HttpClient { get;set;}
        
        [Inject]
        public IAuthHelper AuthHelper { get; set; }

        public bool IsLoading { get;private set;}

        private Models.CCList[] ConflictChamberLists { get; set;}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(this.ConflictChamberLists == null) 
            {
                   
                    var user = await AuthHelper.GetCurrentUser();
                    var firebaseClient = new FirebaseClient(
                        "https://iron-journal.firebaseio.com/",
                        new FirebaseOptions
                        {
                            AuthTokenAsyncFactory = () => AuthHelper.GetUserIdToken(),
                            HttpClientFactory = this.HttpClient
                        });

                    var lists = await firebaseClient
                        .Child("users")
                        .Child(user.Uid)
                        .Child("cc_lists")
                        .OnceSingleAsync<Models.CCList[]>();
                        // .OnceAsync<Models.CCList>();

                        this.ConflictChamberLists = lists;

                        this.StateHasChanged();
            }
        }

        async Task<string> GetUserApiKey()
        {
            var user = await AuthHelper.GetCurrentUser();
            return user.ApiKey;
        }
    }
}