using BepInEx;
using BepInEx.Logging;

namespace Evergreen;

[BepInPlugin(Guid, Name, Version)]
public class Evergreen : BaseUnityPlugin {
  public const string Guid = "com.heuristyx.plugins.evergreen";
  public const string Name = "Evergreen";
  public const string Version = "0.1.0";

  public static PluginInfo PluginInfo { get; private set; }

  internal static ManualLogSource Log;

  private void Awake() {
    Log = BepInEx.Logging.Logger.CreateLogSource("Evergreen");

    PluginInfo = Info;

    Assets.Init();

    TextDrawing.Init();
    Modlist.Init();

    BattleAPI.Init();
    ChartAPI.Init();
    Locking.Init();

    Log.LogInfo("Evergreen loaded.");
  }
}
