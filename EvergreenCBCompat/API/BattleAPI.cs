using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CBCompat;

public static class BattleAPI
{
  public static void FixChartReaderBehaviour()
  {
    IL.Everhood.ModExternal.ChartReader.ChartReaderBehaviour += HookChartReaderBehaviour;
    IL.Everhood.ModExternal.ChartReader.JumpPosChange += HookJumpPosChange;
  }

  private static void HookChartReaderBehaviour(ILContext il)
  {
    var c = new ILCursor(il);

    var m_setSongPosition = typeof(Everhood.ModExternal.ChartReader).GetMethod("set__songposition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    c.TryGotoNext(MoveType.After, i => i.MatchCall(m_setSongPosition));
    c.Emit(OpCodes.Ldarg_0);
    c.EmitDelegate<Action<Everhood.ModExternal.ChartReader>>((cr) =>
    {
      cr._songposition = cr.audioSource.time;
    });
  }

  private static void HookJumpPosChange(ILContext il)
  {
    var c = new ILCursor(il);

    var m_setSongPosition = typeof(Everhood.ModExternal.ChartReader).GetMethod("set__songposition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    c.TryGotoNext(MoveType.After, i => i.MatchCall(m_setSongPosition));
    c.Emit(OpCodes.Ldarg_0);
    c.EmitDelegate<Action<Everhood.ModExternal.ChartReader>>((cr) =>
    {
      cr._songposition = cr.audioSource.time;
    });
  }

  public static void RegisterHookOnBattleLoad(Action<object, GameObject> callback)
  {
    On.EverhoodModding.BattleInitializer.Init += (
      On.EverhoodModding.BattleInitializer.orig_Init orig,
      EverhoodModding.BattleInitializer self
    ) =>
    {
      if (SceneManager.GetActiveScene().buildIndex != 0) callback(self, self.gameObject);

      orig(self);
    };
  }

  // to-do: battle leave hook for CB
}