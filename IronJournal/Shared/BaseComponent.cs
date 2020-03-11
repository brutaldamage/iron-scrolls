using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronJournal.Shared
{
    public abstract class BaseComponent<TViewModel> : ComponentBase
            where TViewModel : ViewModels.BaseViewModel
    {
        [Inject]
        public virtual TViewModel ViewModel { get; set; }        

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                if (ViewModel == null)
                    throw new ArgumentNullException(nameof(ViewModel));

                this.ViewModel.PropertyChanged += OnPropertyChanged;

                await ViewModel?.InitializeAsync();
            }
        }

        // don't love this, but not sure how else to handle it
        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.StateHasChanged();
        }
    }

   public abstract class BaseComponent<TParam, TViewModel> : BaseComponent<TViewModel> 
        where TViewModel : ViewModels.BaseViewModel<TParam>
    {
        [Parameter]
        public virtual TParam Parameter { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if(firstRender)
                this.ViewModel?.Prepare(this.Parameter);
        }
    }

    public abstract class BaseComponent<TParam1, TParam2, TViewModel> : BaseComponent<TViewModel>
     where TViewModel : ViewModels.BaseViewModel<TParam1, TParam2>
    {
        [Parameter]
        public virtual TParam1 Parameter1 { get; set; }

        [Parameter]
        public virtual TParam2 Parameter2 { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                this.ViewModel?.Prepare(this.Parameter1, this.Parameter2);
            }

            base.OnAfterRender(firstRender);
        }
    }
}