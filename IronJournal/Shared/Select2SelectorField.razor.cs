using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IronJournal.Shared
{
    public partial class Select2SelectorField<TValue> : InputBase<TValue>
    {
        [Parameter] public string Id { get; set; }
        [Parameter] public string Label { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public Expression<Func<string>> ValidationFor { get; set; }
        [Parameter] public ICollection<KeyValuePair<string, string>> Datasource { get; set; }
        
        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {
            if (value == "null")
            {
                value = null;
            }
            if (typeof(TValue) == typeof(string))
            {
                result = (TValue)(object)value;
                validationErrorMessage = null;

                return true;
            }
            else if (typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?))
            {
                int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue);
                result = (TValue)(object)parsedValue;
                validationErrorMessage = null;

                return true;
            }

            throw new InvalidOperationException($"{GetType()} does not support the type '{typeof(TValue)}'.");
        }

        protected override void OnInitialized()
        {
            try
            {
                base.OnInitialized();
                
            }
            catch (Exception e)
            {

            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await base.OnAfterRenderAsync(firstRender);
                
            }
            catch (Exception e)
            {

            }
        }
    
        void OnValueChanged(string value)
        {
            if (value == "null")
            {
                value = null;
            }
            if (typeof(TValue) == typeof(string))
            {
                CurrentValue = (TValue)(object)value;
            }
            else if (typeof(TValue) == typeof(int))
            {
                int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue);
                CurrentValue = (TValue)(object)parsedValue;
            }
            else if (typeof(TValue) == typeof(int?))
            {
                if (value == null)
                {
                    CurrentValue = (TValue)(object)null;
                }
                else
                {
                    int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue);
                    CurrentValue = (TValue)(object)parsedValue;
                }
            }
        }
    }

    // need to use a child component that doesn't have a generic type for JSInterop
    // https://github.com/dotnet/aspnetcore/issues/9061#issuecomment-493774294
    public class Select2SelectorFieldChild : ComponentBase
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        public DotNetObjectReference<Select2SelectorFieldChild> DotNetRef;

        [Parameter] 
        public string Id { get; set; }

        [Parameter]
        public Action<string> OnValueChanged { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            DotNetRef = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("select2Component.init", Id);
                await JSRuntime.InvokeVoidAsync("select2Component.onChange", Id, DotNetRef, "Change_SelectWithFilterBase");
            }
        }

        [JSInvokable("Change_SelectWithFilterBase")]
        public void Change(string value)
        {
            this.OnValueChanged?.Invoke(value);
        }
    }
}
