using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class MsgReceiver
{
    public MsgReceiver(){}
    public MsgReceiver(int _playID)
    {
        playID = _playID;
    }

    public int playID = 123;



    public void ReciveBaseMsg(BaseMsg baseMsg)
    {
        Debug.Log("yns  Recive thread  " + (MsgTypeEnum)baseMsg.MsgTypeEnum);
        if (baseMsg.MsgTypeEnum == ((int)MsgTypeEnum.Pos))
        {


        }
        else if (baseMsg.MsgTypeEnum == ((int)MsgTypeEnum.Login))
        {
            CSLoginInfo loginInfo = CSLoginInfo.Parser.ParseFrom(baseMsg.ContextBytes.ToByteArray());
           // PlayerManager.Instance.CreatPlayer();
        }
        else if (baseMsg.MsgTypeEnum == ((int)MsgTypeEnum.Allplayer))
        {
            AllPosMsg allPos = AllPosMsg.Parser.ParseFrom(baseMsg.ContextBytes.ToByteArray());
            Debug.Log("yns  " + PlayerManager.Instance.IsUpdatePos);
            PlayerManager.Instance.SetAllPlayer(allPos); 
        }

    }

    public  void ReciveBaseMsgBytes(byte[] recvData, int myRequestLength)
    {
        recvData = recvData.RemoveEmptyByte(myRequestLength);
        BaseMsg baseMsg = BaseMsg.Parser.ParseFrom(recvData);
        ReciveBaseMsg(baseMsg);
    }

}

public static class ExtensionClass
{
    public static byte[] RemoveEmptyByte(this byte[] by, int length)
    {
        byte[] returnByte = new byte[length];

        for (int i = 0; i < length; i++)
        {
            returnByte[i] = by[i];
        }
        return returnByte;

    }
    public static Vector3 ToVec3(this PosPlayerMsg pos)
    {
        return new Vector3(pos.Pos.X, pos.Pos.Y, pos.Pos.Z);
    }
    //public static Type GetPersonType()
}


