using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace Evergreen;

[BepInPlugin(Guid, Name, Version)]
public class Evergreen : BaseUnityPlugin
{
  public const string Guid = "com.heuristyx.plugins.evergreen";
  public const string Name = "Evergreen";
  public const string Version = "1.1.0";

  public enum Executable
  {
    BaseGame,
    CustomBattles
  };

  public static Executable CurrentExecutable;
  public static bool IsBaseGame = false;
  public static bool DebugMode = false;
  public static event EventHandler<bool> OnToggleDebugMode;

  public static PluginInfo PluginInfo { get; private set; }

  internal static ManualLogSource Log;

  private void Awake()
  {
    Log = BepInEx.Logging.Logger.CreateLogSource("Evergreen");

    PluginInfo = Info;

    Init();

    Assets.Init();

    TextDrawing.Init();
    if (IsBaseGame) Modlist.Init();

    BattleAPI.Init();
    ChartAPI.Init();
    if (IsBaseGame) Locking.Init();

    Log.LogInfo("Evergreen loaded.");
  }

  private void Init()
  {
    const string testType = "Everhood.ModExternal.ChartReader, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    IsBaseGame = Type.GetType(testType) == null;
    CurrentExecutable = IsBaseGame ? Executable.BaseGame : Executable.CustomBattles;
    Log.LogInfo($"Evergreen is running on the {(CurrentExecutable == Executable.BaseGame ? "base game" : "custom battles launcher")}.");
  }

  /// <summary>
  /// Toggles debug mode.
  /// </summary>
  public static void ToggleDebugMode(bool state)
  {
    DebugMode = state;
    OnToggleDebugMode?.Invoke(null, state);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Slash) && DebugMode)
    {
      TextDrawing.inputField.ActivateInputField();
    }
  }
}
