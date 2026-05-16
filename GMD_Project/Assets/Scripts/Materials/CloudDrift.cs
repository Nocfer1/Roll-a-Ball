using UnityEngine;

namespace Materials
{
    public class CloudPuffDriftGroup : MonoBehaviour
    {
        [Header("Movement")]
        public float speed = 2f;
        public bool useUnscaledTime = true;

        [Header("Loop Settings")]
        public float startZ = 330f;
        public float endZ = -80f;

        private void Update()
        {
            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            foreach (Transform cloud in transform)
            {
                Vector3 pos = cloud.position;
                pos.z -= speed * delta;

                if (pos.z <= endZ)
                {
                    pos.z = startZ;
                }

                cloud.position = pos;
            }
        }
    }
}