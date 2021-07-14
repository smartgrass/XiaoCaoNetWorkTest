
using UnityEngine;

namespace Player
{
    public class PlayerModel : PersonModel
    {
        public PlayerModel() { personType = EnumPerson.Player; }

        public PlayerComponent View;
        public TcpClientTool client;

        public int playerID;
        private float deltaTime = 0.02f;
        private float lastTime;

        public Vector3 Pos { get => View.transform.position; }

        public void SetPos(PosPlayerMsg pos, PosPlayerMsg lastPos)
        {
            lastTime = PlayerManager.Instance.reciveTime;
            float deltaT = Time.time - lastTime;
            float timeLen = pos.SendTiem - lastPos.SendTiem;
            float lerp = Mathf.Clamp(deltaT / timeLen, 0.1f, 0.8f);
            Debug.Log("yns  lerp " + lerp) ;
            //lastP
            var target = pos.ToVec3();
            View.transform.position = Vector3.Lerp(View.transform.position, target, lerp);

        }
        public void SendPos()
        {
            if (client)
                client.SendPOS(View.transform.position);
        }

    }
}
