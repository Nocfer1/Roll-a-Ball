using UnityEngine;

namespace Materials
{
    [RequireComponent(typeof(Renderer))]
    public class RainbowBridgeMaterialFlow : MonoBehaviour
    {
        public Renderer targetRenderer;
        public float flowSpeed = 0.5f;
        public bool reverseDirection = false;

        private Material runtimeMaterial;
        private float flowOffset;

        private void Awake()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<Renderer>();

            if (targetRenderer != null)
                runtimeMaterial = targetRenderer.material;
        }

        private void Update()
        {
            if (runtimeMaterial == null)
                return;

            float direction = reverseDirection ? -1f : 1f;

            flowOffset += direction * flowSpeed * Time.deltaTime;
            runtimeMaterial.SetFloat("_FlowOffset", flowOffset);
        }
    }
}