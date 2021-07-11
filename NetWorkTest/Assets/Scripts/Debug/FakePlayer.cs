using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Debug
{
    public class FakePlayer : MonoBehaviour
    {

        public bool isOn;

        public FakeClient fake;


        [Button]
        public void StartFake()
        {
            fake = new FakeClient();
            fake.Login(999, "999");
            StartCoroutine(waitOpenUpdate());
        }

        IEnumerator waitOpenUpdate()
        {
            yield return new WaitForSeconds(2);
            isOn = true;
            yield return null;
        }

        void Update()
        {
            if (isOn)
            {
                UpdateInput();

            }
        }

        private void UpdateInput()
        {
            var hor = Input.GetAxisRaw("Horizontal"); //左右键
            var ver = Input.GetAxisRaw("Vertical");
            Vector3 target = transform.position + new Vector3(hor, 0, ver)*Time.deltaTime;
            transform.position = target;
            fake.SendPOS(target);
        }
    }
}