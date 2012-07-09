using System.Collections.Generic;
using System.Dynamic;

namespace MiniBlog.Includes.Data
{
    public class DynamicMiniBlogSettings : DynamicObject
    {

        /// <summary>
        /// These are global settings and are read-only in this context
        /// </summary>     
        public override bool TrySetMember
            (SetMemberBinder binder, object value)
        {
            return false;
        }

        /// <summary>
        /// When user accesses something, return the value if we have it
        /// </summary>      
        public override bool TryGetMember
            (GetMemberBinder binder, out object result)
        {
            if (MiniBlogSettingsProvider.Settings.ContainsKey(binder.Name))
            {
                result = MiniBlogSettingsProvider.Settings[binder.Name];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Settings which are delegates are not supported
        /// </summary>     
        public override bool TryInvokeMember
            (InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }


        /// <summary>
        /// Return all dynamic member names
        /// </summary>
        /// <returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return MiniBlogSettingsProvider.Settings.Keys;
        }
    }
}