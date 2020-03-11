using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IronJournal.ViewModels
{
    public abstract class BaseViewModel : NotifyPropertyChangedBase
    {
        public bool IsSaving { get; set; }

        public bool IsLoading { get; protected set; }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }

    public abstract class BaseViewModel<TParam> : BaseViewModel
    {
        public virtual void Prepare(TParam param)
        {
        }
    }

    public abstract class BaseViewModel<TParam1, TParam2> : BaseViewModel
    {
        public virtual void Prepare(TParam1 param1, TParam2 param2)
        {
        }
    }
}