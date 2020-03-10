using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IronJournal.Pages.Account
{
	public partial class AccountPage 
	{
		[Parameter]
		public string Path {get;set;}

		[Inject]
		public IJSRuntime JSRuntime { get; set; }

		[Inject]
		public Services.IAuthHelper AuthHelper { get; set; }

		[Inject]
		public NavigationManager Navigation { get; set; }

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);

			Console.WriteLine("account login - onafterreadner");
			Console.WriteLine($"account login path - {Path}");
// root login page
			if(Path == "Login")
			{
				await JSRuntime.InvokeVoidAsync("doLogin", "#firebaseui-auth-container");
			}
			if(Path == "login-success")
			{
				 Navigation.NavigateTo("/", forceLoad: true);
			}
			else if(Path == "Logout")
			{
				await JSRuntime.InvokeVoidAsync("doLogout");
			}
			else 
			{
				// await AuthHelper.OnSignInCompleted();
        		// Navigation.NavigateTo("/");
			}

		}
	}
}