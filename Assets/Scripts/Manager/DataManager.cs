using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Data;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

public class DataManager
{
    private Player _player = new Player();

    public string ID => _player.ID;

    public UInt32 Gold
    {
        get => _player.Gold;
    }

    public UInt32 Token
    {
        get => _player.UpgradeToken;
    }


    public Character[] MainCharacters
    {
        get => _player.MainCharacters;
        set => _player.MainCharacters = value;
    }

    public Dictionary<UInt16, Character> Characters
    {
        get => _player.OwnedCharacters;
        set => _player.OwnedCharacters = value;
    }


    public void LoadPlayerInfo(stPlayerInfo playerInfo)
    {
        SetPlayerInfo(playerInfo);
        SetCharactersInfo(playerInfo);
    }

    private void SetPlayerInfo(stPlayerInfo playerInfo)
    {
        _player.ID = playerInfo.ID;
        _player.Password = playerInfo.Password;
        _player.Gold = playerInfo.GoldAmount;
        _player.UpgradeToken = playerInfo.TokenCount;
    }

    private void SetCharactersInfo(stPlayerInfo playerInfo)
    {
        _player.MainCharacters = ConvertToPlayerCharacters(playerInfo.MainCharacters);
        _player.OwnedCharacters =
            ConvertToPlayerCharacters(playerInfo.OwnedCharacters, playerInfo.CharacterCount).ToDictionary(key => key.ChID, item => item);
    }


    private Character[] ConvertToPlayerCharacters(stCharacterInfo[] chInfos, ushort chCount = 3) 
    {
        Character[] chs = new Character[chCount];

        for (int i = 0; i < chs.Length; i++)
        {
            chs[i].ChID = chInfos[i].ChID;
            chs[i].ChLevel = chInfos[i].ChLevel;
            chs[i].ChHp = chInfos[i].ChHp;
            chs[i].ChDamage = chInfos[i].ChDamage;
            chs[i].ChArmor = chInfos[i].ChArmor;
            chs[i].ChSpd = chInfos[i].ChSpd;
        }
        return chs;
    }
    private stCharacterInfo[] ConvertToServerCharacters(Character[] chInfos, int chCount = 3)
    {
        stCharacterInfo[] chs = new stCharacterInfo[chCount];

        for (int i = 0; i < chs.Length; i++)
        {
            chs[i].ChID = chInfos[i].ChID;
            chs[i].ChLevel = chInfos[i].ChLevel;
            chs[i].ChHp = chInfos[i].ChHp;
            chs[i].ChDamage = chInfos[i].ChDamage;
            chs[i].ChArmor = chInfos[i].ChArmor;
            chs[i].ChSpd = chInfos[i].ChSpd;
        }
        return chs;
    }
    public void UpdatePlayerInfoToServer()
    {
        stPlayerInfo info = new stPlayerInfo();

        info.MsgID = MessageID.PlayerInfo;
        info.PacketSize = (ushort)Marshal.SizeOf(typeof(stPlayerInfo));
        info.ID = _player.ID;
        info.Password = _player.Password;
        info.GoldAmount = _player.Gold;
        info.TokenCount = _player.UpgradeToken;
        info.MainCharacters = ConvertToServerCharacters(_player.MainCharacters);
        info.CharacterCount = (ushort)_player.OwnedCharacters.Count;
        info.OwnedCharacters = ConvertToServerCharacters(_player.OwnedCharacters.Values.ToArray(), _player.OwnedCharacters.Count);

        Managers.Network.TcpSendMessage<stPlayerInfo>(info);
    }


    public Character[] GetStarterCharacters()
    {
        Character[] chs = new Character[6];

        var sChs = Managers.Resource.GetObjectByJson<StarterCharacters>(nameof(StarterCharacters));

        for (int i = 0; i < sChs.Characters.Length; i++)
        {
            chs[i] = sChs.Characters[i];
        }


        return chs;
    }



}
