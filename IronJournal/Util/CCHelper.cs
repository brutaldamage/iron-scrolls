using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IronJournal.Util
{
    public class CCHelper
    {
        public static string GetListId(string link)
        {
            if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out Uri uri))
                return GetListId(uri);

            return null;
        }

        public static string GetListId(Uri uri)
        {
            try
            {
                var pathAndQuery = uri.PathAndQuery;
                var ccId = pathAndQuery.Substring(2);

                return ccId;
            }
            catch (Exception)
            {
                throw new ValidationException("Unable to parse conflict chamber list url.");
            }
        }
    }
}