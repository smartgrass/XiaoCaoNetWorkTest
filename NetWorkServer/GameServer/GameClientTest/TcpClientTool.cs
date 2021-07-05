

//引入库
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Google.Protobuf;

public class TcpClientTool 
{
    string editString = "hello wolrd"; //编辑框文字

    Socket serverSocket; //服务器端socket
    IPAddress ip; //主机ip
    IPEndPoint ipEnd;
    public  string recvStr; //接收的字符串
    string sendStr; //发送的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程

    //初始化
    void InitSocket()
    {
        //定义服务器的IP和端口，端口与服务器对应
        //ip = IPAddress.Parse(GetLocalIP()); //可以是局域网或互联网ip，此处是本机
                                            // 192.168.30.156
        ip = IPAddress.Parse("127.0.0.1");
        ipEnd = new IPEndPoint(ip, 7777);


        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketConnet()
    {
        if (serverSocket != null)
            serverSocket.Close();
        //定义套接字类型,必须在子线程中定义
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        System.Console.WriteLine("ready to connect");
        //连接
        serverSocket.Connect(ipEnd);
        System.Console.WriteLine();
        //输出初次连接收到的字符串
        //recvLen = serverSocket.Receive(recvData);
        //recvStr = Encoding.Unicode.GetString(recvData, 0, recvLen);
        //System.Console.WriteLine(recvStr);
        System.Console.WriteLine("连接成功");

        RequestMsg msg = new RequestMsg();
        msg.FileName = "这是测试";
        msg.StarPos = 1;
        msg.Bytes = ByteString.CopyFrom(Encoding.ASCII.GetBytes("12231354"));
        msg.SendLength = 1;
        msg.FileType = "1";
        msg.FilePath = "1";
        //错误原因: msg.Bytes 不是用于转二进制的文件的
        //Console.WriteLine("发送数据");
        SocketSend(MsgHelper.GetSendBaseMsg(msg.ToByteArray(), MsgTypeEnum.Reques));
        Thread.Sleep(4000);

        SocketSend(MsgHelper.GetSendBaseMsg(msg.ToByteArray(), MsgTypeEnum.Reques));
        //Console.WriteLine("发送数据");

    }

    public void SocketSend(string sendStr)
    {
        //清空发送缓存
        sendData = new byte[1024];
        //数据类型转换
        sendData = Encoding.Unicode.GetBytes(sendStr);
        //发送
        serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
    }
    public void SocketSend(BaseMsg msg)
    {
        try
        {
            Console.WriteLine("send "  + msg.Bytes);
            
            byte[] messageBytes = msg.ToByteArray();
            //byte[] messageBytes = Encoding.ASCII.GetBytes("12231354");
            serverSocket.Send(messageBytes);


            Console.WriteLine("send");
        }
        catch (Exception)
        {
            //Debug.Log("服务器断开");
            return;
        }
    }


    void SocketReceive()
    {
        SocketConnet();
        //不断接收服务器发来的数据
        while (true)
        {
            recvData = new byte[1024];
            recvLen = serverSocket.Receive(recvData);
            if (recvLen == 0)
            {
                SocketConnet();
                continue;
            }
            recvStr = Encoding.Unicode.GetString(recvData, 0, recvLen);
            System.Console.WriteLine(recvStr);
            
        }
    }

    void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭服务器
        if (serverSocket != null)
            serverSocket.Close();
        System.Console.WriteLine("diconnect");
    }

    // Use this for initialization
    public void Start()
    {
        InitSocket();
    }

    //void OnGUI()
    //{
    //    editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
    //    if (GUI.Button(new Rect(10, 30, 60, 20), "send"))
    //        SocketSend(editString);
    //}


    //程序退出则关闭连接
    void OnApplicationQuit()
    {
        SocketQuit();
    }

    public static string GetLocalIP()
    {
        try
        {
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    //Debug.Log(IpEntry.AddressList[i].ToString());
                    return IpEntry.AddressList[i].ToString();
                }
            }
            return "noIp";
        }
        catch
        {
            //Debug.Log("ipGetFailed");
            return ("ipGetFailed");
        }
    }

}
