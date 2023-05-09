using System;
using System.Net;
using ETModel;
using Google.Protobuf;
using MongoDB.Bson;
namespace ETHotfix {
    // 服务器端，【Realm 注册登录服】：处理来自客户端的请求注册登录消息
    [MessageHandler(AppType.Realm)]
    public class C2R_CommonLoginHandler : AMRpcHandler<C2R_CommonLogin, R2C_CommonLogin> {

        protected override async void Run(Session session, C2R_CommonLogin message, Action<R2C_CommonLogin> reply) {
            R2C_CommonLogin response = new R2C_CommonLogin(); // 先生成一个空反应，再填内容
            try {
                // 向【用户服：用户服务器，专用服务器】验证（注册/登陆）并获得一个用户ID. 内网会话框组件，干什么呢
                Session userSession = Game.Scene.GetComponent<NetInnerSessionComponent>().Get(AppType.User); // 所以，用户服，也是一个独立的特责的服务器
                U2R_VerifyUser  u2RVerifyUser = (U2R_VerifyUser)await userSession.Call(new R2U_VerifyUser() { // 请求，验证用户登录信息【用户名＋登录密码】之类的
                        LoginType = message.LoginType,
                            PlatformType = message.PlatformType,
                            DataStr = message.DataStr,
                            // IpAddress=session.RemoteAddress.Address.ToString(),
                            });
                // 如果Message不为空 说明 验证失败
                if (!string.IsNullOrEmpty(u2RVerifyUser.Message)) {
                    response.Message = u2RVerifyUser.Message;
                    reply(response);
                    return;
                }
                // 随机分配一个Gate: 随机分配一个网关服，够熟悉了吧。。。
                StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
                IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
                Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);
                // Realm 注册登录服，向gate请求一个key,客户端可以拿着这个key连接gate
                G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() { UserId = u2RVerifyUser.UserId });
                string outerAddress = config.GetComponent<OuterConfig>().Address2;
                response.Address = outerAddress;
                response.Key = g2RGetLoginKey.Key;
                response.LoginVoucher = u2RVerifyUser.UserId.ToString() + '|' + u2RVerifyUser.Password;
                
                reply(response);
            }
            catch (Exception e) {
                ReplyError(response, e, reply);
            }
        }
    }
}