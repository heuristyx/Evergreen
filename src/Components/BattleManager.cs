using UnityEngine;

namespace Evergreen;

public class BattleManager : MonoBehaviour
{
  private void Awake()
  {
    BattleAPI.RaiseBattleStart(this, this.gameObject);
  }

  private void Update()
  {
    BattleAPI.RaiseBattleUpdate(this);
  }
}