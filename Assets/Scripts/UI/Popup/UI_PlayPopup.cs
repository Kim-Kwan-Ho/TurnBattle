using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;
public class UI_PlayPopup : UI_Popup
{
    enum Buttons
    {
        InvenButton,
        HomeButton,
        PVPButton,
        Ch1Button,
        Ch2Button,
        Ch3Button,

    }

    enum GameObjects
    {
        InfoClick,
        HomeClick,
        PVPClick,
        InfoTab,
        HomeTab,
        PVPTab,
        UI_CHSelect,
        UI_Inven,
        UI_MatchSystem,

    }

    enum Texts
    {
        GoldText,
        UpgradeText,
        //PowerText,

    }
    enum PlayTab
    {
        None,
        Info,
        Home,
        PVP,
    }

    private PlayTab _tab = PlayTab.None;
    private UI_Inven _inven;
    private UI_MatchSystem _matchSystem;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        
        GetButton((int)Buttons.HomeButton).gameObject.BindEvent(() => ShowTab(PlayTab.Home));
        GetButton((int)Buttons.InvenButton).gameObject.BindEvent(() => ShowTab(PlayTab.Info));
        GetButton((int)Buttons.PVPButton).gameObject.BindEvent(() => ShowTab(PlayTab.PVP));
        
        GetButton((int)Buttons.Ch1Button).gameObject.BindEvent(() => OpenCharacterInfo(1));
        GetButton((int)Buttons.Ch2Button).gameObject.BindEvent(() => OpenCharacterInfo(2));
        GetButton((int)Buttons.Ch3Button).gameObject.BindEvent(() => OpenCharacterInfo(3));
        
        _inven = GetObject((int)GameObjects.UI_Inven).GetComponent<UI_Inven>();
        _matchSystem = GetObject((int)GameObjects.UI_MatchSystem).GetComponent<UI_MatchSystem>();
        return true;
    }

    private void Start()
    {
        RefreshUI();
        ShowTab(PlayTab.Home);
    }


    public void RefreshUI()
    {
        GetObject((int)GameObjects.HomeClick).gameObject.SetActive(true);
        GetText((int)Texts.GoldText).text = Managers.Data.Gold.ToString();
        GetText((int)Texts.UpgradeText).text = Managers.Data.Token.ToString();
        //GetText((int)Texts.PowerText).text = Managers.Data.Power.ToString();
        RefreshMainCharactersImage();
        _inven.RefreshSlot();
        _matchSystem.RefreshMainCharactersImage();
    }

    private void RefreshMainCharactersImage()
    {
        if (Managers.Data.MainCharacters[0].ChID != 0)
            GetButton((int)Buttons.Ch1Button).image.sprite =
                Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{Managers.Data.MainCharacters[0].ChID}");
        else
            Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/Null");

        if (Managers.Data.MainCharacters[1].ChID != 0)
            GetButton((int)Buttons.Ch2Button).image.sprite =
                Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{Managers.Data.MainCharacters[1].ChID}");
        else
            Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/Null");

        if (Managers.Data.MainCharacters[2].ChID != 0)
            GetButton((int)Buttons.Ch3Button).image.sprite =
                Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{Managers.Data.MainCharacters[2].ChID}");
        else
            Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/Null");
    }

    private void ShowTab(PlayTab tab)
    {
        if (_tab == tab)
            return;
        
        _tab = tab;

        GetObject((int)GameObjects.HomeClick).gameObject.SetActive(true);
        GetObject((int)GameObjects.InfoClick).gameObject.SetActive(true);
        GetObject((int)GameObjects.PVPClick).gameObject.SetActive(true);
        GetObject((int)GameObjects.HomeTab).gameObject.GetComponent<Canvas>().enabled = false;
        GetObject((int)GameObjects.InfoTab).gameObject.GetComponent<Canvas>().enabled = false;
        GetObject((int)GameObjects.PVPTab).gameObject.GetComponent<Canvas>().enabled = false;

        switch (_tab)
        {
            case PlayTab.Home:
                GetObject((int)GameObjects.HomeClick).gameObject.SetActive(false);
                GetObject((int)GameObjects.HomeTab).gameObject.GetComponent<Canvas>().enabled = true;
                break;
                    
                    
            case PlayTab.Info:
                GetObject((int)GameObjects.InfoClick).gameObject.SetActive(false);
                GetObject((int)GameObjects.InfoTab).gameObject.GetComponent<Canvas>().enabled = true;
                break;

            case PlayTab.PVP:
                GetObject((int)GameObjects.PVPClick).gameObject.SetActive(false);
                GetObject((int)GameObjects.PVPTab).gameObject.GetComponent<Canvas>().enabled = true;
                break;
        }
    }

    private void OpenCharacterInfo(int index)
    {
        GetObject((int)GameObjects.UI_CHSelect).GetComponent<UI_CHSelect>().SetChsInfo(index);
        GetObject((int)GameObjects.UI_CHSelect).GetComponent<Canvas>().sortingOrder = Managers.UI.GetPopupStack() + 1;
    }

}
