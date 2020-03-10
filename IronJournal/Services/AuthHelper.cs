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
         TaskCompletionSource<bool> onUserInitializedTask;

        bool? isSignedIn;

        readonly IJSRuntime _jsruntime;
        readonly NavigationManager _navigation;

        public AuthHelper(IJSRuntime jsruntime, NavigationManager navigation)
        {
            _jsruntime = jsruntime;
            _navigation = navigation;
        }

        [JSInvokable]
        public async Task OnSignInCompleted()
        {
            Console.WriteLine("user signin completed");
            this.onUserInitializedTask?.TrySetResult(true);
            isSignedIn = true;

            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task OnSignOutCompleted()
        {
            Console.WriteLine("on user signout completed");
            isSignedIn = false;
            this.onUserInitializedTask?.TrySetResult(false);

            await Task.CompletedTask;
        }

        public async Task<FirebaseUser> GetCurrentUser(CancellationToken cancellationToken = default(CancellationToken))
        {
            try 
            {
                if(!isSignedIn.HasValue) 
                {
                    this.onUserInitializedTask = new TaskCompletionSource<bool>();
                    cancellationToken.Register(() => 
                    {
                        onUserInitializedTask?.TrySetCanceled(cancellationToken);
                    });
                    // wait for user to be initialized

                    bool returnNull = false;
                    try 
                    {
                        await onUserInitializedTask.Task;
                        this.onUserInitializedTask = null;
                    }
                    catch 
                    { 
                        /* assume canceleld for now */
                        returnNull = true;
                    }
                    
                    // was cancelled
                    if(returnNull)
                    {
                        return null;
                    }
                }

                Console.WriteLine("getting current user");
                var user = await _jsruntime.InvokeAsync<FirebaseUser>("getUser");
                Console.WriteLine("got user: " + JsonConvert.SerializeObject(user));
                return user;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }
    }
}