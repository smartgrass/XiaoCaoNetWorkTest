

//引入库
using Google.Protobuf;
using NaughtyAttributes;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class FakeClient : MonoBehaviour
{
    //常量
    const int NetConnectTimeout = 10000;    //default connect wait milliseconds

    //数据
    public string playerName="999";
    public int playID=999;


    //网络
    Socket socket; 
    IPAddress ip; //主机ip
    IPEndPoint ipEnd;
    public  string recvStr; //接收的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节


    public bool isConect = true;

    //private MsgReceiver m_Receiver;
    //public MsgReceiver Receiver { 
    //    get {
    //        if (m_Receiver != null) return m_Receiver;
    //        else
    //        {
    //            m_Receiver = new MsgReceiver(playID);
    //            return m_Receiver;
    //        };
    //    }
    //    set => m_Receiver = value; 
    //}

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
            IAsyncResult result =  socket.BeginConnect(ipEnd, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(NetConnectTimeout);
            if (success)
            {
                ConnectCallback();
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
    private void ConnectCallback()
    {
        SendLogin();
    }
    [Button]
    private void SendLogin()
    {
        CSLoginInfo loginInfo = new CSLoginInfo();

        loginInfo.PlayerID = playID;
        loginInfo.UserName = playerName;
        SocketSend(MsgHelper.GetSendBaseMsg(loginInfo, MsgTypeEnum.Login, playID));
    }

    public void SendPOS(Vector3 pos)
    {
        PosPlayerMsg posPlayer = new PosPlayerMsg();
        posPlayer.PlayerId = playID;
        posPlayer.Pos = new PosMsg() { X = pos.x, Y = pos.y, Z = pos.z };
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
        //socket.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, ReceiveCallback,null);
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
    private  byte[] ReciveBytes( byte[] recvData, int myRequestLength)
    {
        if (recvData != null)
        {
            //Receiver.ReciveBaseMsgBytes(recvData, myRequestLength);
        }
        return recvData;
    }



    #region Login

    public void Login(int playerID, string pass)
    {
        playID = playerID;
        InitSocket();
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
