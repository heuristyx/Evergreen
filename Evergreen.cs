using System;
using BepInEx;
using BepInEx.Logging;
using Everhood.Battle;

namespace Evergreen;

[BepInPlugin(Guid, Name, Version)]
public class Evergreen : BaseUnityPlugin {
  public const string Guid = "com.heuristyx.plugins.evergreen";
  public const string Name = "Evergreen";
  public const string Version = "0.1.0";

  internal static ManualLogSource Log;

  private void Awake() {
    Log = BepInEx.Logging.Logger.CreateLogSource("Evergreen");

    // Init APIs
    BattleAPI.Init();
    Locking.Init();

    Log.LogInfo("Evergreen loaded.");
  }
}
