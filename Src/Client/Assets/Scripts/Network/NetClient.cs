using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using SkillBridge.Message;

namespace Network
{
    class NetClient : MonoSingleton<NetClient>
    {
        #region 服务器默认设置

        /// <summary>
        /// 默认的网络线程保持时间间隔
        /// </summary>
        const int DEF_POLL_INTERVAL_MILLISECONDS = 100;
        /// <summary>
        /// 默认为服务器重试的次数
        /// </summary>
        const int DEF_TRY_CONNECT_TIMES = 3;
        /// <summary>
        /// RecvStream的默认初始缓冲区大小
        /// </summary>
        const int DEF_RECV_BUFFER_SIZE = 64 * 1024;
        /// <summary>
        /// 默认包头大小
        /// </summary>
        const int DEF_PACKAGE_HEADER_LENGTH = 4;
        /// <summary>
        /// 默认发送ping包的时间间隔
        /// </summary>
        const int DEF_SEND_PING_INTERVAL = 30;
        /// <summary>
        /// 默认连接等待毫秒
        /// </summary>
        const int NetConnectTimeout = 10000;
        /// <summary>
        /// 默认等待几毫秒，然后显示加载轮。
        /// </summary>
        const int DEF_LOAD_WHEEL_MILLISECONDS = 1000;
        /// <summary>
        /// 默认重新连接秒数
        /// </summary>
        const int NetReconnectPeriod = 10;

        #endregion


        public const int NET_ERROR_UNKNOW_PROTOCOL = 2;           //协议错误
        public const int NET_ERROR_SEND_EXCEPTION = 1000;       //发送异常
        public const int NET_ERROR_ILLEGAL_PACKAGE = 1001;      //接受到错误数据包
        public const int NET_ERROR_ZERO_BYTE = 1002;            //收发0字节
        public const int NET_ERROR_PACKAGE_TIMEOUT = 1003;      //收包超时
        public const int NET_ERROR_PROXY_TIMEOUT = 1004;        //proxy超时
        public const int NET_ERROR_FAIL_TO_CONNECT = 1005;      //3次连接不上
        public const int NET_ERROR_PROXY_ERROR = 1006;          //proxy重启
        public const int NET_ERROR_ON_DESTROY = 1007;           //结束的时候，关闭网络连接
        public const int NET_ERROR_ON_KICKOUT = 25;           //被踢了

        public delegate void ConnectEventHandler(int result, string reason);
        public delegate void ExpectPackageEventHandler();

        public event ConnectEventHandler OnConnect;
        public event ConnectEventHandler OnDisconnect;
        public event ExpectPackageEventHandler OnExpectPackageTimeout;
        public event ExpectPackageEventHandler OnExpectPackageResume;

        //socket instance
        private IPEndPoint address;
        private Socket clientSocket;
        private MemoryStream sendBuffer = new MemoryStream();
        private MemoryStream receiveBuffer = new MemoryStream(DEF_RECV_BUFFER_SIZE);
        private Queue<NetMessage> sendQueue = new Queue<NetMessage>();

        /// <summary>
        /// 正在连接
        /// </summary>
        private bool connecting = false;

        private int retryTimes = 0;
        //重试次数总和等于服务器重试次数
        private int retryTimesTotal = DEF_TRY_CONNECT_TIMES;
        /// <summary>
        /// 最后发送时间
        /// </summary>
        private float lastSendTime = 0;
        private int sendOffset = 0;

        public bool running { get; set; }

        public PackageHandler packageHandler = new PackageHandler(null);

        void Awake()
        {
            running = true;
        }

        protected override void OnStart()
        {
            MessageDistributer.Instance.ThrowException = true;
        }

        protected virtual void RaiseConnected(int result, string reason)
        {
            ConnectEventHandler handler = OnConnect;
            if (handler != null)
            {
                handler(result, reason);
            }
        }

        public virtual void RaiseDisonnected(int result, string reason = "")
        {
            ConnectEventHandler handler = OnDisconnect;
            if (handler != null)
            {
                handler(result, reason);
            }
        }

        protected virtual void RaiseExpectPackageTimeout()
        {
            ExpectPackageEventHandler handler = OnExpectPackageTimeout;
            if (handler != null)
            {
                handler();
            }
        }
        protected virtual void RaiseExpectPackageResume()
        {
            ExpectPackageEventHandler handler = OnExpectPackageResume;
            if (handler != null)
            {
                handler();
            }
        }

        public bool Connected
        {
            get
            {
                return (clientSocket != default(Socket)) ? clientSocket.Connected : false;
            }
        }

        public NetClient()
        {
        }

