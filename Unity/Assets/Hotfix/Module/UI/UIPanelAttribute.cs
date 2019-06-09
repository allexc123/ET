using System;

namespace ETHotfix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class UIPanelAttribute : Attribute
    {
        public UIEnum Panel;
        public string uiPrefab;
        public UIPanelAttribute(UIEnum panel, string uiPrefab)
        {
            Panel = panel;
            this.uiPrefab = uiPrefab;
        }
    }
}
