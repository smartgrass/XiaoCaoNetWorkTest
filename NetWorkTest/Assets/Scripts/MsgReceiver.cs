using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MsgReceiver : Singleton<MsgReceiver>
{
    public static void ReciveBaseMsg(BaseMsg baseMsg)
    {
        if (baseMsg.MsgTypeEnum == ((int)MsgTypeEnum.Pos))
        {


        }
        else if (baseMsg.MsgTypeEnum == ((int)MsgTypeEnum.Login))
        {
            CSLoginInfo loginInfo = CSLoginInfo.Parser.ParseFrom(baseMsg.ContextBytes.ToByteArray());
            PlayerManager.Instance.CreatPlayer();
        }
    }

    public static void ReciveBaseMsgBytes(byte[] recvData, int myRequestLength)
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
}


