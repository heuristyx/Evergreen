using System;
using System.Reflection;
using Everhood.Chart;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Sirenix.Serialization;

namespace Evergreen;

/// <summary>
/// The Evergreen API for chart-related events.
/// </summary>
public static class ChartAPI
{
  public class NoteEventArgs : EventArgs
  {
    public Everhood.Chart.Note note { get; set; }
  }

  public class SectionEventArgs : EventArgs
  {
    public Everhood.Chart.Section section { get; set; }
  }

  public static bool audioClipLoaded = false;

  /// <summary>
  /// Invoked when a chart starts (i.e. when music starts playing).
  /// </summary>
  public static event EventHandler OnChartStart;

  /// <summary>
  /// Invoked on the tick a note spawns.
  /// </summary>
  public static event EventHandler OnNoteSpawn;

  /// <summary>
  /// Invoked on the tick a section starts.
  /// </summary>
  public static event EventHandler OnSectionStart;

  internal static void Init()
  {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(ChartAPI)}");

    if (Evergreen.IsBaseGame)
    {
      On.Everhood.Chart.ChartReader.ChartReaderBehaviour += HookOnChartStart;
      On.Everhood.Chart.ChartReader.Release += (On.Everhood.Chart.ChartReader.orig_Release orig, Everhood.Chart.ChartReader cr) => { audioClipLoaded = false; };
      On.Everhood.Chart.NoteEventHandler.OnNote += HookOnNoteSpawn;
      On.Everhood.Chart.SectionEventHandler.OnSection += HookOnSectionStart;
    }
    else
    {
      CBCompat.ChartAPI.RegisterHookOnChartStart(RaiseChartStartEvent);
      CBCompat.ChartAPI.RegisterHookOnNoteSpawn(RaiseNoteSpawnEvent);
      CBCompat.ChartAPI.RegisterHookOnSectionStart(RaiseSectionStartEvent);
    }

    // Patch to guarantee chart is synced to audio
    if (Evergreen.IsBaseGame)
    {
      IL.Everhood.Chart.ChartReader.ChartReaderBehaviour += HookChartReaderBehaviour;
      IL.Everhood.Chart.ChartReader.JumpPosChange += HookJumpPosChange;
    }
    else
    {
      CBCompat.ChartAPI.FixChartReaderBehaviour();
    }
  }

  private static void HookChartReaderBehaviour(ILContext il)
  {
    var c = new ILCursor(il);

    var m_setSongPosition = typeof(Everhood.Chart.ChartReader).GetMethod("set__songposition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    c.TryGotoNext(MoveType.After, i => i.MatchCall(m_setSongPosition));
    c.Emit(OpCodes.Ldarg_0);
    c.EmitDelegate<Action<Everhood.Chart.ChartReader>>((cr) =>
    {
      cr._songposition = cr.audioSource.time;
    });
  }

  private static void HookJumpPosChange(ILContext il)
  {
    var c = new ILCursor(il);

    var m_setSongPosition = typeof(Everhood.Chart.ChartReader).GetMethod("set__songposition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    c.TryGotoNext(MoveType.After, i => i.MatchCall(m_setSongPosition));
    c.Emit(OpCodes.Ldarg_0);
    c.EmitDelegate<Action<Everhood.Chart.ChartReader>>((cr) =>
    {
      cr._songposition = cr.audioSource.time;
    });
  }

  public static void RaiseChartStartEvent(object self, EventArgs args)
  {
    OnChartStart?.Invoke(self, args);
  }

  public static void RaiseNoteSpawnEvent(object self, CBCompat.ChartAPI.NoteEventArgsCB args)
  {
    OnNoteSpawn?.Invoke(self, args);
  }

  public static void RaiseSectionStartEvent(object self, CBCompat.ChartAPI.SectionEventArgsCB args)
  {
    OnSectionStart?.Invoke(self, args);
  }

  private static void HookOnChartStart(On.Everhood.Chart.ChartReader.orig_ChartReaderBehaviour orig, Everhood.Chart.ChartReader self)
  {
    if (self._started && !audioClipLoaded)
    {
      audioClipLoaded = true;
      RaiseChartStartEvent(self, EventArgs.Empty);
    }

    orig(self);
  }

  private static void HookOnNoteSpawn(On.Everhood.Chart.NoteEventHandler.orig_OnNote orig, NoteEventHandler self, Everhood.Chart.Note note)
  {
    var args = new NoteEventArgs { note = note };
    OnNoteSpawn?.Invoke(self, args);
    note = args.note;
    orig(self, note);
  }

  private static void HookOnSectionStart(On.Everhood.Chart.SectionEventHandler.orig_OnSection orig, SectionEventHandler self, Everhood.Chart.Section section)
  {
    var args = new SectionEventArgs { section = section };
    OnSectionStart?.Invoke(self, args);
    section = args.section;
    orig(self, section);
  }
}