using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace cAPParel.ConsoleClient.Helpers
{
    public static class QueryStringBuilder
    {
        public static string BuildQueryString(params (string key, object? value)[] parameters)
        {
            var encodedParameters = parameters
                .Where(p => p.value != null)
                .Select(p => $"{HttpUtility.UrlEncode(p.key)}={HttpUtility.UrlEncode(p.value.ToString())}");

            return string.Join("&", encodedParameters);
        }
    }
}
