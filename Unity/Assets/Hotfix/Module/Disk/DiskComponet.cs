using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{ 
    public class DiskComponet: Component
    {
        private List<string> bigIcons = new List<string>();
        private List<string> middlesIcons = new List<string>();
        private List<string> smallsIcons = new List<string>();

        public void Clear()
        {
            bigIcons.Clear();
            middlesIcons.Clear();
            smallsIcons.Clear();
        }

        public void AddBigIcon(string icon)
        {
            bigIcons.Add(icon);
        }
        public void AddMiddleIcon(string icon)
        {
            middlesIcons.Add(icon);
        }
        public void AddSmallIcon(string icon)
        {
            smallsIcons.Add(icon);
        }

        public List<string> GetBigIcons()
        {
            return this.bigIcons;
        }
        public List<string> GetMiddleIcons()
        {
            return this.middlesIcons;
        }

        public List<string> GetSmallIcons()
        {
            return this.smallsIcons;
        }
    }
}
