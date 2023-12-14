using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace cAPParel.BlazorApp.Helpers
{
    public static class QueryStringBuilder
    {
        public static string BuildQueryString(params (string key, object? value)[] parameters)
        {
            var encodedParameters = parameters
                .Where(p => p.value != null && !(p.value is string str && str.Length == 0) && !(p.value is ICollection col && col.Count == 0))
                .SelectMany(p =>
                    p.value is IEnumerable enumerableValue && !(p.value is string)
                        ? ((IEnumerable)enumerableValue).Cast<object>()
                            .Select(item => $"{HttpUtility.UrlEncode(p.key)}={HttpUtility.UrlEncode(item.ToString())}")
                        : new[] { $"{HttpUtility.UrlEncode(p.key)}={HttpUtility.UrlEncode(p.value.ToString())}" }
                );

            return string.Join("&", encodedParameters);
        }
    }
}
