using System;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.UI;
namespace ETHotfix {

    // 登录面板：可以找这个面板上的【各种按钮】，再根据各按钮的调用与回调往下推进去看，自己没能理解透彻的逻辑
    
    [UIComponent(UIType.LoginPanel)]
    public class LoginPanelComponent : NormalUIView {

#region 脚本工具生成的代码
        private Button mTouristLoginBtn; // 随便输入帐号，走过场 
        private Button mWeChatLoginBtn; // 微信登录
        private Toggle mAgreeToggle; // 同意条款
        private Button mLoginBtn;    // 登录按钮
        private InputField mAccountInputField; // 帐户，随便什么字符串 EricMarryMe ！！！
        private GameObject mTestLoginParentGo; // 父控件
        
        public override void Awake() {
            base.Awake();
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            mTouristLoginBtn = rc.Get<GameObject>("TouristLoginBtn").GetComponent<Button>();
            mWeChatLoginBtn = rc.Get<GameObject>("WeChatLoginBtn").GetComponent<Button>();
            mAgreeToggle = rc.Get<GameObject>("AgreeToggle").GetComponent<Toggle>();
            mLoginBtn = rc.Get<GameObject>("LoginBtn").GetComponent<Button>();
            mAccountInputField = rc.Get<GameObject>("AccountInputField").GetComponent<InputField>();
            mTestLoginParentGo = rc.Get<GameObject>("TestLoginParentGo");
            InitPanel();
        }
#endregion

        public void InitPanel() {
            if (Application.platform== RuntimePlatform.WindowsEditor|| Application.platform == RuntimePlatform.WindowsPlayer) {
                mTestLoginParentGo.SetActive(true);
            } else {
                mTestLoginParentGo.SetActive(ETModel.Init.IsAdministrator);
            }
            mLoginBtn.Add(LoginBtnEvent);
            mTouristLoginBtn.Add(TouristLoginBtnEvent);
            mWeChatLoginBtn.Add(w);
            if (Application.isMobilePlatform && PlayerPrefs.HasKey(GlobalConstant.LoginVoucher)) {
                string loginVoucher = PlayerPrefs.GetString(GlobalConstant.LoginVoucher, string.Empty);
                if (!string.IsNullOrEmpty(loginVoucher)) {
                    Game.Scene.GetComponent<KCPUseManage>().LoginAndConnect(LoginType.Voucher, loginVoucher);// 如果记录有凭证 直接发送凭证登陆
                }
            }
        }
        private void WeChatLoginBtnEvent() { // 微信登录：逻辑 
            if (!mAgreeToggle.isOn) { // 必须先同意：第三方代理登录条款。不同意就不能玩儿
                ShowAgreeHint();
                return;
            }
            SdkCall.Ins.WeChatLoginAction = WeChatLogin;// 微信回调。不知道这里两行 55 59 在干嘛：微信SDK 没有接，就仍走了普通大众的那个走过场的登录方式
            SdkMgr.Ins.WeChatLogin();// 发起微信登陆
        }
        public void WeChatLogin(string message) {
            KCPUseManage.Ins.LoginAndConnect(LoginType.WeChat, message);
        }
        private void TouristLoginBtnEvent() {
            if (!mAgreeToggle.isOn) {
                ShowAgreeHint(); // 提示，先同意条款
                return;
            }
            Log.Debug("DOTO 发送游客登录协议账号");
        }
        public void ShowAgreeHint() {
            UIComponent.GetUiView<NormalHintPanelComponent>().ShowHintPanel("请勾选协议");
        }
        private void LoginBtnEvent() {
            try {
                if (string.IsNullOrEmpty(mAccountInputField.text)) {
                    mAccountInputField.text = "0"; // 空号也可以登录，它给配个 '0'. 主要应该是程序开发时，方便测试，不用输入
                }
                Game.Scene.GetComponent<KCPUseManage>().LoginAndConnect(LoginType.Editor, mAccountInputField.text); // 调用这里，编辑器登录模式
            }
            catch (Exception e) {
                Log.Error("登陆失败" + e);
                throw;
            }
        }
    }
}
