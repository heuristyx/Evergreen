using System;

namespace Evergreen;

public static class BattleAPI {
  public class DamageEventArgs : EventArgs {
    public int damage { get; set; }
  }

  public static event EventHandler<DamageEventArgs> OnDamage;

  internal static void Init() {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(BattleAPI)}");

    // Add events to hooks
    On.Everhood.Battle.BattlePlayer.Damage += HookOnDamage;
  }

  public static void HookOnDamage(On.Everhood.Battle.BattlePlayer.orig_Damage orig, Everhood.Battle.BattlePlayer self, int damage) {
    var args = new DamageEventArgs { damage = damage };
    foreach (EventHandler<DamageEventArgs> e in OnDamage.GetInvocationList()) e?.Invoke(self, args);
    damage = args.damage;

    orig(self, damage);
  }
}