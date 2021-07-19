
using System;
using UnityEngine;

namespace Player
{
    public class PlayerModel : PersonModel
    {
        public PlayerModel() { personType = EnumPerson.Player; }

        public PlayerComponent View;
        public TcpClientTool client;
        public PlayerManager manager;

        public int playerID;
        private long lastTime = 0;
        //public Vector3 lastPos;
        PosPlayerMsg lastPos;
        private Vector3 speedVec= Vector3.zero;
        private float needTime;

        public Vector3 Pos { get => View.transform.position; }

        public void SetPos(PosPlayerMsg pos)
        {
            if(lastPos == pos)
            {
                //无新包 移动
                if(speedVec != Vector3.zero)
                    View.transform.position += speedVec * (Time.fixedDeltaTime / needTime);
            }
            else if(lastPos!=null)
            {
                //有新包 计算两个包的速度
                //speedVec = pos.ToVec3() - lastPos.ToVec3();
                speedVec = pos.ToVec3() - View.transform.position;
                //4位数是将时间戳转毫秒
                //7位数是将时间转转为秒
                needTime = (pos.SendTime - lastPos.SendTime)/ 10000000f;
                //移动
                if (speedVec != Vector3.zero)
                    View.transform.position += speedVec * (Time.fixedDeltaTime / needTime);
            }
            else
            {
                View.transform.position = pos.ToVec3();
            }
            lastPos = pos;
        }

        public void SendPos()
        {
            if (client)
                client.SendPOS(View.transform.position);
        }
        //public void SaveLastPos()
        //{
        //    lastPos = View.transform.position;
        //}
    }
}
