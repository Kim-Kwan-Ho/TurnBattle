using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlayerData;
using ServerData;
using TMPro;
using UnityEngine;
using static Utils;

public class UI_CHSelect : UI_Base
{
    enum Buttons
    {
        ExitButton, // 닫기 버튼
        ChangeButton // 캐릭터 선택 변경버튼
    }
    enum GameObjects
    {
        Content, // 스크롤 - 콘텐츠
    }
    [Header("Popup")]
    private UI_PlayPopup _play = null; // 부모 팝업

    [Header("CH Infos")]
    private UI_HomeChInfo _mainInfo = null;
    private UI_HomeChInfo[] _chInfos = null;
    private UI_HomeChInfo _selectedInfo = null;

    [Header("Canvas")]
    private Canvas _canvas = null;
    private int _curIndex = 0;
    private GameObject _content = null;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _play = GetComponentInParent<UI_PlayPopup>();
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(() => _canvas.enabled = false);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(() => _canvas.sortingOrder = -100);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(() => _selectedInfo = null);
        GetButton((int)Buttons.ChangeButton).gameObject.BindEvent(ChangeMainCharacter);

        _content = GetObject((int)GameObjects.Content);

        return true;
    }

    private void SetChSlot() // 캐릭터 슬롯 설정
    {

        int chCount = Managers.Data.Characters.Count + 1;
        int chInfoCount = _content.transform.childCount;
        _chInfos = new UI_HomeChInfo[chCount-1];
        if (chInfoCount < chCount)
        {
            for (int i = chInfoCount; i < chCount; i++)
            {
                Managers.Resource.Instantiate("UI/Scene/UI_HomeChInfo",_content.transform);
            }
        }
        else if (chInfoCount > chCount)
        {
            for (int i = chCount; i < chInfoCount; i++)
            {
                Managers.Resource.Destroy(_content.transform.GetChild(i).gameObject);
            }
        }

        _mainInfo = _content.transform.GetChild(0).GetComponent<UI_HomeChInfo>();
        for (int i = 0; i < chCount - 1; i++)
        {
            _chInfos[i] = _content.transform.GetChild(i + 1).GetComponent<UI_HomeChInfo>();
        }

        ResizeContent();
    }

    public void SetChsInfo(int index) // 캐릭터 슬롯 정보 설정
    {
        _canvas.enabled = true;
        SetChSlot();

        _curIndex = index - 1;
        int i = 0;

        foreach (var ch in Managers.Data.Characters)
        {
            _chInfos[i].SetCharacter(ch.Value);
            _chInfos[i].SetMainCharacter(false);
            i++;
        }
        if (Managers.Data.MainCharacters[index - 1].ChID == 0)
        {
            _mainInfo.gameObject.SetActive(false);
            return;
        }
        if (_mainInfo != null)
        {
            _mainInfo.gameObject.SetActive(true);
            SetMainInfo(Managers.Data.MainCharacters[index - 1]);
        }




    }

    private void SetMainInfo(Character character)
    {
        _mainInfo.SetCharacter(character, true);
        _mainInfo.SetMainCharacter(true);
        _mainInfo.transform.SetAsFirstSibling();
        _selectedInfo = _mainInfo;
    }

    private void ResizeContent() // 스크롤 사이즈 조정
    {
        _content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, _content.transform.childCount * 200 + 20);
    }

    public void ChangeSelect(UI_HomeChInfo info) // 캐릭터 선택 변경 (표시만)
    {
        if (_selectedInfo != null)
        {
            _selectedInfo.SelectCharacter();

        }
        _selectedInfo = info;

    }

    private void ChangeMainCharacter() // 캐릭터 변경 (정보 + UI)
    {
        if (_mainInfo == _selectedInfo || _selectedInfo == null)
            return;

        if (_mainInfo != null && _mainInfo.isActiveAndEnabled)
        {
            Managers.Data.Characters.Remove(_mainInfo.GetCharacter().ChID);
            Managers.Data.Characters.Add(_mainInfo.GetCharacter().ChID, _mainInfo.GetCharacter());
            _mainInfo.SetMainCharacter(false);
        }
        
        _mainInfo = _selectedInfo;
        _mainInfo.SetMainCharacter(true);
        Managers.Data.MainCharacters[_curIndex] = _mainInfo.GetCharacter();
        Managers.Data.Characters.Remove(_mainInfo.GetCharacter().ChID);
        _play.RefreshUI();
        Managers.Data.UpdatePlayerInfoToServer();
    }
}

