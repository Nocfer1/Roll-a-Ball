using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlanarReflection : MonoBehaviour
{
    public Camera reflectionCamera;
    public RenderTexture reflectionTexture;

    void Start()
    {
        if (reflectionTexture == null)
        {
            reflectionTexture = new RenderTexture(1024, 1024, 16);
        }

        if (reflectionCamera != null)
        {
            reflectionCamera.targetTexture = reflectionTexture;
        }
    }
}