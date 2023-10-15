using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Game;
        Managers.UI.ShowPopupUI<UI_PlayPopup>();
        if (IsNewAccount()) // 신규계정일 경우 카드선택 팝업 생성
        {
            Managers.UI.ShowPopupUI<UI_CardSelectionPopup>();
        }
        return true;
    }


    private bool IsNewAccount() // 캐릭터를 보유하고 있는지에 따라 신규계정 확인
    {
        for (int i = 0; i < Managers.Data.MainCharacters.Length; i++) 
        {
            if (Managers.Data.MainCharacters[i].ChID != 0) 
                return false;
        }

        if (Managers.Data.Characters.Count > 0)
            return false;

        return true;
    }
}
