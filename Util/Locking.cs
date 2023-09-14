using System.Collections.Generic;
using SG.Leaderboard;

namespace Evergreen;

public static class Locking {
  private readonly static List<Lock> locks = new List<Lock>();

  public enum LockType {
    Score,
    BattleStars,
    Achievements
  }

  public class Lock {
    public string PluginGUID;
    public string LockID;
    public LockType LockType;

    public Lock(string pluginGUID,  string lockID, LockType lockType) {
      PluginGUID = pluginGUID;
      LockID = lockID;
      LockType = lockType;
    }
  }

  public static void AddLock(Lock _lock) {
    locks.Add(_lock);
  }

  public static void RemoveLock(Lock _lock) {
    locks.Remove(_lock);
  }

  public static List<Lock> GetLocks() {
    return locks;
  }

  public static List<Lock> GetLocks(LockType lockType) {
    return locks.FindAll(l => l.LockType == lockType);
  }

  // Locking
  internal static void Init() {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(Locking)}");

    On.SG.Leaderboard.Leaderboard.SaveScore += HookLockSaveScore;
    On.SteamAchievement.UnlockAchievement += HookLockAchievements;
    On.EverhoodGameData.GlobalData.UpdateBattleStars += HookLockBattleStars;
  }

  private static void HookLockSaveScore(On.SG.Leaderboard.Leaderboard.orig_SaveScore orig, Leaderboard self, byte battleID, byte modeID, int score) {
    if (GetLocks(LockType.Score).Count > 0) {
      Evergreen.Log.LogInfo("Did not submit score to leaderboards because one or more mods has disabled it.");
      return;
    }

    orig(self, battleID, modeID, score);
  }

  private static void HookLockAchievements(On.SteamAchievement.orig_UnlockAchievement orig, SteamAchievement self) {
    if (GetLocks(LockType.Achievements).Count > 0) {
      Evergreen.Log.LogInfo($"Did not unlock Steam achievement {self.name} because one or more mods has disabled it.");
      return;
    }

    orig(self);
  }

  private static void HookLockBattleStars(On.EverhoodGameData.GlobalData.orig_UpdateBattleStars orig, EverhoodGameData.GlobalData self, string ID, int score) {
    if (GetLocks(LockType.BattleStars).Count > 0) {
      Evergreen.Log.LogInfo("Did not save battle stars because one or more mods has disabled it.");
      return;
    }

    orig(self, ID, score);
  }
}