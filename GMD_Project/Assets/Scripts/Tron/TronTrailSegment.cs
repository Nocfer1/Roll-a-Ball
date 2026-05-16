using UnityEngine;

namespace Tron
{
    public class TronTrailSegment : MonoBehaviour
    {
        private Vector3 startPoint;
        private float thickness = 0.4f;

        public int ownerPlayerID;
        public bool armedForOwner = false;

        public void Initialize(Vector3 start, Vector3 dir, Material mat, float trailThickness, int ownerID)
        {
            startPoint = start;
            thickness = trailThickness;
            ownerPlayerID = ownerID;
            armedForOwner = false;

            transform.position = new Vector3(start.x, 0.6f, start.z);
            transform.localScale = new Vector3(thickness, 0.35f, thickness);

            Renderer rend = GetComponent<Renderer>();
            if (rend != null && mat != null)
            {
                Material trailMatInstance = new Material(mat);
                rend.material = trailMatInstance;

                if (trailMatInstance.HasProperty("_EmissionColor"))
                {
                    Color baseColor = trailMatInstance.color;
                    Color emissionColor = baseColor * 2.5f;

                    trailMatInstance.EnableKeyword("_EMISSION");
                    trailMatInstance.SetColor("_EmissionColor", emissionColor);
                }
            }
        }

        public void UpdateEndPoint(Vector3 endPoint)
        {
            Vector3 center = (startPoint + endPoint) / 2f;
            Vector3 diff = endPoint - startPoint;

            transform.position = new Vector3(center.x, 0.6f, center.z);

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
            {
                transform.localScale = new Vector3(Mathf.Abs(diff.x) + thickness, 0.15f, thickness);
            }
            else
            {
                transform.localScale = new Vector3(thickness, 0.35f, Mathf.Abs(diff.z) + thickness);
            }
        }

        public void ArmForOwner()
        {
            armedForOwner = true;
        }
    }
}