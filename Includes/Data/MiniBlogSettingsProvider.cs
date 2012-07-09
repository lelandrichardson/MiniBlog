using System.Collections.Generic;
using System.Linq;

namespace MiniBlog.Includes.Data
{
    public static class MiniBlogSettingsProvider
    {
        private static Dictionary<string, object> _settings = new Dictionary<string, object>();
        public static Dictionary<string, object> Settings
        {
            get
            {
                if(_settings.Count == 0)
                {
                    RefreshSettings();
                }
                return _settings;
            }
            private set { _settings = value; }
        }

        public static void RefreshSettings()
        {
            IEnumerable<MiniBlogSetting> settings;
            using (var db = Db.GetOpenConnection())
            {
                settings = db.Query<MiniBlogSetting>("dbo.spu_Setting_ListAll");
            }
            lock (_settings)
            {
                _settings = settings.ToDictionary(s => s.SettingName, s => s.TypedValue());
            }
        }
    }


    public class MiniBlogSetting
    {
        #region Database Fields

        public string SettingName;
        public string SettingValue;
        public char DataType;

        #endregion

        public object TypedValue()
        {
            switch (DataType)
            {
                case 'D':
                    return double.Parse(SettingValue);
                case 'B':
                    return bool.Parse(SettingValue);
                case 'I':
                    return int.Parse(SettingValue);
                default:
                    return SettingValue;
            }
        }
    }
}