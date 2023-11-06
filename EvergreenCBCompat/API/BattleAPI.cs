using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CBCompat;

public static class BattleAPI {
  public static void RegisterHookOnBattleLoad(Action<object, GameObject> callback) {
    On.EverhoodModding.BattleInitializer.Init += (
      On.EverhoodModding.BattleInitializer.orig_Init orig,
      EverhoodModding.BattleInitializer self
    ) => {
      if (SceneManager.GetActiveScene().buildIndex != 0) callback(self, self.gameObject);

      orig(self);
    };
  }

  // to-do: battle leave hook for CB
}