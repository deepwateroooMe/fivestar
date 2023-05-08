using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if !ILRuntime
using System.Reflection;
#endif

namespace ETModel {
    public sealed class Hotfix: Object {

#if ILRuntime
        private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
        private MemoryStream dllStream;
        private MemoryStream pdbStream;
#else
        private Assembly assembly;
#endif
        private IStaticMethod start;
        private List<Type> hotfixTypes;
        public Action Update;
        public Action LateUpdate;
        public Action OnApplicationQuit;

        public void GotoHotfix() {
#if ILRuntime
            ILHelper.InitILRuntime(this.appDomain);
#endif
            this.start.Run();
        }
        public void SetHotfixTypes(List<Type> types) {
            this.hotfixTypes = types;
        }
        public List<Type> GetHotfixTypes() {
            return this.hotfixTypes;
        }

        // 加载热更新程序集
		public void LoadHotfixAssembly() {
            // 0.加载打包的代码资源包，内含热更新代码程序集dll动态链接库，对应的路径：Assets\Res\Code
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"code.unity3d");
            // 1.从加载的AssetBundle资源中获取代码资源并转化成游戏对象
			GameObject code = (GameObject)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("code.unity3d", "Code");
			
            // 2.从游戏对象上获取对应的动态链接库和程序数据库资源转化成字节
			byte[] assBytes = code.Get<TextAsset>("Hotfix.dll").bytes;
			byte[] pdbBytes = code.Get<TextAsset>("Hotfix.pdb").bytes;

#if ILRuntime
            // 因为设置了ILRuntime的宏，所以会进入到这里，这意味着热更新模式运行游戏
            Log.Debug($"当前使用的是ILRuntime模式");
            // 3.获取热更库的环境域，这个属于ILRuntime的知识了
            this.appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            // 4.把动态链接库库和PDB（Program Database File，程序数据库文件）加入内存
            this.dllStream = new MemoryStream(assBytes);
			this.pdbStream = new MemoryStream(pdbBytes);
            // 5.通过内存加载上面的资源
			this.appDomain.LoadAssembly(this.dllStream, this.pdbStream, new Mono.Cecil.Pdb.PdbReaderProvider());
            // 6.热更代码的启动方法，直接定位到ETHotfix.Init类下的启动方法
            this.start = new ILStaticMethod(this.appDomain, "ETHotfix.Init", "Start", 0); // 这里设置了值，实际是，进入到热更新程序域里去执行，相应方法 
			// 7.热更类型通过反射
			this.hotfixTypes = this.appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
#else
			Log.Debug($"当前使用的是Mono模式");

			this.assembly = Assembly.Load(assBytes, pdbBytes);

			Type hotfixInit = this.assembly.GetType("ETHotfix.Init");
			this.start = new MonoStaticMethod(hotfixInit, "Start");
			
			this.hotfixTypes = this.assembly.GetTypes().ToList();
#endif
			// 8.秉承过河拆桥的原则，呸，优化内存的原则，卸载AssetBundle资源
			Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"code.unity3d");
		}
    }
}