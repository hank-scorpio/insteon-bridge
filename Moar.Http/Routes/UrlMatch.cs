using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moar.Http
{
    public class UrlMatch
    {
        public bool IsMatch { get; protected set; }
        public IDictionary<string, string> MatchedArgs { get; protected set; }
    
        public UrlMatch(bool isMatch, IDictionary<string, string> matchedArgs = null)
        {
            IsMatch = isMatch;
            MatchedArgs = matchedArgs;
        }
    }
}