        public void Reset()
        {
            MessageDistributer.Instance.Clear();
            this.sendQueue.Clear();

            this.sendOffset = 0;

            this.connecting = false;

            this.retryTimes = 0;
            this.lastSendTime = 0;

            this.OnConnect = null;
            this.OnDisconnect = null;
            this.OnExpectPackageTimeout = null;
            this.OnExpectPackageResume = null;
        }

        public void Init(string serverIP, int port)
        {
            this.address = new IPEndPoint(IPAddress.Parse(serverIP), port);
        }

        /// <summary>
        /// 连接
        /// 异步连接
        /// 请使用OnConnect处理连接事件 
        /// </summary>
        /// <param name="times"></param>
        public void Connect(int times = DEF_TRY_CONNECT_TIMES)
        {
            //效验逻辑
            if (this.connecting)
            {
                return;
            }

            if (this.clientSocket != null)
            {
                this.clientSocket.Close();
            }
            if (this.address == default(IPEndPoint))
            {
                throw new Exception("Please Init first.");
            }
            Debug.Log("DoConnect");
            this.connecting = true;
            this.lastSendTime = 0;

            this.DoConnect();
        }

        public void OnDestroy()
        {
            Debug.Log("OnDestroy NetworkManager.");
            this.CloseConnection(NET_ERROR_ON_DESTROY);
        }

        public void CloseConnection(int errCode)
        {
            Debug.LogWarning("CloseConnection(), errorCode: " + errCode.ToString());
            this.connecting = false;
            if (this.clientSocket != null)
            {
                this.clientSocket.Close();
            }

            //清空缓冲区
            MessageDistributer.Instance.Clear();
            this.sendQueue.Clear();

            this.receiveBuffer.Position = 0;
            this.sendBuffer.Position = sendOffset = 0;

            switch (errCode)
            {
                case NET_ERROR_UNKNOW_PROTOCOL:
                    {
                        //致命错误，停止网络服务
                        this.running = false;
                    }
                    break;
                case NET_ERROR_FAIL_TO_CONNECT:
                case NET_ERROR_PROXY_TIMEOUT:
                case NET_ERROR_PROXY_ERROR:
                    //NetworkManager.Instance.dropCurMessage();
                    //NetworkManager.Instance.Connect();
                    break;
                //离线处理
                case NET_ERROR_ON_KICKOUT:
                case NET_ERROR_ZERO_BYTE:
                case NET_ERROR_ILLEGAL_PACKAGE:
                case NET_ERROR_SEND_EXCEPTION:
                case NET_ERROR_PACKAGE_TIMEOUT:
                default:
                    this.lastSendTime = 0;
                    this.RaiseDisonnected(errCode);
                    break;
            }

        }

        //发送一个Protobuf消息
        public void SendMessage(NetMessage message)
        {
            if (!running)
            {
                return;
            }

            if (!this.Connected)
            {
                //判断有没有连接 没连接在连接一遍 同时生成日志
                this.receiveBuffer.Position = 0;
                this.sendBuffer.Position = sendOffset = 0;

                this.Connect();
                Debug.Log("Connect Server before Send Message!");
                return;
            }
            //将message放到队列里发送出去
            sendQueue.Enqueue(message);
        
            if (this.lastSendTime == 0)
            {
                this.lastSendTime = Time.time;
            }
        }

        //真正的去连接
        void DoConnect()
        {
            Debug.Log("NetClient.DoConnect on " + this.address.ToString());
            try
            {
                if (this.clientSocket != null)
                {
                    this.clientSocket.Close();
                }


                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Blocking 阻塞 希望客户端连接服务器的阻塞的 能实时等待连接的结果
                this.clientSocket.Blocking = true;

                Debug.Log(string.Format("Connect[{0}] to server {1}", this.retryTimes, this.address) + "\n");
                //异步的方式 BeginConnect  得到result对像 根据result来决定要不要去等待他
                IAsyncResult result = this.clientSocket.BeginConnect(this.address, null, null);
                //waitone 等待我们异步的事件完成才继续执行
                bool success = result.AsyncWaitHandle.WaitOne(NetConnectTimeout);
                if (success)
                {
                    //如果连接成功 就调用end把请求结束
                    this.clientSocket.EndConnect(result);
                }
            }
            catch(SocketException ex)
            {
                if(ex.SocketErrorCode == SocketError.ConnectionRefused)    //捕获网络的异常 ConnectionRefused(服务器拒绝)
                {
                    this.CloseConnection(NET_ERROR_FAIL_TO_CONNECT);
                }
                Debug.LogErrorFormat("DoConnect SocketException:[{0},{1},{2}]{3} ", ex.ErrorCode,ex.SocketErrorCode,ex.NativeErrorCode, ex.ToString()); 
            }
            catch (Exception e)
            {
                Debug.Log("DoConnect Exception:" + e.ToString() + "\n");  //常规的异常
            }

            if (this.clientSocket.Connected)
            {
                //再次判断连接是否成功 成功后关闭阻塞模式 希望在游戏运行过程，使用非阻塞的
                this.clientSocket.Blocking = false;
                this.RaiseConnected(0, "Success");
            }
            else
            {
                this.retryTimes++;
                if (this.retryTimes >= this.retryTimesTotal)
                {
                    this.RaiseConnected(1, "Cannot connect to server");
                }
            }
            this.connecting = false;
        }

