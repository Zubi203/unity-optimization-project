using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToolkitManager : MonoBehaviour
{
    public bool objectPooling = true;
    public bool reducedUpdateRate = true;
    public bool simplifiedPathing = true;
    public bool burstCompiling = true;
    public bool centralizedUpdateLogic = true;
    public bool GPUInstancing = true;
    public bool culling = true;
    public bool bakedLighting = true;

    
    private void Awake()
    {
        ServiceLocator.Instance.Register(typeof(ToolkitManager), gameObject);
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.Unregister(typeof(ToolkitManager));
    }

    public void ObjectPoolingToggled(bool toggleOn)
    {
        objectPooling = toggleOn;
    }

    public void CentralizedUpdateToggled(bool toggleOn)
    {
        centralizedUpdateLogic = toggleOn;
    }

    public void BurstCompilingToggled(bool toggleOn)
    {
        burstCompiling = toggleOn;
    }

    public void ReducedUpdateRateToggled(bool toggleOn)
    {
        reducedUpdateRate = toggleOn;
    }

    public void SimplifiedPathingToggled(bool toggleOn)
    {
        simplifiedPathing = toggleOn;
    }

    public void GPUInstancingToggled(bool toggleOn)
    {
        GPUInstancing = toggleOn;
    }

    public void CullingToggled(bool toggleOn)
    {
        Camera cam = Camera.main;
        cam.useOcclusionCulling = toggleOn;//
    }

    public void BakedLightingToggled(bool toggleOn)
    {
        bakedLighting = toggleOn;
    }

    public void ShadowsToggled(bool toggleOn)
    {
        if (Camera.main.TryGetComponent(out UniversalAdditionalCameraData cameraData))
        {
            cameraData.renderShadows = !toggleOn;
        }
    }

    public void LODToggled(bool toggleOn)
    {
        if (toggleOn)
        {//
            QualitySettings.meshLodThreshold = 8.0f;
        }
        else
        {
            QualitySettings.meshLodThreshold = 0.01f;
        }
    }

}
