using System;
using ETModel;
using UnityEngine;

// 【常识：】这个框架里的常识，UI 是由负责各种不同类型UI 加工厂来生产的。各个加工厂各司其责

namespace ETHotfix {
    // [UIFactory(UIType.LoginPanel)]
    public class LoginPanelFactory : IUIFactory {
        public UI Create(Scene scene, string type, GameObject gameObject) {
            try {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.BundleNameGetAsset($"{type}.unity3d", $"{type}");
                GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
                go.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI,string, GameObject>(type,go);
                ui.AddComponent<LoginPanelComponent>();
                return ui;
            }
            catch (Exception e) {
                Log.Error(e);
                return null;
            }
        }
        public void Remove(string type) {
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }
    }
}
