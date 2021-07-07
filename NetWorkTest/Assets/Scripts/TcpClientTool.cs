

//引入库
using Google.Protobuf;
using NaughtyAttributes;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class TcpClientTool :MonoBehaviour
{
    //常量
    const int NetConnectTimeout = 10000;    //default connect wait milliseconds

    //数据
    public string playerName="123";
    public int playID=123;


    //网络
    Socket socket; 
    IPAddress ip; //主机ip
    IPEndPoint ipEnd;
    public  string recvStr; //接收的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节


    public bool isConect = true;

    public void SetPlayer(string playerName,int playID)
    {
        this.playerName = playerName;
        this.playID = playID;
    }
    void InitSocket()
    {
        ip = IPAddress.Parse("127.0.0.1");
        ipEnd = new IPEndPoint(ip, 7777);
        if (socket != null)
            socket.Close();
        try{
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //连接  serverSocket.Connect(ipEnd);
            IAsyncResult result =  socket.BeginConnect(ipEnd, ConnectCallback, null);
            bool success = result.AsyncWaitHandle.WaitOne(NetConnectTimeout);
            if (success)
            {
                StartReceive();
            }
        }
        catch (SocketException ex)
        {
            Debug.LogErrorFormat("DoConnect SocketException:[{0},{1},{2}]{3} ", ex.ErrorCode, ex.SocketErrorCode, ex.NativeErrorCode, ex.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("DoConnect Exception:" + e.ToString() + "\n");
        }
    }
    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Debug.Log("yns  callBack");
            SendLogin();
        }
        catch
        {

        }
    }
    [Button]
    private void SendLogin()
    {
        CSLoginInfo loginInfo = new CSLoginInfo();

        loginInfo.PlayerID = playID;
        loginInfo.UserName = playerName;
        SocketSend(MsgHelper.GetSendBaseMsg(loginInfo, MsgTypeEnum.Login, playID));
    }
    [Button]
    private void SendPOS()
    {
        PosPlayerMsg posPlayer = new PosPlayerMsg();
        posPlayer.PlayerId = playID;
        posPlayer.Pos = new PosMsg() { X = 1, Y = 1, Z = 1 };
        SocketSend(MsgHelper.GetSendBaseMsg(posPlayer, MsgTypeEnum.Pos, playID));
    }

    public void SocketSend(BaseMsg msg)
    {
        try
        {
            Debug.Log("send "  +(MsgTypeEnum) msg.MsgTypeEnum);
            byte[] messageBytes = msg.ToByteArray();
            socket.Send(messageBytes);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }
    }
    private void StartReceive()
    {
        socket.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, ReceiveCallback,null);
    }

    private  void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (socket == null | socket.Connected == false) return;
            int len = socket.EndReceive(ar);
            if(len == 0)
            {
                CloseSocket();
            }
            
            ReciveBytes(recvData, len);
            StartReceive();
        }
        catch
        {

        }
    }
    private static byte[] ReciveBytes( byte[] recvData, int myRequestLength)
    {
        if (recvData != null)
        {
            recvData = recvData.RemoveEmptyByte(myRequestLength);
            BaseMsg baseMsg = BaseMsg.Parser.ParseFrom(recvData);
            Debug.Log("yns  callBack " +((MsgTypeEnum)baseMsg.MsgTypeEnum).ToString());
            if (baseMsg.MsgTypeEnum == (int)MsgTypeEnum.Pos)
            {
                //转发消息给其他客户端


            }
            else if (baseMsg.MsgTypeEnum == (int)MsgTypeEnum.Login)
            {
                CSLoginInfo loginInfo = CSLoginInfo.Parser.ParseFrom(baseMsg.ContextBytes);

                Console.WriteLine($"yns {loginInfo.UserName} conected....");
                //转发消息给其他客户端
 
            }
        }
        return recvData;
    }



    #region Login

    private UnityAction<object> LoginCallBackAct;
    public Action LoginAct;
    public void Login(int playerID, string pass)
    {
        playID = playerID;
        LoginAct = () =>
        {
            CSLoginInfo loginInfo = new CSLoginInfo()
            {
                UserName = "11",
                Password = pass,
                PlayerID = playerID
            };
            var msg = MsgHelper.GetSendBaseMsg(loginInfo, MsgTypeEnum.Login, loginInfo.PlayerID);
            SocketSend(msg);
        };
        InitSocket();
    }
    public void AddCallBack(UnityAction<object> callBack)
    {
        LoginCallBackAct = callBack;
    }    
    public void RemoveCallBack()
    {
        LoginCallBackAct = null;
    }
    private void LoginCallBack()
    {
        LoginCallBackAct.Invoke("11");
    }

    #endregion


    #region 基本
    [Button]
    void CloseSocket()
    {
        if (socket != null)
        {
            socket.Close();
        }
        Debug.Log("diconnect");
    }
    void OnApplicationQuit()
    {
        CloseSocket();
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
    #endregion

}
