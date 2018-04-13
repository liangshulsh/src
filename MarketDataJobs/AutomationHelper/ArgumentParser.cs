using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutomationHelper
{
    public class ArgumentParser
    {
        public static IDictionary<string, string> ParseArgs(string[] args)
        {
            var pattern = new Regex("^/([^=]+)(=\"?(.*)?\"?)?$");

            var argMap = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var match = pattern.Match(arg);

                if (match.Success)
                {
                    var g = match.Groups;
                    if (g[1].Value.Contains(":"))
                    {
                        throw new ArgumentException(
                            "Command-line arguments are of the form /name=value, not /name:value");
                    }
                    argMap[g[1].Value.ToLower()] = g[3].Value;
                }
            }

            return argMap;
        }

        public static List<KeyValuePair<string, string>> ParseArgs(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                return null;
            }

            string[] fieldValues = arguments.Split(new string[] { " /" }, StringSplitOptions.RemoveEmptyEntries);

            List<KeyValuePair<string, string>> dictArguments = new List<KeyValuePair<string, string>>();

            foreach (string fieldValue in fieldValues)
            {
                string pair = fieldValue.Trim(new char[] { '/' }).Trim();
                string[] pairFields = pair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (pairFields.Count() == 1)
                {
                    dictArguments.Add(new KeyValuePair<string, string>(pairFields[0].Trim().Trim(new char[] { '\"' }), string.Empty));
                }
                else if (pairFields.Count() > 1)
                {
                    string field = pairFields[0].Trim().Trim(new char[] { '\"' });
                    string value = string.Empty;
                    for (int i = 1; i < pairFields.Count(); i++)
                    {
                        value = value + pairFields[i];
                    }

                    dictArguments.Add(new KeyValuePair<string, string>(field, value.Trim().Trim(new char[] { '\"' })));
                }
            }

            return dictArguments;
        }
    }
}
