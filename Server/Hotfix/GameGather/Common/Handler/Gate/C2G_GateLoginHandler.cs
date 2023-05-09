using System;
using System.Collections.Generic;
using ETModel;
namespace ETHotfix {
    // 【网关服】：小区消息中转代理，帮助与它连接的所有客户端中转消息 
    [MessageHandler(AppType.Gate)]
    public class C2G_GateLoginHandler : AMRpcHandler<C2G_GateLogin, G2C_GateLogin> {

        protected override async void Run(Session session, C2G_GateLogin message, Action<G2C_GateLogin> reply) {
            G2C_GateLogin response = new G2C_GateLogin();
            try {
                long userId = Game.Scene.GetComponent<GateSessionKeyComponent>().Get(message.Key);
                
                // 添加收取Actor消息组件 并且本地化一下 就是所有服务器都能向这个对象发 并添加一个消息拦截器
                await session.AddComponent<MailBoxComponent, string>(ActorInterceptType.GateSession).AddLocation();
                // 这里，是我之前没太注意的地主。添加 MailBoxComponent, 就是说，它，当前连接的客户端，可以收发进程间消息，方便远程玩家之间聊天，“再不出牌我就要打 120 呀。。。”
                // 而消息拦截器的意思，大概是说，所以想要与我小区代理下的客户端连接的服务器，我小区的客户端消息，全部先拦截在我网关服这里，我网关服，先拦截后，再负责向下各客户端代发下发 ?
                
                // 通知GateUserComponent组件和用户服玩家上线 并获取User实体
                User user = await Game.Scene.GetComponent<GateUserComponent>().UserOnLine(userId, session.Id);
                if (user == null) {
                    response.Message = "用户信息查询不到";
                    reply(response);
                    return;
                }
                // 记录客户端session在User中
                user.AddComponent<UserClientSessionComponent>().session = session;
                // 给Session组件添加下线监听组件和添加User实体
                session.AddComponent<SessionUserComponent>().user= user;
                // 返回客户端User信息和 当前服务器时间
                response.User = user;
                response.ServerTime = TimeTool.GetCurrenTimeStamp();
                reply(response);
            }
            catch (Exception e) {
                ReplyError(response, e, reply);
            }
        }
        public bool tsasdc(User s) {
            return true;
        }
    }
}