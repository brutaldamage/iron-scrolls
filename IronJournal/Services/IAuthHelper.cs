using System.Threading;
using System.Threading.Tasks;
using IronJournal.Models;
using Microsoft.JSInterop;

namespace IronJournal.Services
{
    public interface IAuthHelper
    {
        Task OnSignInCompleted(UserModel user);

        Task OnSignOutCompleted();
    }
}