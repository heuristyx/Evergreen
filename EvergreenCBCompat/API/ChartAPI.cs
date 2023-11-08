using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;

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

  public static void RegisterHookOnChartStart(Action<object, EventArgs> callback)
  {
    On.Everhood.ModExternal.ChartReader.StartChartReader += (
      On.Everhood.ModExternal.ChartReader.orig_StartChartReader orig,
      Everhood.ModExternal.ChartReader self
    ) =>
    {
      orig(self);

      callback(self, EventArgs.Empty);
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