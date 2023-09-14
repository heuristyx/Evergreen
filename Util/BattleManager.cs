using UnityEngine;

namespace Evergreen;

public class BattleManager : MonoBehaviour {
  private void Update() {
    BattleAPI.HookOnBattleUpdate(this);
  }
}