using UnityEngine;

public class CameraVisibilityChecker : MonoBehaviour
{
    [Header("Camera Reference")]
    public Camera playerCamera;

    [Header("Target Objects")]
    public Transform[] targetObjects;

    private Plane[] cameraFrustumPlanes;

    private void Update()
    {
        if (playerCamera == null || targetObjects == null)
            return;

        // Recalculate the camera’s frustum planes each frame
        cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        foreach (Transform target in targetObjects)
        {
            if (target == null)
                continue;

            Renderer targetRenderer = target.GetComponent<Renderer>();
            if (targetRenderer == null)
                continue;

            bool isVisible = GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, targetRenderer.bounds);

            // Optional: visualize visible objects by color
            targetRenderer.material.color = isVisible ? Color.green : Color.red;

            if (isVisible)
            {
                Debug.Log($"Camera can see: {target.name}");
            }
        }
    }
}
