using UnityEngine;

public class DFRedNightVisionMod : IModApi
{
    public void InitMod(Mod mod)
    {
        Log.Out("[DFRedNightVision] Loading");

        ModEvents.GameStartDone.RegisterHandler(OnGameStartDone);
    }

    private void OnGameStartDone()
    {
        var go = new GameObject("DFRedNightVisionOverlay");

        Object.DontDestroyOnLoad(go);
        go.AddComponent<RedNightVisionOverlay>();

        Log.Out("[DFRedNightVision] Overlay initialized");
    }
}
