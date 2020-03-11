using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using IronJournal.Models;
using IronJournal.Services;
using IronJournal.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IronJournal.Pages.Lists
{
    [Authorize]
    public partial class CCListsPage
    {
    }
}