using System;
using UnityEngine;

public class RedNightVisionOverlay : MonoBehaviour
{
    private Texture2D texture;

    private bool isActive;
    private bool lastLoggedState;
    private bool hasLoggedInitialState;

    private GameObject nvLightObject;
    private Light nvLight;
    private Camera attachedCamera;

    private void Awake()
    {
        // Existing red tint overlay.
        texture = new Texture2D(1, 1);

        texture.SetPixel(
            0, 0,
            new Color(
                0.55f, // red
                0.06f, // green
                0.02f, // blue
                0.22f  // alpha
            )
        );

        texture.Apply();

        Log.Out("[DFRedNightVision] Red overlay Awake()");
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

        //
        // Create/reattach the NV illumination light when we have
        // access to the gameplay camera.
        //
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

        //
        // If the game replaced the camera (loading, respawn, etc.),
        // rebuild the light on the new camera.
        //
        if (nvLight != null && attachedCamera == camera)
            return;

        DestroyNightVisionLight();

        attachedCamera = camera;

        nvLightObject = new GameObject("DFRedNightVisionLight");

        nvLightObject.transform.SetParent(
            camera.transform,
            false
        );

        //
        // Put the light essentially at the player's eyes.
        //
        nvLightObject.transform.localPosition =
            new Vector3(0.0f, 0.0f, 0.15f);

        nvLightObject.transform.localRotation =
            Quaternion.identity;

        nvLight = nvLightObject.AddComponent<Light>();

        //
        // Weak omnidirectional illumination.
        //
        // Think "IR illuminator", not flashlight.
        //
        nvLight.type = LightType.Point;

        //
        // Mostly neutral/warm illumination. The red screen effect
        // will provide the strong red coloration separately.
        //
        nvLight.color =
            new Color(1.0f, 0.85f, 0.75f);

        //
        // These are the main two values to tune.
        //
        nvLight.range = 8.0f;
        nvLight.intensity = 0.35f;

        //
        // We don't want this to behave like another expensive
        // shadow-casting flashlight.
        //
        nvLight.shadows = LightShadows.None;

        nvLight.enabled = isActive;

        Log.Out(
            "[DFRedNightVision] Attached NV light to camera: "
            + camera.name
        );
    }

    private void OnGUI()
    {
        if (!isActive)
            return;

        GUI.depth = -10000;

        GUI.DrawTexture(
            new Rect(
                0,
                0,
                Screen.width,
                Screen.height
            ),
            texture
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

        if (texture != null)
        {
            Destroy(texture);
            texture = null;
        }
    }
}
