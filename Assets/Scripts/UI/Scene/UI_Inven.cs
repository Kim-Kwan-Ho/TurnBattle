using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Base
{
    enum GameObjects
    {
        Content, // 스크롤 - 콘텐츠
    }

    private UI_InvenChInfo[] _mainInfos = null;
    private UI_InvenChInfo[] _chInfos = null;
    private GameObject _content;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        _mainInfos = new UI_InvenChInfo[Managers.Data.MainCharacters.Length];

        BindObject(typeof(GameObjects));
        _content = GetObject((int)GameObjects.Content);

        return true;
    }

    private void Start()
    {
        SetChsInfo();
    }


    private void SetChSlot()
    {

        int chCount = Managers.Data.Characters.Count + Managers.Data.MainCharacters.Length;
        int chInfoCount = _content.transform.childCount;
        _chInfos = new UI_InvenChInfo[chCount - Managers.Data.MainCharacters.Length];
        if (chInfoCount < chCount)
        {
            for (int i = chInfoCount; i < chCount; i++)
            {
                Managers.Resource.Instantiate("UI/Scene/UI_InvenChInfo", _content.transform);
            }
        }
        else if (chInfoCount > chCount)
        {
            for (int i = chCount; i < chInfoCount; i++)
            {
                Managers.Resource.Destroy(_content.transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < Managers.Data.MainCharacters.Length; i++)
        {
            _mainInfos[i] = _content.transform.GetChild(i).GetComponent<UI_InvenChInfo>();
        }

        for (int i = Managers.Data.MainCharacters.Length; i < chCount; i++)
        {
            _chInfos[i - Managers.Data.MainCharacters.Length] =
                _content.transform.GetChild(i).GetComponent<UI_InvenChInfo>();
        }


        ResizeContent();
    }

    public void SetChsInfo()
    {
        SetChSlot();
        for (int i = 0; i < Managers.Data.MainCharacters.Length; i++)
        {
            if (Managers.Data.MainCharacters[i].ChID != 0)
            {
                _mainInfos[i].gameObject.SetActive(true);
                _mainInfos[i].SetCharacter(Managers.Data.MainCharacters[i], true);
            }
            else
            {
                _mainInfos[i].gameObject.SetActive(false);
            }
        }

        int k = 0;
        foreach (var ch in Managers.Data.Characters)
        {
            _chInfos[k].SetCharacter(ch.Value);
            k++;
        }
    }

    public void RefreshSlot()
    {
        SetChsInfo();
    }

    private void ResizeContent()
    {
        int childCount = _content.transform.childCount - Managers.Data.MainCharacters.Length;
        int rows = (childCount + 2) / 3;
        int height = 700 * rows + 20;
        _content.GetComponent<RectTransform>().sizeDelta = new Vector2(1020, height);
    }


}
