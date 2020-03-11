using System;
using System.Threading;
using System.Threading.Tasks;
using IronJournal.Models;
using Microsoft.JSInterop;

namespace IronJournal.Services
{
    public interface ICurrentUser
    {
        void SetCurrentUser(UserModel user);

        Task<UserModel> GetCurrentUser();

        Task<string> GetUserIdToken();
    }

    public class CurrentUser : ICurrentUser
    {
        readonly IJSRuntime _jsruntime;

        bool userWasSetFirstTime = false;

        UserModel user;

        TaskCompletionSource<UserModel> waitForUser;
        public CurrentUser(IJSRuntime jsruntime)
        {
            _jsruntime = jsruntime;
        }

        public void SetCurrentUser(UserModel user)
        {
            userWasSetFirstTime = true;

            this.user = user;

            if (waitForUser != null)
                waitForUser.TrySetResult(user);
        }

        public async Task<UserModel> GetCurrentUser()
        {
            var usr = this.user;

            if (!userWasSetFirstTime && this.user == null)
            {
                this.waitForUser = new TaskCompletionSource<UserModel>();
                usr = await waitForUser.Task;
            }

            return usr;
        }

        public async Task<string> GetUserIdToken()
        {
            // get the user first to make sure user object exists
            var user = await GetCurrentUser();

            // force refresh
            var wrapper = new Util.PromiseWrapper<string>();
            await _jsruntime.InvokeAsync<UserModel>("getUserIdToken", true, DotNetObjectReference.Create(wrapper));

            var idToken = await wrapper.GetResult();

            Console.WriteLine(idToken);

            return idToken.ToString();
        }
    }
}