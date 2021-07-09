using System;
using UnityEngine;

namespace Player
{
    public class PlayerComponent : MonoBehaviour
    {
        public PlayerModel model = new PlayerModel();

        public PlayerModel Init(int playerID)
        {
            model.View = this;
            model.playerID = playerID;
            return model;
        }
    }
}
