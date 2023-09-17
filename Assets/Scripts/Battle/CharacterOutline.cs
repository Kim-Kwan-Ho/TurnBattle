using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum OutlineState
{
    SelectCharacter, CharacterSelected, SelectTarget, None // 행동 선택, 캐릭터 선택, 상대 선택, 기다림 or 죽음
}
public class CharacterOutline : MonoBehaviour
{
    private SpriteRenderer _spr = null;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
    }

    public void SetOutlineColor(OutlineState state)
    {
        switch (state)
        {
            case OutlineState.SelectCharacter:
                _spr.enabled = true;
                _spr.color = Color.white;
                break;
            case OutlineState.CharacterSelected:
                _spr.enabled = true;
                _spr.color = Color.green;
                break;
            case OutlineState.SelectTarget:
                _spr.enabled = true;
                _spr.color = Color.red;
                break;
            case OutlineState.None:
                _spr.enabled = false;
                break;
        }
    }


}
