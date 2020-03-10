using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IronJournal.Util
{
    public interface IDialogs
    {
        Task<string> ShowPrompt(string message, string defaultValue = null);

        Task ShowAlert(string message);

        Task<bool> ShowConfirm(string message);
    }


    public class Dialogs : IDialogs
    {
        private readonly IJSRuntime _jsruntime;

        public Dialogs(IJSRuntime jSRuntime)
        {
            _jsruntime = jSRuntime;
        }

        public async Task<string> ShowPrompt(string message, string defaultValue = null)
        {
            List<object> args = new List<object> { message };
            if (!string.IsNullOrWhiteSpace(defaultValue))
                args.Add(defaultValue);

            var value = await _jsruntime.InvokeAsync<string>("prompt", args);

            return value;
        }

        public async Task ShowAlert(string message)
        {
            await _jsruntime.InvokeVoidAsync("alert", message);
        }

        public async Task<bool> ShowConfirm(string message)
        {
            return await _jsruntime.InvokeAsync<bool>("confirm", message);
        }
    }
}