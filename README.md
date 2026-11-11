# DFRedNightVision

A small C# mod for **7 Days to Die**, intended for a Darkness Falls setup that uses the `dfRedNV` player custom variable.

Despite the historical name, the current implementation adds a **white point light**, not a red screen overlay. The light follows the main camera and is enabled when the local player's `dfRedNV` value is greater than `0.5`.

- Range: 8 Unity units
- Intensity: 0.35
- No shadows
- Reattaches when the main camera changes

This mod only reads `dfRedNV`; it does not provide items, buffs, or a keybinding to set it. You need the corresponding game/mod configuration that controls that variable. Compatibility with specific game or Darkness Falls versions has not been established; build against your own installation's assemblies.

## Build

Requires a .NET SDK capable of building .NET Framework 4.8 projects, .NET Framework 4.8 reference assemblies (restored by modern SDKs when needed), and a local game installation with the required managed DLLs.

```sh
dotnet build -c Release -p:GameDir="/path/to/7 Days To Die"
```

If your installation has a different directory layout, specify the managed directory directly:

```sh
dotnet build -c Release -p:GameManagedDir="/path/to/7DaysToDie_Data/Managed"
```

The build references `Assembly-CSharp.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.IMGUIModule.dll`, and `LogLibrary.dll` from your installation. These proprietary dependencies are not included in this repository or copied to the output.

Output: `bin/Release/net48/DFRedNightVision.dll` and `ModInfo.xml`.

## Install

1. Build against the game/mod installation you intend to use.
2. Create a `DFRedNightVision` folder inside the game's applicable `Mods` directory.
3. Copy `DFRedNightVision.dll` and `ModInfo.xml` from the build output into that folder.
4. Launch the game with **Easy Anti-Cheat disabled**; this is a DLL mod.

Install on the client that needs the effect. This implementation relies on a local player and camera, not a dedicated server. Look for `[DFRedNightVision]` messages in the game log to check initialization and activation.

To remove the mod, close the game and delete its `Mods/DFRedNightVision` folder.

## License

The mod source is licensed under **GPL-3.0-only**. See [LICENSE](LICENSE).

7 Days to Die, Darkness Falls, Unity, and their assemblies remain subject to their respective licenses. They are not covered by this project's license. This is an unofficial project.
