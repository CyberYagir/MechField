using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.Menu.Enviroment
{
    public class WalkingMan : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float maxWalkTime;

        private float time;
        private Vector3 startPos;
        private Vector3 startRot;

        private void Awake()
        {
            startPos = transform.position;
            startRot = transform.eulerAngles;
        }

        void Update()
        {
            time += Time.deltaTime;
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

            if (time >= maxWalkTime)
            {
                var rnd = Random.value;
                if (rnd < 0.5f)
                {
                    transform.position = startPos;
                    transform.eulerAngles = startRot;
                }
                else
                {
                    transform.eulerAngles += Vector3.up * 180;
                }

                time = 0;
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * speed * maxWalkTime);
        }
    }
}
