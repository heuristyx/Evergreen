using System;

namespace CBCompat;

public static class ChartAPI
{
  public class NoteEventArgsCB : EventArgs
  {
    public ChartNoteSectionData.Data.Note note { get; set; }
  }

  public class SectionEventArgsCB : EventArgs
  {
    public ChartNoteSectionData.Data.Section section { get; set; }
  }

  public static void RegisterHookOnChartStart(Action<object, EventArgs> callback)
  {
    On.Everhood.ModExternal.ChartReader.ChartReaderBehaviour += (
      On.Everhood.ModExternal.ChartReader.orig_ChartReaderBehaviour orig,
      Everhood.ModExternal.ChartReader self
    ) =>
    {
      // to-do: chart start logic
      orig(self);
    };
  }

  public static void RegisterHookOnNoteSpawn(Action<object, NoteEventArgsCB> callback)
  {
    On.Everhood.ModExternal.NoteEventHandler.OnNote += (
      On.Everhood.ModExternal.NoteEventHandler.orig_OnNote orig,
      Everhood.ModExternal.NoteEventHandler self,
      ChartNoteSectionData.Data.Note note
    ) =>
    {
      var args = new NoteEventArgsCB { note = note };
      callback(self, args);
      note = args.note;

      orig(self, note);
    };
  }

  public static void RegisterHookOnSectionStart(Action<object, SectionEventArgsCB> callback)
  {
    On.Everhood.ModExternal.SectionEventHandler.OnSection += (
      On.Everhood.ModExternal.SectionEventHandler.orig_OnSection orig,
      Everhood.ModExternal.SectionEventHandler self,
      ChartNoteSectionData.Data.Section section
    ) =>
    {
      var args = new SectionEventArgsCB { section = section };
      callback(self, args);
      section = args.section;

      orig(self, section);
    };
  }
}