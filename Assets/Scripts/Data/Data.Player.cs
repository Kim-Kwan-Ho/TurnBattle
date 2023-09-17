using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;

namespace PlayerData
{
    public static class Constants
    {
        public const float SelectTime = 10;


    }
    public class Player
    {
        public string PlayerID;
        public string PlayerPassword;
        public string PlayerName;
        public UInt32 PlayerGold;
        public UInt32 UpgradeToken;
        public UInt32 Power;
        public Character[] PlayerMainCharacters;
        public Dictionary<UInt16, Character> PlayerCharacters;
        public ushort BattleRoomID;
        public bool IsPlayer1;
        public Player()
        {
            PlayerName = "ChangeName";
            PlayerMainCharacters = new Character[3];
            PlayerCharacters = new Dictionary<UInt16, Character>();
        }
    }

    public enum CharacterType
    {
        Warrior,
        Archor,
        Magician,

    }

    public struct StarterCharacters
    {
        public Character[] Characters;
    }

    [Serializable]
    public struct Character
    {
        public UInt16 ChID;
        public UInt16 ChLevel;
        public UInt16 ChHp;
        public UInt16 ChDamage;
        public UInt16 ChArmor;
        public UInt16 ChSpd;
    }
    //[CanBeNull] public WeaponEquipment ChEqWeapon;
    //[CanBeNull] public ArmorEquipment ChEqArmor;
    //[CanBeNull] public ShoeEquipment ChEqShoe;
    //[CanBeNull] public HeadEquipment ChEqHead;
    //[CanBeNull] public GloveEquipment ChEqGlove;
    //public CharacterType ChType;
    public class Equipment
    {
        public int ItemID;
        public string ItemName;
    }

    public class WeaponEquipment : Equipment
    {
        public int WeaponDamage;
    }

    public class ArmorEquipment : Equipment
    {
        public int ArmorHP;
    }

    public class ShoeEquipment : Equipment
    {
        public int ShoeSpeed;
    }

    public class HeadEquipment : Equipment
    {
        public int HeadDefence;
    }

    public class GloveEquipment : Equipment
    {
        public CharacterType increaseType;
        public int DamagePer;
    }


    public enum CharacterState
    {
        Death = 0,
        None = 1,
        Defense = 2,
        Attack = 3,
        SetAttackTarget = 4
    }

    public enum GameState
    {
        ContinueSelect,
        Player1Win,
        Player2Win
    }

    public enum ParticularInfo
    {
        Surrender,
        LogOut
    }

    public struct CharacterTurn
    {
        public int Id;
        public bool IsPlayerCharacter;
    }
}


