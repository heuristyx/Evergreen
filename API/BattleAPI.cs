using System;
using UnityEngine;
using Everhood.Battle;

namespace Evergreen;

public static class BattleAPI {
  // Event args
  public class DamageEventArgs : EventArgs {
    public int damage { get; set; }
  }

  public class StateLikeEventArgs : EventArgs {
    public bool state { get; set; }
  }

  // Event handlers
  public static event EventHandler<DamageEventArgs> OnTakeDamage;
  public static event EventHandler<DamageEventArgs> OnDealDamage;
  public static event EventHandler<StateLikeEventArgs> OnJump;
  public static event EventHandler OnBattleUpdate;

  internal static void Init() {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(BattleAPI)}");

    // Add events to hooks
    On.Everhood.Battle.BattlePlayer.Damage += HookOnTakeDamage;
    On.Everhood.Battle.BattleEnemy.Damage += HookOnDealDamage;
    On.Everhood.Battle.BattlePlayer.SetJumpState += HookOnJump;
    On.Everhood.Battle.BattleEnemyHealthObserver.Init += HookOnBattleStart;
  }

  // Hooks
  internal static void HookOnTakeDamage(On.Everhood.Battle.BattlePlayer.orig_Damage orig, BattlePlayer self, int damage) {
    if (OnTakeDamage != null) {
      var args = new DamageEventArgs { damage = damage };
      foreach (EventHandler<DamageEventArgs> e in OnTakeDamage.GetInvocationList()) e?.Invoke(self, args);
      damage = args.damage;
    }

    orig(self, damage);
  }

  internal static void HookOnDealDamage(On.Everhood.Battle.BattleEnemy.orig_Damage orig, BattleEnemy self, int damage) {
    if (OnDealDamage != null) {
      var args = new DamageEventArgs { damage = damage };
      foreach (EventHandler<DamageEventArgs> e in OnDealDamage.GetInvocationList()) e?.Invoke(self, args);
      damage = args.damage;
    }

    orig(self, damage);
  }

  internal static void HookOnJump(On.Everhood.Battle.BattlePlayer.orig_SetJumpState orig, BattlePlayer self, bool state) {
    if (OnJump != null && state) {
      var args = new StateLikeEventArgs { state = state };
      foreach (EventHandler<StateLikeEventArgs> e in OnJump.GetInvocationList()) e?.Invoke(self, args);
      state = args.state;
    }

    orig(self, state);
  }

  internal static void HookOnBattleStart(On.Everhood.Battle.BattleEnemyHealthObserver.orig_Init orig, BattleEnemyHealthObserver self) {
    
    GameObject evergreenBattleManager = new GameObject("BattleManager", typeof(BattleManager));

    orig(self);
  }

  internal static void HookOnBattleUpdate(BattleManager self) {
    if (OnBattleUpdate != null)
      foreach (EventHandler e in OnBattleUpdate.GetInvocationList())
        e?.Invoke(self, EventArgs.Empty);
  }
}