using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class UI_CardSelectionPopup : UI_Popup
{
    enum Buttons
    {
        ConfirmButton
    }
    enum GameObjects
    {
        CardLayout,
    }

    enum Texts
    {
        CharacterCountText
    }
    private UI_CharacterCard[] _cards = null;
    private List<Character> _selectedCharacters = new List<Character>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(ConfirmCharacterCards);
        GetButton((int)Buttons.ConfirmButton).gameObject.SetActive(false);

        SetCardSetting();
        return true;
    }



    private void SetCardSetting() // 카드 생성 및 설정
    {
        Character[] characters = Managers.Data.GetStarterCharacters(); // json에 저장된 데이터를 통해
                                                                       // 신규 플레이어에게 주어지는 카드 저장
        _cards = new UI_CharacterCard[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            _cards[i] = Managers.Resource.Instantiate
                    ("UI/Scene/UI_CharacterCard", GetObject((int)GameObjects.CardLayout).transform)
                .GetComponent<UI_CharacterCard>(); // 캐릭터 카드 생성
            _cards[i].SetCardInfo(characters[i]); // 캐릭터 카드 정보 삽입
            _cards[i].OnClick += SelectCard; // Action<UI_CharacterCard> OnClick에 캐릭터 카드 선택 함수 추가
        }
    }

    private void SelectCard(UI_CharacterCard card) // 캐릭터 카드 선택 (카드는 Toggle ValueChange시 OnClick 호출)
    {
        if (card.SelectToggle.isOn) // 선택된 카드가 3개 이상 있을 시 false 반환, 3개보다 적을 경우 List에 삽입
        {
            if (_selectedCharacters.Count >= Constants.MainCharacterCount)
            {
                card.SelectToggle.isOn = false;
                return;
            }
            else
            {
                _selectedCharacters.Add(card.CardCharacter);
            }
        }
        else // Toggle Off시 선택된 카드가 List에 있을 경우 리스트에서 제거
        {
            if (_selectedCharacters.Contains(card.CardCharacter))
                _selectedCharacters.Remove(card.CardCharacter);
        }
        // 선택된 카드가 3개일시 Confirm버튼 활성화
        GetButton((int)Buttons.ConfirmButton).gameObject.SetActive(_selectedCharacters.Count == Constants.MainCharacterCount); 
        GetText((int)Texts.CharacterCountText).text = $"({_selectedCharacters.Count}/{Constants.MainCharacterCount})";
    }

    private void ConfirmCharacterCards() // 선택한 캐릭터 카드 확정 및 서버에 업로드
    {
        foreach (Character ch in _selectedCharacters)
        {
            Managers.Data.Characters.Add(ch.ChID, ch); // 플레이어에 캐릭터 추가
        }
        Managers.Data.UpdatePlayerInfoToServer(); // 추가된 캐릭터를 서버에 업데이트
        ClosePopupUI();
    }
}
