using System;
using UnityEngine;

namespace Player
{
    public class EnemyComponent : MonoBehaviour
    {
        public EnemyModel model = new EnemyModel();

        public EnemyModel Init(int playerID)
        {
            model.View = this;
            return model;
        }
    }
}
