using System;

namespace CBCompat;

public static class ChartAPI {
  public class NoteEventArgsCB : EventArgs {
    public ChartNoteSectionData.Data.Note note { get; set; }
  }

  public class SectionEventArgsCB : EventArgs {
    public ChartNoteSectionData.Data.Section section { get; set; }
  }

  public static void RegisterHookOnChartStart(Action<object, EventArgs> EventRaiser) {
    On.Everhood.ModExternal.ChartReader.ChartReaderBehaviour += (
      On.Everhood.ModExternal.ChartReader.orig_ChartReaderBehaviour orig,
      Everhood.ModExternal.ChartReader self
    ) => {
      // to-do: chart start logic
      orig(self);
    };
  }

  public static void RegisterHookOnNoteSpawn(Action<object, NoteEventArgsCB> EventRaiser) {
    On.Everhood.ModExternal.NoteEventHandler.OnNote += (
      On.Everhood.ModExternal.NoteEventHandler.orig_OnNote orig,
      Everhood.ModExternal.NoteEventHandler self,
      ChartNoteSectionData.Data.Note note
    ) => {
      var args = new NoteEventArgsCB { note = note };
      EventRaiser(self, args);
      note = args.note;

      UnityEngine.Debug.Log("Note spawned");

      orig(self, note);
    };
  }

  public static void RegisterHookOnSectionStart(Action<object, SectionEventArgsCB> EventRaiser) {
    On.Everhood.ModExternal.SectionEventHandler.OnSection += (
      On.Everhood.ModExternal.SectionEventHandler.orig_OnSection orig,
      Everhood.ModExternal.SectionEventHandler self,
      ChartNoteSectionData.Data.Section section
    ) => {
      var args = new SectionEventArgsCB { section = section };
      EventRaiser(self, args);
      section = args.section;

      UnityEngine.Debug.Log("Section started");

      orig(self, section);
    };
  }
}