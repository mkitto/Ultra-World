using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Services
{
    class UserSerevice:Singleton<UserSerevice>
    {
        public UserSerevice()
        {
            //创建分发器单例，执行订阅 订阅用户注册请求
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);

        }


        public void Init()
        {

        }

        //登录
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0} Pass{1}",request .User,request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

           TUser user= DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

           if (user!=null)
           {
               message.Response.userLogin.Result = Result.Failed;
               message.Response.userLogin.Errormsg = "用户不存在";
           }
           else if(user.Password!=request.Passward)
           {
               message.Response.userLogin.Result = Result.Failed;
               message.Response.userLogin.Errormsg = "密码错误";
           }
           else
           {
               sender.Session.User = user;

               //用户信息
               message.Response.userLogin.Result = Result.Success;
               message.Response.userLogin.Errormsg = "None";
               message.Response.userLogin.Userinfo = new NUserInfo();
               message.Response.userLogin.Userinfo.Id = 1;
               message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
               message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
               foreach (var c in user.Player.Characters)
               {
                   NCharacterInfo info = new NCharacterInfo();
                   info.Id = c.ID;
                   info.Name = c.Name;
                   info.Class = (CharacterClass) c.Class;
                   message.Response.userLogin.Userinfo.Player.Characters.Add(info);
               }
           }

           byte[] data = PackageHandler.PackMessage(message);
           sender.SendData(data,0,data.Length);

        }

        //注册
        void OnRegister(NetConnection<NetSession> sender,UserRegisterRequest request)
        {
            //输出日志
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            //收到消息，给客户端回发
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            //查找一下数据库有没有冲突数据
           TUser user= DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

            //填充消息
            if (user!=null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户名已存在";
            }
            else
            {
                //写入数据
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                //保存修改
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }

            //打包成数据发送到客户端
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
    }
}
