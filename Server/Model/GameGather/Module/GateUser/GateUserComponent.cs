using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using ETHotfix;
using ETModel;
namespace ETModel {

    [ObjectSystem]
    public class GateUserComponentAwakeSystem : AwakeSystem<GateUserComponent> {
        public override void Awake(GateUserComponent self) {
            self.Awake();
        }
    }
    public class GateUserComponent : Component {

        public readonly Dictionary<long, User> mUserDic = new Dictionary<long, User>();
        private Session userSession;  // 用户【登录验证】服？
        private Session matchSession; // 服务器各司其责，Match 有专用的匹配服务器，就得有，至少是小区代理网关服，与匹配服之间的会话框，方便通话

        public Session UserSession {
            get {
                if (userSession == null) {
                    userSession = Game.Scene.GetComponent<NetInnerSessionComponent>().Get(AppType.User);
                }
                return userSession;
            }
        }
        public Session MatchSession {
            get {
                if (matchSession == null) {
                    matchSession = Game.Scene.GetComponent<NetInnerSessionComponent>().Get(AppType.Match);
                }
                return matchSession;
            }
        }
        public static GateUserComponent Ins { private set; get; }
        public void Awake() {
            Ins = this;
        }
    }
}
