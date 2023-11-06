using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine.Experimental.PlayerLoop;

namespace Evergreen;

[BepInPlugin(Guid, Name, Version)]
public class Evergreen : BaseUnityPlugin {
  public const string Guid = "com.heuristyx.plugins.evergreen";
  public const string Name = "Evergreen";
  public const string Version = "0.1.0";
  public enum Executable {
    BaseGame,
    CustomBattles
  };

  public static Executable CurrentExecutable;

  public static PluginInfo PluginInfo { get; private set; }

  internal static ManualLogSource Log;

  private void Awake() {
    Log = BepInEx.Logging.Logger.CreateLogSource("Evergreen");

    PluginInfo = Info;

    Init();

    Assets.Init();

    TextDrawing.Init();
    Modlist.Init();

    BattleAPI.Init();
    ChartAPI.Init();
    if (CurrentExecutable == Executable.BaseGame) Locking.Init();

    Log.LogInfo("Evergreen loaded.");
  }

  private void Init() {
    if (Type.GetType("Everhood.ModExternal.ChartReader, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") != null) CurrentExecutable = Executable.CustomBattles;
    else CurrentExecutable = Executable.BaseGame;
    Log.LogInfo($"Evergreen is running on the {(CurrentExecutable == Executable.BaseGame ? "base game" : "custom battles launcher")}.");
  }
}
