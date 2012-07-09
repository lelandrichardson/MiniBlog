using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

namespace MiniBlog.Framework
{
    public class SafeExpando : DynamicObject
    {
        private readonly IDictionary<string, object> _dictionary;
        public SafeExpando()
        {
            _dictionary = new Dictionary<string, object>();
        }
        public SafeExpando(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            _dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_dictionary.TryGetValue(binder.Name, out result))
            {
                // return null to avoid exception.  caller can check for null this way...
                result = null;
                return true;
            }

            var dictionary = result as IDictionary<string, object>;
            if (dictionary != null)
            {
                result = new SafeExpando(dictionary);
                return true;
            }

            var arrayList = result as ArrayList;
            if (arrayList != null && arrayList.Count > 0)
            {
                if (arrayList[0] is IDictionary<string, object>)
                    result = new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new SafeExpando(x)));
                else
                    result = new List<object>(arrayList.Cast<object>());
            }

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!_dictionary.ContainsKey(binder.Name))
                _dictionary.Add(binder.Name, value);
            else
                _dictionary[binder.Name] = value;

            return true;
        }

        #region ToString Methods
        public HtmlString ToJson()
        {
            return new HtmlString(ToString());
        }
        public override string ToString()
        {
            var sb = new StringBuilder("{");
            ToString(sb);
            return sb.ToString();
        }

        private void ToString(StringBuilder sb)
        {
            var firstInDictionary = true;
            foreach (var pair in _dictionary)
            {
                if (!firstInDictionary)
                {
                    sb.Append(",");
                }
                firstInDictionary = false;

                ObjectToString(sb, pair.Value, pair.Key);
            }
            sb.Append("}");
        }

        private static void ObjectToString(StringBuilder sb, object value, string name = null)
        {
            if (name != null)
            {
                sb.AppendFormat("{0}:", name);
            }
            if (value is string)
            {
                sb.AppendFormat("\"{0}\"", value);
            }
            else if (value is bool)
            {
                sb.Append(value.ToString().ToLower());
            }
            else if (value is IDictionary<string, object>)
            {
                IDictionaryToString(sb, (IDictionary<string, object>)value);
            }
            else if (value is IEnumerable)
            {
                EnumerableToString(sb, (IEnumerable)value);
            }
            else
            {
                sb.Append(value);
            }
        }
        private static void EnumerableToString(StringBuilder sb, IEnumerable value)
        {
            sb.Append("[");
            var firstInArray = true;
            foreach (var arrayValue in value)
            {
                if (!firstInArray)
                {
                    sb.Append(",");
                }
                firstInArray = false;

                ObjectToString(sb, arrayValue);
            }
            sb.Append("]");
        }
        private static void IDictionaryToString(StringBuilder sb, IDictionary<string, object> value)
        {
            new SafeExpando(value).ToString(sb);
        }
        #endregion

    }



}