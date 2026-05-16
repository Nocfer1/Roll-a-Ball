using UnityEngine;

namespace Runner
{
    public class ObstacleSpin : MonoBehaviour
    {
        public Vector3 rotationSpeed = new Vector3(120f, 180f, 90f);

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}