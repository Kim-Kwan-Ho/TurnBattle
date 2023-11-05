using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Data;
using UnityEngine.Networking.Match;
using static Define;

public class UI_MatchSystem : UI_Base
{
    enum Buttons
    {
        MatchButton,
        CancelMatchButton
    }

    enum Images
    {
        Character1Image, 
        Character2Image, 
        Character3Image,
    }

    enum GameObjects
    {
        MatchWaitingPanel
    }

    enum Texts
    {
        RatingText,
        MatchTimeText
    }

    private stMatchPlayerInfo _matchInfo = new stMatchPlayerInfo();
    private bool _isMatching = false;
    private float _matchingTime = 0.0f;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));

        _matchInfo.MsgID = MessageID.MatchPlayerInfo;
        _matchInfo.PacketSize = (ushort)Marshal.SizeOf(typeof(stMatchPlayerInfo));
        _matchInfo.ID = Managers.Data.ID;

        GetObject((int)GameObjects.MatchWaitingPanel).SetActive(false);
        GetButton((int)Buttons.MatchButton).gameObject.BindEvent(StartMatching);
        GetButton((int)Buttons.CancelMatchButton).gameObject.BindEvent(StopMatching);

        return true;
    }

    private void StartMatching()
    {
        _isMatching =  true;
        _matchInfo.Matching = true;
        Managers.Network.TcpSendMessage<stMatchPlayerInfo>(_matchInfo);
        GetObject((int)GameObjects.MatchWaitingPanel).SetActive(true);
        _matchingTime = 0;
    }

    private void StopMatching()
    {
        _isMatching = false;
        _matchInfo.Matching = false;
        Managers.Network.TcpSendMessage<stMatchPlayerInfo>(_matchInfo);
        GetObject((int)GameObjects.MatchWaitingPanel).SetActive(false);
    }

    public void RefreshMainCharactersImage()
    {
        GetImage((int)Images.Character1Image).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{Managers.Data.MainCharacters[0].ChID}");
        GetImage((int)Images.Character2Image).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{Managers.Data.MainCharacters[1].ChID}");
        GetImage((int)Images.Character3Image).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{Managers.Data.MainCharacters[2].ChID}");
    }

    private void Update()
    {
        if (Managers.Network.Matched && _isMatching)
        {
            _isMatching = false;
            Managers.UI.CloseAllPopupUI();
            Managers.Scene.ChangeScene(Scene.Battle, true);
        }
        else if (_isMatching)
        {
            _matchingTime += Time.deltaTime;
            GetText((int)Texts.MatchTimeText).text = $"{(int)_matchingTime / 60} : {(int)_matchingTime % 60}";
        }

    }

    


}
