
using UnityEngine;

namespace Player
{
    public class PlayerModel : PersonModel
    {
        public PlayerModel() { personType = EnumPerson.Player; }

        public PlayerComponent View;
        public TcpClientTool client;

        public int playerID;

        public Vector3 Pos { get => View.transform.position; }

        public void SetPos(Vector3 pos,Vector3 lastPos)
        {
            //lastP
            View.transform.position = Vector3.Lerp(View.transform.position, pos, 0.5f);
        }
        public void SendPos()
        {
            if (client)
                client.SendPOS(View.transform.position);
        }

    }
}
