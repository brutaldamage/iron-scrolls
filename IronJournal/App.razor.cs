using System;
using System.Threading.Tasks;
using IronJournal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IronJournal
{
    public partial class App
    {
        [Inject]
        public IAuthHelper AuthHelper { get;set;}

        [Inject]
        public IJSRuntime JSRuntime { get;set;}

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("app oninitialized");
            await base.OnInitializedAsync();

            var jsWrapper = DotNetObjectReference.Create(AuthHelper);
            await JSRuntime.InvokeVoidAsync("onAppInitialized", jsWrapper);
        }
    }
}