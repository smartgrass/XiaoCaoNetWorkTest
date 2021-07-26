
using System;
using UnityEngine;

namespace Player
{
    public class EnemyModel : PersonModel
    {
        public EnemyModel() { personType = EnumPerson.Enemy; }

        public EnemyComponent View;

        public Vector3 Pos { get => View.transform.position; }


        public void SendPos()
        {

        }

    }
}
