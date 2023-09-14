using Everhood.Battle;
using System;
using UnityEngine;

namespace Evergreen;

public static class BattleAPI {
  // Event args
  public class DamageEventArgs : EventArgs {
    public int damage { get; set; }
  }

  // Event handlers
  public static event EventHandler<DamageEventArgs> OnDamage;
  public static event EventHandler OnBattleUpdate;

  internal static void Init() {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(BattleAPI)}");

    // Add events to hooks
    On.Everhood.Battle.BattlePlayer.Damage += HookOnDamage;
    On.Everhood.Battle.BattleEnemyHealthObserver.Init += HookOnBattleStart;
  }

  // Hooks
  internal static void HookOnDamage(On.Everhood.Battle.BattlePlayer.orig_Damage orig, Everhood.Battle.BattlePlayer self, int damage) {
    if (OnDamage != null) {
      var args = new DamageEventArgs { damage = damage };
      foreach (EventHandler<DamageEventArgs> e in OnDamage.GetInvocationList()) e?.Invoke(self, args);
      damage = args.damage;
    }

    orig(self, damage);
  }

  internal static void HookOnBattleStart(On.Everhood.Battle.BattleEnemyHealthObserver.orig_Init orig, Everhood.Battle.BattleEnemyHealthObserver self) {
    
    GameObject evergreenBattleManager = new GameObject("BattleManager", typeof(BattleManager));

    orig(self);
  }

  internal static void HookOnBattleUpdate(BattleManager self) {
    if (OnBattleUpdate != null)
      foreach (EventHandler e in OnBattleUpdate.GetInvocationList())
        e?.Invoke(self, EventArgs.Empty);
  }
}