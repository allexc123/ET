using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.U2D;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIComponentAwakeSystem : AwakeSystem<UIComponent>
    {
        public override void Awake(UIComponent self)
        {
            self.Camera = Component.Global.transform.Find("UICamera").gameObject;
            self.Awake();

        }
    }

    /// <summary>
    /// 管理所有UI
    /// </summary>
    public class UIComponent : Component
    {
        public class PanelType
        {
            public Type panelType;
            public string uiPrefab;
        }

        private readonly Dictionary<UIEnum, PanelType> uiTypes = new Dictionary<UIEnum, PanelType>();

        public GameObject Camera;

        public Dictionary<string, UI> uis = new Dictionary<string, UI>();


        public void Awake()
        {
            Load();
        }

        private void Load()
        {
            uiTypes.Clear();
            List<Type> types = Game.EventSystem.GetTypes();
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UIPanelAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                UIPanelAttribute attribute = attrs[0] as UIPanelAttribute;
                if (uiTypes.ContainsKey(attribute.Panel))
                {
                    Log.Debug($"已经存在同类UI IUIBase: {attribute.Panel.ToString()}");
                    throw new Exception($"已经存在同类UI IUIBase: {attribute.Panel.ToString()}");
                }
                uiTypes.Add(attribute.Panel, new PanelType { panelType = type, uiPrefab = attribute.uiPrefab });
            }
        }

        public void OpenPanelAsync(UIEnum panel)
        {
            PanelType panelType = uiTypes[panel];
            string prefab = panelType.uiPrefab;
            if (this.uis.ContainsKey(prefab))
            {
                return;
            }
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle(prefab.StringToAB());
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset(prefab.StringToAB(), prefab);

            GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject);

            UI ui = ComponentFactory.Create<UI, string, GameObject>(prefab, gameObject, false);

            ui.AddComponent(panelType.panelType);

            Add(ui);

        }


        public void Add(UI ui)
        {
            ui.GameObject.GetComponent<Canvas>().worldCamera = this.Camera.GetComponent<Camera>();

            this.uis.Add(ui.Name, ui);
            ui.Parent = this;
        }

        public void Remove(UIEnum panel)
        {
            PanelType panelType = uiTypes[panel];
            this.Remove(panelType.uiPrefab);

        }

        public void Remove(string name)
        {
            if (!this.uis.TryGetValue(name, out UI ui))
            {
                return;
            }
            this.uis.Remove(name);
            ui.Dispose();
        }
        public UI Get(UIEnum panel)
        {
            PanelType panelType = uiTypes[panel];
            return Get(panelType.uiPrefab);
        }
        public UI Get(string name)
        {
            UI ui = null;
            this.uis.TryGetValue(name, out ui);
            return ui;
        }


        public Sprite GetSprite(String atlasName, string spriteName)
        {
            //string prefab = "Icon";
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle(atlasName.StringToAB());

            SpriteAtlas sprintAtlas = (SpriteAtlas)resourcesComponent.GetAsset(atlasName.StringToAB(), atlasName);

            return sprintAtlas.GetSprite(spriteName);


        }
    }
}