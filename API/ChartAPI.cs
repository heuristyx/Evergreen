using System;
using Everhood.Chart;

namespace Evergreen;

public static class ChartAPI {
  // Event args
  public class NoteEventArgs : EventArgs {
    public Everhood.Chart.Note note { get; set; }
  }

  public class SectionEventArgs : EventArgs {
    public Everhood.Chart.Section section { get; set; }
  }

  // Event handlers
  public static event EventHandler<NoteEventArgs> OnNoteSpawn;
  public static event EventHandler<SectionEventArgs> OnSectionStart;

  internal static void Init() {
    On.Everhood.Chart.NoteEventHandler.OnNote += HookOnNoteSpawn;
    On.Everhood.Chart.SectionEventHandler.OnSection += HookOnSectionStart;
  }

  // Hooks
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