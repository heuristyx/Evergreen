using System;
using Everhood;
using Everhood.Battle;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Evergreen;

public static class BattleAPI {
  // Event args
  public class DamageEventArgs : EventArgs {
    public int damage { get; set; }
  }

  // Event handlers
  public static event EventHandler<DamageEventArgs> OnTakeDamage;
  public static event EventHandler<DamageEventArgs> OnDealDamage;
  public static event EventHandler OnBattleUpdate;
  public static event EventHandler OnBattleStart;
  public static event EventHandler OnKill;  
  public static event EventHandler OnLose;
  public static event EventHandler OnJump;

  internal static void Init() {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(BattleAPI)}");

    On.Everhood.Battle.BattlePlayer.Damage += HookOnTakeDamage;
    On.Everhood.Battle.BattleEnemy.Damage += HookOnDealDamage;
    On.Everhood.Battle.BattlePlayer.Update += HookOnBattleUpdate;
    On.Everhood.Battle.BattlePlayer.Awake += HookOnBattleStart;
    On.Everhood.Battle.BattleGameOverController.GameOver += HookOnLose;
    On.Everhood.KillModeEvents.NpcKilled += HookOnKill;
    On.Everhood.Battle.PlayerVerticalMovement.Jump += HookOnJump;
  }

  // Hooks
  internal static void HookOnTakeDamage(On.Everhood.Battle.BattlePlayer.orig_Damage orig, BattlePlayer self, int damage) {
    var args = new DamageEventArgs { damage = damage };
    OnTakeDamage?.Invoke(self, args);
    damage = args.damage;

    orig(self, damage);
  }

  internal static void HookOnDealDamage(On.Everhood.Battle.BattleEnemy.orig_Damage orig, BattleEnemy self, int damage) {
    var args = new DamageEventArgs { damage = damage };
    OnDealDamage?.Invoke(self, args);
    damage = args.damage;

    orig(self, damage);
  }

  internal static void HookOnBattleUpdate(On.Everhood.Battle.BattlePlayer.orig_Update orig, BattlePlayer self) {
    OnBattleUpdate?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  internal static void HookOnBattleStart(On.Everhood.Battle.BattlePlayer.orig_Awake orig, BattlePlayer self) {
    orig(self);

    OnBattleStart?.Invoke(self, EventArgs.Empty);
  }

  internal static void HookOnLose(On.Everhood.Battle.BattleGameOverController.orig_GameOver orig, BattleGameOverController self) {
    OnLose?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  internal static void HookOnKill(On.Everhood.KillModeEvents.orig_NpcKilled orig, KillModeEvents self) {
    OnKill?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  internal static void HookOnJump(On.Everhood.Battle.PlayerVerticalMovement.orig_Jump orig, PlayerVerticalMovement self) {
    OnJump?.Invoke(self, EventArgs.Empty);

    orig(self);
  }
}