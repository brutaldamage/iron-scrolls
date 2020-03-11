using System.Threading;
using System.Threading.Tasks;
using IronJournal.Models;
using Microsoft.JSInterop;

namespace IronJournal.Services
{
    public interface IAuthHelper
    {
        Task OnSignInCompleted();

        Task OnSignOutCompleted();

        Task<UserModel> GetCurrentUser(CancellationToken cancellation = default(CancellationToken));

        Task<string> GetUserIdToken();
    }
}