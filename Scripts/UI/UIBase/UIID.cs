using System.Collections.Generic;
using Utils;

namespace UI
{
    public class UIID : Enumeration
    {
        public static UIID None = new UIID(0, nameof(None));

        #region Title
        public static UIID TitleWindow = new UIID(100, "UI/Title/TitleUI");

        #endregion

        private static Dictionary<string, UIID> _dicUiPath;

        private UIID(int id, string name) : base(id, name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            _dicUiPath ??= new Dictionary<string, UIID>();
            _dicUiPath.TryAdd(name, this);
            
        }

        public static UIID GetWindowIdByName(string name)
        {
            return _dicUiPath.ContainsKey(name) == false ? null : _dicUiPath[name];
        }
    }
}
