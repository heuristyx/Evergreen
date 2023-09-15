using System;
using Everhood;
using Everhood.Battle;
using Mono.Cecil.Cil;
using MonoMod.Cil;

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
    IL.Everhood.Battle.BattleEnemyHealthObserver.Update += HookOnKill;
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

  internal static void HookOnKill(ILContext il) {
    ILCursor c = new ILCursor(il);
    bool found = c.TryGotoNext(MoveType.Before, i => i.MatchCallvirt(typeof(EventCommandsGroupExecutor).GetMethod(nameof(EventCommandsGroupExecutor.Execute))));

    c.Emit(OpCodes.Ldarg_0);
    c.EmitDelegate<Action<BattleEnemyHealthObserver>>((obs) => {
      OnKill?.Invoke(obs, EventArgs.Empty);
    });
  }

  internal static void HookOnJump(On.Everhood.Battle.PlayerVerticalMovement.orig_Jump orig, PlayerVerticalMovement self) {
    OnJump?.Invoke(self, EventArgs.Empty);

    orig(self);
  }
}