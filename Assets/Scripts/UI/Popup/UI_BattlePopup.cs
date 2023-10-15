using System;
using System.Collections;
using System.Collections.Generic;
using PlayerData;
using System.Runtime.InteropServices;
using ServerData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattlePopup : UI_Popup
{
    enum Buttons
    {
        SettingButton,
        SurrenderButton,
    }
    enum GameObjects
    {
        SettingPanel
    }

    enum Texts
    {
        ActTimeText
    }

    enum CharacterImages
    {
        CharacterImage1,
        CharacterImage2,
        CharacterImage3,
        CharacterImage4,
        CharacterImage5,
        CharacterImage6,
        CharacterBackImage1,
        CharacterBackImage2,
        CharacterBackImage3,
        CharacterBackImage4,
        CharacterBackImage5,
        CharacterBackImage6,
    }
    private TextMeshProUGUI _timeText = null;
    private Image[] _characterImgs = new Image[6];
    private Image[] _characterBackImgs = new Image[6];

    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));
        BindImage(typeof(CharacterImages));

        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(() => GetObject((int)GameObjects.SettingPanel).SetActive(true));
        GetButton((int)Buttons.SurrenderButton).gameObject.BindEvent(SurrenderGame);
        GetObject((int)GameObjects.SettingPanel).SetActive(false);


        for (int i = 0; i < _characterImgs.Length; i++)
        {
            _characterImgs[i] = GetImage(i);
            _characterBackImgs[i] = GetImage(i + 6);
        }


        _timeText = GetText((int)Texts.ActTimeText);
        return true;
    }
    private void Start()
    {

    }

    public void SetSelectTimeText(float time)
    {
        if (_timeText == null)
            return;

        _timeText.text = time.ToString("F1");
        if (time <= 0)
        {
           _timeText.enabled = false;
        }
        else
        {
           _timeText.enabled = true;
        }
    }

    public void SetBattleOrderImages(CharacterTurn[] cts)
    {
        for (int i = 0; i < _characterBackImgs.Length; i++)
        {
            _characterImgs[i].sprite = Managers.Resource.Load<Sprite>($"Sprites/Characters/Battle/{cts[i].Id}");
            _characterBackImgs[i].sprite = Managers.Resource.Load<Sprite>($"Sprites/Characters/Battle/{cts[i].IsPlayerCharacter.ToString()}");
        }
    }

    private void SurrenderGame()
    {
        stBattleParticularInfo info = new stBattleParticularInfo();
        info.MsgID = ServerData.MessageID.BattleParticularInfo;
        info.PacketSize = (ushort)Marshal.SizeOf(info);
        info.ID = Managers.Data.ID;
        info.RoomID = (ushort)Managers.Battle.RoomID;
        info.ParticularInfo = (ushort)ParticularInfo.Surrender;
        Managers.Network.TcpSendMessage<stBattleParticularInfo>(info);
        Managers.Battle.ResetBattle();
        Managers.UI.CloseAllPopupUI();
        Managers.Scene.ChangeScene(Define.Scene.Base, true);
    }


}
