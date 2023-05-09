using System;
using System.IO;
namespace ETModel {
    // 组件功能：主要是方便，客户端掉线的时候，服务器自动删除，与客户端的会话框等，及时释放系统资源
    public class SessionCallbackComponent: Component {

        public Action<Session, byte, ushort, MemoryStream> MessageCallback;
        public Action<Session> DisposeCallback;

        public override void Dispose() {
            if (this.IsDisposed) 
                return;
            Session session = this.GetParent<Session>();
            base.Dispose();
            Session session2 = this.GetParent<Session>();
            this.DisposeCallback?.Invoke(session);
        }
    }
}
