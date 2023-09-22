using System;
using Everhood;
using Everhood.Battle;

namespace Evergreen;

/// <summary>
/// The Evergreen API for battle-related events.
/// </summary>
public static class BattleAPI {
  public class DamageEventArgs : EventArgs {
    public int damage { get; set; }
  }

  /// <summary>
  /// Invoked when the player takes damage.
  /// </summary>
  public static event EventHandler<DamageEventArgs> OnTakeDamage;

  /// <summary>
  /// Invoked when the enemy takes damage.
  /// </summary>
  public static event EventHandler<DamageEventArgs> OnDealDamage;

  /// <summary>
  /// Invoked every frame during battle.
  /// </summary>
  public static event EventHandler OnBattleUpdate;

  /// <summary>
  /// Invoked when a battle is loaded. (For when the music starts, try <c>ChartAPI.OnChartStart</c>.)
  /// </summary>
  public static event EventHandler OnBattleStart;

  /// <summary>
  /// Invoked when the battle is restarted, through the pause or game over menu.
  /// </summary>
  public static event EventHandler OnBattleRetry;

  /// <summary>
  /// Invoked when the enemy is killed.
  /// </summary>
  public static event EventHandler OnKill;

  /// <summary>
  /// Invoked when the player is defeated.
  /// </summary>
  public static event EventHandler OnLose;

  /// <summary>
  /// Invoked when the player jumps.
  /// </summary>
  public static event EventHandler OnJump;

  /// <summary>
  /// Invoked when the player successfully absorbs a note.
  /// </summary>
  public static event EventHandler OnAbsorbNote;

  /// <summary>
  /// Invoked when the player shoots absorbed notes back.
  /// </summary>
  public static event EventHandler OnShootDeflect;

  internal static void Init() {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(BattleAPI)}");

    On.Everhood.BattlePauseController.Awake += HookOnBattleLoad;
    On.Everhood.BattlePauseController.Retry += HookOnBattleRetryBPC;
    On.Everhood.Battle.GameOverController.Retry += HookOnBattleRetryGOC;
    On.Everhood.Battle.BattlePlayer.Damage += HookOnTakeDamage;
    On.Everhood.Battle.BattleEnemy.Damage += HookOnDealDamage;
    On.Everhood.Battle.BattleGameOverController.GameOver += HookOnLose;
    On.Everhood.KillModeEvents.NpcKilled += HookOnKill;
    On.Everhood.Battle.PlayerVerticalMovement.Jump += HookOnJump;
    On.AbsorbBehaviour.NotifyOnAbsorbSuccess += HookOnAbsorbNote;
    On.AbsorbBehaviour.NotifyOnAbsorbTallSuccess += HookOnAbsorbTallNote;
    On.Everhood.ShootDeflectiveProjectileEventCommand.ShootDeflect += HookOnShootDeflect;
  }

  internal static void RaiseBattleStart(object sender) {
    OnBattleStart?.Invoke(sender, EventArgs.Empty);
  }

  internal static void RaiseBattleUpdate(object sender) {
    OnBattleUpdate?.Invoke(sender, EventArgs.Empty);
  }

  private static void HookOnBattleLoad(On.Everhood.BattlePauseController.orig_Awake orig, BattlePauseController self) {
    self.gameObject.AddComponent<BattleManager>();

    orig(self);
  }

  private static void HookOnBattleRetryBPC(On.Everhood.BattlePauseController.orig_Retry orig, BattlePauseController self) {
    OnBattleRetry?.Invoke(self, EventArgs.Empty);
    
    orig(self);
  }

  private static void HookOnBattleRetryGOC(On.Everhood.Battle.GameOverController.orig_Retry orig, Everhood.Battle.GameOverController self) {
    OnBattleRetry?.Invoke(self, EventArgs.Empty);
    
    orig(self);
  }

  private static void HookOnTakeDamage(On.Everhood.Battle.BattlePlayer.orig_Damage orig, BattlePlayer self, int damage) {
    var args = new DamageEventArgs { damage = damage };
    OnTakeDamage?.Invoke(self, args);
    damage = args.damage;

    orig(self, damage);
  }

  private static void HookOnDealDamage(On.Everhood.Battle.BattleEnemy.orig_Damage orig, BattleEnemy self, int damage) {
    var args = new DamageEventArgs { damage = damage };
    OnDealDamage?.Invoke(self, args);
    damage = args.damage;

    orig(self, damage);
  }

  private static void HookOnLose(On.Everhood.Battle.BattleGameOverController.orig_GameOver orig, BattleGameOverController self) {
    OnLose?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  private static void HookOnKill(On.Everhood.KillModeEvents.orig_NpcKilled orig, KillModeEvents self) {
    OnKill?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  private static void HookOnJump(On.Everhood.Battle.PlayerVerticalMovement.orig_Jump orig, PlayerVerticalMovement self) {
    OnJump?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  private static void HookOnAbsorbNote(On.AbsorbBehaviour.orig_NotifyOnAbsorbSuccess orig, AbsorbBehaviour self) {
    OnAbsorbNote?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  private static void HookOnAbsorbTallNote(On.AbsorbBehaviour.orig_NotifyOnAbsorbTallSuccess orig, AbsorbBehaviour self) {
    OnAbsorbNote?.Invoke(self, EventArgs.Empty);

    orig(self);
  }

  private static void HookOnShootDeflect(On.Everhood.ShootDeflectiveProjectileEventCommand.orig_ShootDeflect orig, ShootDeflectiveProjectileEventCommand self, bool projectileDeflectedIsUnjumpable) {
    OnShootDeflect?.Invoke(self, EventArgs.Empty);

    orig(self, projectileDeflectedIsUnjumpable);
  }
}