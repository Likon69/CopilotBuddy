using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Styx.Helpers
{
    public class Arguments
    {
        private readonly Dictionary<string, Collection<string>> _args;
        private string _pendingFlag;

        internal static string[] Tokenize(string input)
        {
            var sb = new StringBuilder(input);
            bool inQuotes = false;
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '"') inQuotes = !inQuotes;
                if (sb[i] == ' ' && !inQuotes) sb[i] = '\n';
            }
            string[] tokens = sb.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < tokens.Length; j++)
                tokens[j] = UnquoteString(tokens[j]);
            return tokens;
        }

        private static string UnquoteString(string s)
        {
            int first = s.IndexOf('"');
            int last = s.LastIndexOf('"');
            while (first != last)
            {
                s = s.Remove(first, 1);
                s = s.Remove(last - 1, 1);
                first = s.IndexOf('"');
                last = s.LastIndexOf('"');
            }
            return s;
        }

        public Arguments(IEnumerable<string> arguments)
        {
            _args = new Dictionary<string, Collection<string>>(StringComparer.OrdinalIgnoreCase);
            var regex = new Regex("^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            foreach (string token in arguments)
            {
                string[] parts = regex.Split(token, 3);
                switch (parts.Length)
                {
                    case 1:
                        HandleValue(parts[0]);
                        break;
                    case 2:
                        FlushPendingFlag();
                        _pendingFlag = parts[1];
                        break;
                    case 3:
                        FlushPendingFlag();
                        string val = UnquoteString(parts[2]);
                        AddTokens(parts[1], val.Split(new char[] { ',' }));
                        break;
                }
            }
            FlushPendingFlag();
        }

        private void AddTokens(string key, IEnumerable<string> values)
        {
            foreach (string v in values)
                AddSingle(key, v);
        }

        private void FlushPendingFlag()
        {
            if (_pendingFlag != null)
            {
                AddSingleStrict(_pendingFlag, "true");
                _pendingFlag = null;
            }
        }

        private void HandleValue(string value)
        {
            if (_pendingFlag != null)
            {
                value = UnquoteString(value);
                AddSingle(_pendingFlag, value);
                _pendingFlag = null;
            }
        }

        public int Count => _args.Count;

        private void AddSingle(string key, string value)
        {
            if (!_args.ContainsKey(key))
                _args.Add(key, new Collection<string>());
            _args[key].Add(value);
        }

        private void AddSingleStrict(string key, string value)
        {
            if (!_args.ContainsKey(key))
            {
                _args.Add(key, new Collection<string>());
                _args[key].Add(value);
                return;
            }
            throw new ArgumentException(string.Format("Argument {0} has already been defined", key));
        }

        public void Remove(string key)
        {
            if (_args.ContainsKey(key))
                _args.Remove(key);
        }

        public bool IsTrue(string argument)
        {
            AssertSingleValue(argument);
            var col = this[argument];
            return col != null && col[0].Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private void AssertSingleValue(string argument)
        {
            if (this[argument] != null && this[argument].Count > 1)
                throw new ArgumentException(string.Format("{0} has been specified more than once, expecting single value", argument));
        }

        public string GetString(string argument)
        {
            AssertSingleValue(argument);
            if (this[argument] != null && !IsTrue(argument))
                return this[argument][0];
            return null;
        }

        public bool Exists(string argument)
        {
            return this[argument] != null && this[argument].Count > 0;
        }

        public int GetInt(string argument)
        {
            AssertSingleValue(argument);
            if (this[argument] != null && !IsTrue(argument))
            {
                if (!int.TryParse(this[argument][0], out int result))
                    throw new ArgumentException(string.Format("{0} is not a valid integer value.", argument));
                return result;
            }
            return 0;
        }

        public Collection<string> this[string parameter]
        {
            get
            {
                _args.TryGetValue(parameter, out Collection<string> col);
                return col;
            }
        }
    }
}
