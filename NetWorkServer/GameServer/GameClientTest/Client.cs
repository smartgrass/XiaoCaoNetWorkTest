using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Client
{

    public static string clientUserName = "user1";
    public string ip = "127.0.0.1";
    public int port = 7777;

    Socket clientSock;

    Thread receive;

    public void DefautClientIni()
    {
        ClientIni(ip, port);
    }
    public void ClientIni(string ip, int port)
    {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //TcpClient client = new TcpClient()
        try
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
            clientSock.Connect(point);

            //SocketSend("我是"+clientUserName + "，连接了你\n");
            receive = new Thread(new ThreadStart(ReceiveMessage));
            receive.Start();
            // StartCoroutine(ieReceiveMessage());
            // StartCoroutine(ieSendPos());

            Console.WriteLine("已连接服务器");
            RequestMsg msg = new RequestMsg();
            msg.FileName = "这是测试";

            SocketSend(MsgHelper.GetSendBaseMsg(msg.Bytes, MsgTypeEnum.Reques));
            Console.WriteLine("发送数据");
            Thread.Sleep(4000);

            SocketSend(MsgHelper.GetSendBaseMsg(msg.Bytes, MsgTypeEnum.Reques));
            Console.WriteLine("发送数据");
            //Debug.Log("已连接服务器");
        }
        catch (Exception)
        {
            //Debug.Log("服务器没有开启");
            //Debug.Log("连接服务器失败");
            //Destroy(this);
            return;
        }

    }


    void Start()
    {
        
        if (ip == "")
        {
            ip = GetLocalIP();
        }
        ClientIni(ip, port);

    }


    public void SocketSend(BaseMsg msg)
    {
        try
        {
            byte[] messageBytes = msg.Bytes.ToByteArray();
            clientSock.Send(messageBytes);
            Console.WriteLine("send");
        }
        catch (Exception)
        {
            //Debug.Log("服务器断开");
            return;
        }

    }
    private bool isReceviving = true;


    public void ReceiveMessage()
    {
        try
        {
            while (true)
            {
                if (isReceviving == false)
                {
                    break;
                }
                byte[] messageBytes = new byte[1024];
                //等待接收
                int num = clientSock.Receive(messageBytes);
                BaseMsg baseMsg = BaseMsg.Parser.ParseFrom(messageBytes);
                if (baseMsg.MsgType == (int)MsgTypeEnum.Reques)
                {
                    RequestMsg request = RequestMsg.Parser.ParseFrom(baseMsg.Bytes);
                    Console.WriteLine(request.ToString());
                }
                Console.WriteLine("接收到信息");
            }
        }
        catch (Exception)
        {
            //Debug.Log("服务器断开");
            return;
        }
    }


    #region 暂时用不上
    private void OnDestroy()
    {
        isReceviving = false;

        //关闭线程
        if (receive != null)
        {
            receive.Interrupt();
            receive.Abort();
        }

        //最后关闭服务器
        if (clientSock != null)
            clientSock.Close();
    }

    public static string GetLocalIP()
    {
        try
        {
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    // //Debug.Log(IpEntry.AddressList[i].ToString());
                    return IpEntry.AddressList[i].ToString();
                }
            }
            return "noIp";
        }
        catch
        {
            return ("ipGetFailed");
        }
    }
    #endregion

}


