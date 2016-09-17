using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moar.Http
{
    public class UrlFormat
    {
        public string Format { get; protected set; }
        Regex regex;
    
        public UrlFormat(string format)
        {
            Format = format;
            regex = ToRegex(format);
        }
    
        Regex ToRegex(string format)
        {
    //if (!format.Contains('{') || !format.Contains('}')) return null;
    // string pattern = "^" + format.Replace("{", "(?<").Replace("}", ">.*)") + "$";
    
    string pattern = "^" + Regex.Replace(format, @"\{(?<name>[^\|\}]*)\|?(?<regex>[^\}]*)?\}", ReplaceArgExpr) + "$";
    return new Regex(pattern, RegexOptions.RightToLeft);;
        }
    string ReplaceArgExpr(Match m)
    {
    var name = m.Groups["name"].ToString();
    var regex = m.Groups["regex"].ToString();
    if (regex == "")
    regex = "[^/]+";
    return String.Format("(?<{0}>{1})", name, regex);
    }
    
    public UrlMatch Matches(string url)
        {
            if (regex == null) return new UrlMatch(Format == url);
            Match match = regex.Match(url);
            if (!match.Success) return new UrlMatch(false);
    
            return new UrlMatch(true, regex.GetGroupNames().Skip(1).ToDictionary(x => x, x => match.Groups[x].Value));
        }
    }
}
