using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IronJournal.Util
{
    public class PromiseWrapper<T> 
    {
        readonly TaskCompletionSource<T> _tcs;

        public PromiseWrapper()
        {
            _tcs = new TaskCompletionSource<T>();
        }

        public async Task<T> GetResult()
        {
            return await _tcs.Task;
        }

        [JSInvokable("onComplete")]
        public void OnComplete(T value)
        {
            _tcs?.TrySetResult(value);
        }

        [JSInvokable("onError")]
        public void OnError(string message)
        {
            _tcs.TrySetException(new Exception(message));
        }
    }
}