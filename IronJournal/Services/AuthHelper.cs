using System;
using System.Threading;
using System.Threading.Tasks;
using IronJournal.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace IronJournal.Services
{
    public class AuthHelper : IAuthHelper
    {
        readonly NavigationManager _navigation;
        readonly ICurrentUser _currentUser;
        readonly IDataService _dataService;

        TaskCompletionSource<bool> signinCompleted;

        public AuthHelper(NavigationManager navigation, ICurrentUser currentUser, IDataService dataService)
        {
            _navigation = navigation;
            _currentUser = currentUser;
            _dataService = dataService;

            signinCompleted = new TaskCompletionSource<bool>();
            this.InitializeTask = signinCompleted.Task;
        }

        public Task InitializeTask { get; }

        [JSInvokable]
        public async Task OnSignInCompleted(UserModel user)
        {
            Console.WriteLine("user signin completed");

            this._currentUser.SetCurrentUser(user);
            await this._dataService.UpdatePlayerInfo(new PlayerInfoModel
            {
                Uid = user.Uid,
                Name = user.DisplayName,
                PhotoUrl = user.PhotoUrl
            });
        }

        [JSInvokable]
        public async Task OnSignOutCompleted()
        {
            Console.WriteLine("on user signout completed");

            this._currentUser.SetCurrentUser(null);

            await Task.CompletedTask;
        }
    }
}