using Newtonsoft.Json;
using System;

namespace IronJournal.Models
{
    public class FirebaseUser 
    {
        public string Uid { get;set;}

        public string DisplayName { get;set;}

        public string PhotoUrl { get;set;}

        public string Email {get;set;}

        public bool EmailVerified { get;set;}

        public bool IsAnonymous { get; set;}

        public string ApiKey { get;set;}

        public string LastLoginAt { get; set; }

        public DateTime? LastLoginDate {
            get {
                if(long.TryParse(this.LastLoginAt, out long ticks))
                {
return new DateTime(ticks: ticks);
                }

                return null;
            }
        }

        public string CreatedAt { get;set;}

        public DateTime? CreatedDate {
            get {
                if(long.TryParse(this.CreatedAt, out long ticks))
                {
return new DateTime(ticks: ticks);
                }

                return null;
            }
        }

        public FirebaseTokenInfo StsTokenManager { get; set; }
    }

    public class FirebaseTokenInfo
    {
        public string ApiKey { get;set;}

        public string RefreshToken {get;set;}

        public string AccessToken { get; set;}

        public long ExpirationTime { get; set; }
    }
}