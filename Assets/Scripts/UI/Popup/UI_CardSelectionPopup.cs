using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;

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

    private void SetCardSetting()
    {
        Character[] characters = Managers.Data.GetStarterCharacters();
        _cards = new UI_CharacterCard[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            _cards[i] = Managers.Resource.Instantiate("UI/Scene/UI_CharacterCard", GetObject((int)GameObjects.CardLayout).transform)
                .GetComponent<UI_CharacterCard>();
            _cards[i].SetCardInfo(characters[i]);
            _cards[i].OnClick += SelectCard;
        }
    }


    private void SelectCard(UI_CharacterCard card)
    {
        if (card.SelectToggle.isOn)
        {
            if (_selectedCharacters.Count >= 3)
            {
                card.SelectToggle.isOn = false;
                return;
            }
            else
            {
                _selectedCharacters.Add(card.CardCharacter);
            }
        }
        else
        {
            if (_selectedCharacters.Contains(card.CardCharacter))
                _selectedCharacters.Remove(card.CardCharacter);
        }

        GetButton((int)Buttons.ConfirmButton).gameObject.SetActive(_selectedCharacters.Count == 3);
        GetText((int)Texts.CharacterCountText).text = $"({_selectedCharacters.Count}/3)";
    }

    private void ConfirmCharacterCards()
    {
        foreach (Character ch in _selectedCharacters)
        {
            Managers.Data.Characters.Add(ch.ChID, ch);
        }
        Managers.Data.UpdatePlayerInfoToServer();
        ClosePopupUI();
    }
}
