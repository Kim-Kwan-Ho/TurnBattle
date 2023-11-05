using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCharacterController : BaseCharacterController
{
    private void Update()
    {
        if (IsDeath())
        {
            SetCharacterOutline(OutlineState.None);
            return;
        }

        if (BattleSystem.SelectAttackTarget)
        {
            SetCharacterOutline(OutlineState.SelectTarget);
        }
    }
}
