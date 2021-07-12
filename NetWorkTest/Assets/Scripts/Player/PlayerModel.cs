
using UnityEngine;

namespace Player
{
    public class PlayerModel : PersonModel
    {
        public PlayerModel() { personType = EnumPerson.Player; }

        public PlayerComponent View;
        public TcpClientTool client;

        public int playerID;
        private float lastTime =0 ;
        private float deltaTime = 0.02f;
        

        public Vector3 Pos { get => View.transform.position; }

        public void SetPos(Vector3 pos,Vector3 lastPos)
        {
            if(lastTime != 0)
                deltaTime = Time.time - lastTime;
            lastTime = Time.time;
            //lastP
            var dis = Vector3.Distance(View.transform.position, pos);
            var speed = dis / deltaTime;
            var dir = (pos - View.transform.position ).normalized;
            var target = View.transform.position + dir * speed * deltaTime;
            View.transform.position = Vector3.Lerp(View.transform.position, target, 0.25f);

        }
        public void SendPos()
        {
            if (client)
                client.SendPOS(View.transform.position);
        }

    }
}
