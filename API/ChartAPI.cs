using System;
using Everhood.Chart;

namespace Evergreen;

/// <summary>
/// The Evergreen API for chart-related events.
/// </summary>
public static class ChartAPI {
  public class NoteEventArgs : EventArgs {
    public Everhood.Chart.Note note { get; set; }
  }

  public class SectionEventArgs : EventArgs {
    public Everhood.Chart.Section section { get; set; }
  }

  private static bool audioClipLoaded = false;

  /// <summary>
  /// Invoked when a chart starts (i.e. when music starts playing).
  /// </summary>
  public static event EventHandler OnChartStart;

  /// <summary>
  /// Invoked on the tick a note spawns.
  /// </summary>
  public static event EventHandler<NoteEventArgs> OnNoteSpawn;

  /// <summary>
  /// Invoked on the tick a section starts.
  /// </summary>
  public static event EventHandler<SectionEventArgs> OnSectionStart;

  internal static void Init() {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(ChartAPI)}");

    On.Everhood.Chart.ChartReader.ChartReaderBehaviour += HookOnChartUpdate;
    On.Everhood.Chart.ChartReader.Release += (On.Everhood.Chart.ChartReader.orig_Release orig, Everhood.Chart.ChartReader cr) => { audioClipLoaded = false; };
    On.Everhood.Chart.NoteEventHandler.OnNote += HookOnNoteSpawn;
    On.Everhood.Chart.SectionEventHandler.OnSection += HookOnSectionStart;
  }

  private static void HookOnChartUpdate(On.Everhood.Chart.ChartReader.orig_ChartReaderBehaviour orig, Everhood.Chart.ChartReader self) {
    if (self._started && !audioClipLoaded) {
      audioClipLoaded = true;
      OnChartStart?.Invoke(self, EventArgs.Empty);
    }

    orig(self);
  }

  private static void HookOnNoteSpawn(On.Everhood.Chart.NoteEventHandler.orig_OnNote orig, NoteEventHandler self, Everhood.Chart.Note note) {
    var args = new NoteEventArgs { note = note };
    OnNoteSpawn?.Invoke(self, args);
    note = args.note;

    orig(self, note);
  }

  private static void HookOnSectionStart(On.Everhood.Chart.SectionEventHandler.orig_OnSection orig, SectionEventHandler self, Everhood.Chart.Section section) {
    var args = new SectionEventArgs { section = section };
    OnSectionStart?.Invoke(self, args);
    section = args.section;

    orig(self, section);
  }
}