        /// <summary>
        /// 保持连接
        /// 判断连接状态
        /// </summary>
        /// <returns></returns>
        bool KeepConnect()
        {
            if (this.connecting)
            {
                return false;
            }
            if (this.address == null)
                return false;

            if (this.Connected)
            {
                return true;
            }

            if (this.retryTimes < this.retryTimesTotal)
            {
                this.Connect();
            }
            return false;
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <returns></returns>
        bool ProcessRecv()
        {
            bool ret = false;
            try
            {
                if (this.clientSocket.Blocking)
                {
                    Debug.Log("this.clientSocket.Blocking = true\n");
                }
                //Poll 判断连接是否断开
                bool error = this.clientSocket.Poll(0, SelectMode.SelectError);
                if (error)
                {
                    Debug.Log("ProcessRecv Poll SelectError\n");
                    this.CloseConnection(NET_ERROR_SEND_EXCEPTION);
                    return false;
                }

                ret = this.clientSocket.Poll(0, SelectMode.SelectRead);
                if (ret)
                {
                    //然后接收 接收完 把数据丢到数据缓冲区
                    int n = this.clientSocket.Receive(this.receiveBuffer.GetBuffer(), 0, this.receiveBuffer.Capacity, SocketFlags.None);
                    if (n <= 0)
                    {
                        this.CloseConnection(NET_ERROR_ZERO_BYTE);
                        return false;
                    }

                    this.packageHandler.ReceiveData(this.receiveBuffer.GetBuffer(), 0, n);

                }
            }
            catch (Exception e)
            {
                Debug.Log("ProcessReceive exception:" + e.ToString() + "\n");
                this.CloseConnection(NET_ERROR_ILLEGAL_PACKAGE);
                return false;
            }
            return true;
        }
       
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        bool ProcessSend()
        {
            bool ret = false;
            try
            {
                if (this.clientSocket.Blocking)
                {
                    Debug.Log("this.clientSocket.Blocking = true\n");
                }
                bool error = this.clientSocket.Poll(0, SelectMode.SelectError);
                if (error)
                {
                    Debug.Log("ProcessSend Poll SelectError\n");
                    this.CloseConnection(NET_ERROR_SEND_EXCEPTION);
                    return false;
                }
                ret = this.clientSocket.Poll(0, SelectMode.SelectWrite);
                if (ret)
                {
                    //sendStream exist data
                    if (this.sendBuffer.Position > this.sendOffset)
                    {
                        int bufsize = (int)(this.sendBuffer.Position - this.sendOffset);
                        int n = this.clientSocket.Send(this.sendBuffer.GetBuffer(), this.sendOffset, bufsize, SocketFlags.None);
                        if (n <= 0)
                        {
                            this.CloseConnection(NET_ERROR_ZERO_BYTE);
                            return false;
                        }
                        this.sendOffset += n;
                        if (this.sendOffset >= this.sendBuffer.Position)
                        {
                            this.sendOffset = 0;
                            this.sendBuffer.Position = 0;
                            this.sendQueue.Dequeue();//remove message when send complete
                        }
                    }
                    else
                    {
                        //fetch package from sendQueue
                        if (this.sendQueue.Count > 0)
                        {
                            NetMessage message = this.sendQueue.Peek();
                            byte[] package = PackageHandler.PackMessage(message);
                            this.sendBuffer.Write(package, 0, package.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("ProcessSend exception:" + e.ToString() + "\n");
                this.CloseConnection(NET_ERROR_SEND_EXCEPTION);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 分发消息
        /// 分发器单例直接分发
        /// </summary>
        void ProceeMessage()
        {
            MessageDistributer.Instance.Distribute();
        }

        //Update need called once per frame
        public void Update()
        {
            if (!running)
            {
                return;
            }

            if (this.KeepConnect())
            {
                if (this.ProcessRecv())
                {
                    //每一帧先接收 接收过程中可能会断线 所以再判断一次有没有连接 
                    if (this.Connected)
                    {
                        //如果连接就发送消息 处理消息
                        this.ProcessSend();
                        this.ProceeMessage();
                    }
                }
            }
        }
    }
}
