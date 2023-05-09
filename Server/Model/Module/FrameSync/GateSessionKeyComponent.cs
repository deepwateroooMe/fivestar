using System.Collections.Generic;
namespace ETModel {

    // 【网关服会话框】管理：管理所有与之连接的客户端的会话框，及时删除，最长存活 20 秒
    public class GateSessionKeyComponent : Component {
        private readonly Dictionary<long, long> sessionKey = new Dictionary<long, long>();
        
        public void Add(long key, long userId) {
            this.sessionKey[key] = userId;
            this.TimeoutRemoveKey(key);
        }
        public long Get(long key) {
            long userId;
            this.sessionKey.TryGetValue(key, out userId);
            return userId;
        }
        public void Remove(long key) {
            this.sessionKey.Remove(key);
        }
        private async void TimeoutRemoveKey(long key) { // 与每个客户端的会话框有效生命周期 20 秒钟，过时自动删除 
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(20000);
            this.sessionKey.Remove(key);
        }
    }
}
