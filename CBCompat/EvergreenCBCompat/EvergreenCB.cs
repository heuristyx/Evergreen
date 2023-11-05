using System;
using Everhood.ModExternal;
using UnityEngine.SceneManagement;
using static Evergreen.ChartAPI;

namespace CBCompat;

public static class CompatMethods {

  public static void InitBattleHooks() {
    On.EverhoodModding.BattleInitializer.Init += HookOnBattleLoad;
    // TO-DO: CB battle leave hook
  }

  private static void HookOnBattleLoad(On.EverhoodModding.BattleInitializer.orig_Init orig, EverhoodModding.BattleInitializer self) {
    if (SceneManager.GetActiveScene().buildIndex != 0) self.gameObject.AddComponent<Evergreen.BattleManager>();
    audioClipLoaded = false;

    orig(self);
  }

  public static void InitChartHooks() {
    On.Everhood.ModExternal.ChartReader.ChartReaderBehaviour += HookOnChartUpdate;
    On.Everhood.ModExternal.NoteEventHandler.OnNote += HookOnNoteSpawn;
    On.Everhood.ModExternal.SectionEventHandler.OnSection += HookOnSectionStart;
  }

  private static void HookOnNoteSpawn(On.Everhood.ModExternal.NoteEventHandler.orig_OnNote orig, NoteEventHandler self, ChartNoteSectionData.Data.Note note) {
    var args = new Evergreen.ChartAPI.NoteEventArgsCB { note = note };
    RaiseNoteSpawnEvent(self, args);
    note = args.note;

    orig(self, note);
  }

  private static void HookOnSectionStart(On.Everhood.ModExternal.SectionEventHandler.orig_OnSection orig, SectionEventHandler self, ChartNoteSectionData.Data.Section section) {
    var args = new Evergreen.ChartAPI.SectionEventArgsCB { section = section };
    RaiseSectionStartEvent(self, args);
    section = args.section;

    orig(self, section);
  }

  private static void HookOnChartUpdate(On.Everhood.ModExternal.ChartReader.orig_ChartReaderBehaviour orig, Everhood.ModExternal.ChartReader self) {
    if (self._started && !audioClipLoaded) {
      audioClipLoaded = true;
      RaiseChartStartEvent(self, EventArgs.Empty);
    }

    orig(self);
  }
}