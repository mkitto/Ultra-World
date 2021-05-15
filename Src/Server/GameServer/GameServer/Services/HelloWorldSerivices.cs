using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class HelloWorldSerivices:Singleton<HelloWorldSerivices>
    {
        public void Init()
        {

        }

        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance
                .Subscribe<FirstTexstRequest>(this.OnFirstTestRequest);
        }

        public void Stop()
        {

        }

        void OnFirstTestRequest(NetConnection<NetSession> sender, FirstTexstRequest request)
        {
            Log.InfoFormat("FirstTexstRequest: Helloworld:{0} ",request.Helloworld);
        }
    }
}
