using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IronJournal.Util
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}