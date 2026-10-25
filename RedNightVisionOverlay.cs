using System;
using UnityEngine;

public class RedNightVisionOverlay : MonoBehaviour
{
    private bool isActive;
    private bool lastLoggedState;
    private bool hasLoggedInitialState;

    private GameObject nvLightObject;
    private Light nvLight;
    private Camera attachedCamera;

    private void Awake()
    {
        Log.Out("[DFRedNightVision] Awake()");
    }

    private void Update()
    {
        isActive = GetNightVisionState();

        if (!hasLoggedInitialState || isActive != lastLoggedState)
        {
            Log.Out("[DFRedNightVision] Active = " + isActive);

            lastLoggedState = isActive;
            hasLoggedInitialState = true;
        }

        EnsureNightVisionLight();

        if (nvLight != null)
            nvLight.enabled = isActive;
    }

    private bool GetNightVisionState()
    {
        try
        {
            if (GameManager.Instance == null ||
                GameManager.Instance.World == null)
            {
                return false;
            }

            EntityPlayerLocal player =
                GameManager.Instance.World.GetPrimaryPlayer();

            if (player == null || player.Buffs == null)
                return false;

            return player.Buffs.GetCustomVar("dfRedNV") > 0.5f;
        }
        catch (Exception ex)
        {
            Log.Out(
                "[DFRedNightVision] GetNightVisionState exception: "
                + ex.Message
            );

            return false;
        }
    }

    private void EnsureNightVisionLight()
    {
        Camera camera = Camera.main;

        if (camera == null)
            return;

        // If we're already attached to the current gameplay camera,
        // there's nothing to do.
        if (nvLight != null && attachedCamera == camera)
            return;

        DestroyNightVisionLight();

        attachedCamera = camera;

        nvLightObject = new GameObject("DFRedNightVisionLight");

        nvLightObject.transform.SetParent(
            camera.transform,
            false
        );

        // Slightly in front of the player's viewpoint.
        nvLightObject.transform.localPosition =
            new Vector3(0.0f, 0.0f, 0.15f);

        nvLightObject.transform.localRotation =
            Quaternion.identity;

        nvLight = nvLightObject.AddComponent<Light>();

        // Weak omnidirectional local illumination.
        nvLight.type = LightType.Point;

        // White light.
        nvLight.color = Color.white;

        // Main tuning knobs.
        nvLight.range = 8.0f;
        nvLight.intensity = 0.35f;

        nvLight.shadows = LightShadows.None;
        nvLight.enabled = isActive;

        Log.Out(
            "[DFRedNightVision] Attached white NV light to camera: "
            + camera.name
        );
    }

    private void DestroyNightVisionLight()
    {
        if (nvLightObject != null)
        {
            Destroy(nvLightObject);
            nvLightObject = null;
        }

        nvLight = null;
        attachedCamera = null;
    }

    private void OnDestroy()
    {
        DestroyNightVisionLight();
    }
}
