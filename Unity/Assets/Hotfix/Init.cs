using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ETModel;
using Google.Protobuf.Collections;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix {
    public static class Init {

        public static async ETTask<Sprite> etSprite() {
            await  ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(500);
            return null;
        }

        public async  static void Start() {
#if ILRuntime
            if (!Define.IsILRuntime) {
                Log.Error("mono层是mono模式, 但是Hotfix层是ILRuntime模式");
            }
#else
            if (Define.IsILRuntime) {
                Log.Error("mono层是ILRuntime模式, Hotfix层是mono模式");
            }
#endif
        
            try {
                // Log.Debug("游戏版本" + GameVersionsConfigMgr.HotfixLocalConfig.Version);
                // Log.Debug("游戏版本" + GameVersionsConfigMgr.Ins.LocalGameVersionsConfig.Version);
                // 注册热更层回调：什么意思呢？热更新并没有Unity 游戏主程序域里的生命周期回调方法。这里就是定义几个回调，让它与游戏主程序域里生命周期方法同步而已
                ETModel.Game.Hotfix.Update = () => { Update(); };
                ETModel.Game.Hotfix.LateUpdate = () => { LateUpdate(); };
                ETModel.Game.Hotfix.OnApplicationQuit = () => { OnApplicationQuit(); };
              
                Game.Scene.AddComponent<UIComponent>();
                Game.Scene.AddComponent<OpcodeTypeComponent>();
                Game.Scene.AddComponent<MessageDispatcherComponent>();
              
                // 加载热更配置
                ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
                Game.Scene.AddComponent<ConfigComponent>();
                ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
                // GameVersionsConfig unitConfig = (GameVersionsConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(GameVersionsConfig), 1000);
                //         Log.Debug($"config {JsonHelper.ToJson(unitConfig)}");
                // 房间配置：【房间的概念】是说，三个人四个人一桌打麻将，就把他们关一个房间里。。。
                AnnouncementConfig cardFiveStarRoom = (AnnouncementConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(AnnouncementConfig), 1);
        
                Log.Debug($"config {JsonHelper.ToJson(cardFiveStarRoom)}");
                // Game.EventSystem.Run(EventIdType.InitSceneStart);
                
                // 直接添加Session组件
                Game.Scene.AddComponent<SessionComponent>();
                // GameGather新加的组件
                Game.Scene.AddComponent<VersionsShowComponent>();// 版本号显示组件
                Game.Scene.AddComponent<KCPUseManage>();// KCP使用组件
                Game.Scene.AddComponent<UserComponent>();// 用户信息管理组件
                Game.Scene.AddComponent<ToyGameComponent>();// 游戏场景 管理组件
                Game.Scene.AddComponent<MusicSoundComponent>();// 音乐 音效组件
                Game.Scene.AddComponent<FrienCircleComponet>();// 亲友圈组件
                Game.Scene.GetComponent<ToyGameComponent>().StartGame(ToyGameId.Login); // 它说，这里是跳到了游戏的登录界面
                //  Game.Scene.GetComponent<ToyGameComponent>().StartGame(ToyGameId.CardFiveStar);
                GameObject.Find("Reporter").SetActive(ETModel.Init.IsAdministrator);// 打印日志
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        public static void Update() {
            try {
                
                Game.EventSystem.Update();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        public static void LateUpdate() {
            try {
                Game.EventSystem.LateUpdate();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        public static void OnApplicationQuit() {
            Game.Close();
        }
    }
}