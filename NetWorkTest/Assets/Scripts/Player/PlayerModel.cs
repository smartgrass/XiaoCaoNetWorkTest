
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
        private long lastTime;
        public Vector3 lastPos;

        public Vector3 Pos { get => View.transform.position; }

        public void SetPos(PosPlayerMsg pos)
        {
            lastTime = manager.reciveTime;
            float deltaT =(DateTime.Now.Ticks -lastTime)/10000;


            
            //需要上一个位置

            float lerp = Mathf.Clamp((deltaT / 200), 0.1f, 0.9f);

            Debug.Log("yns   deltal = " + deltaT  + "; lerp " + lerp);
            var target = pos.ToVec3();
            View.transform.position = Vector3.Lerp(lastPos, target, lerp);

        }
        public void SendPos()
        {
            if (client)
                client.SendPOS(View.transform.position);
        }
        public void SaveLastPos()
        {
            lastPos = View.transform.position;
        }
    }
}
