using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingSkill;
using WuXingBase;
using WuXingCore;

namespace WuXingTool
{
    public class SkillTranslate
    {
        public static TriggerPhase GetTriggerPhase(string triggerPhase)
        {
            switch (triggerPhase)
            {
                case "Initial":
                    return TriggerPhase.Initial;
                case "Normal":
                    return TriggerPhase.Normal;
                case "Final":
                    return TriggerPhase.Final;
                default:
                    return TriggerPhase.Final;
            }
        }
        public static WuXing GetWuXing(string wuXing)
        {
            switch(wuXing)
            {
                case "金":
                    return WuXing.Metal;
                case "木":
                    return WuXing.Wood;
                case "水":
                    return WuXing.Water;
                case "火":
                    return WuXing.Fire;
                case "土":
                    return WuXing.Earth;
                case "纪":
                    return WuXing.Epoch;
                case "术":
                    return WuXing.Spell;
                default:
                    return WuXing.Chaos;
            }
        }
        public static SanCai GetSanCai(string sanCai)
        {
            switch(sanCai)
            {
                case "天":
                    return SanCai.Sky;
                case "地":
                    return SanCai.Earth;
                case "人":
                    return SanCai.Human;
                default:
                    return SanCai.Earth;
            }
        }

        public static PlayerType GetPlayerType(string playerType, PlayerType myplayerType)
        {
            switch(playerType)
            {
                case "Player":
                    return myplayerType;
                case "Enemy":
                    if(myplayerType == PlayerType.Player)
                        return PlayerType.Enemy;
                    else
                        return PlayerType.Player;
                default:
                    return PlayerType.Player;
            }
        }
        public static CardType GetCardType(string cardType)
        {
            switch (cardType)
            {
                case "DeckCard":
                    return CardType.DeckCard;
                case "SiteCard":
                    return CardType.SiteCard;
                case "HandCard":
                    return CardType.HandCard;
                case "ExtraCard":
                    return CardType.ExtraCard;
                case "CardDeck":
                    return CardType.CardDeck;
                case "DeadSite":
                    return CardType.DeadSite;
                case "DeadHand":
                    return CardType.DeadHand;
                case "SiteSignal":
                    return CardType.SiteSignal;
                default:
                    return CardType.HandCard;
            }
        }
        public static CardCategory GetCardCategory(string cardCategory)
        {
            switch (cardCategory)
            {
                case "RoleCard":
                    return CardCategory.RoleCard;
                case "ExtraCard":
                    return CardCategory.ExtraCard;
                case "LeaderCard":
                    return CardCategory.LeaderCard;
                case "FakeCard":
                    return CardCategory.FakeCard;
                case "EpochCard":
                    return CardCategory.EpochCard;
                case "SpellCard":
                    return CardCategory.SpellCard;
                default:
                    return CardCategory.FakeCard;
            }
        }
        public static SelectType GetSelectType(string selectType)
        {
            switch (selectType)
            {
                case "Auto":
                    return SelectType.Auto;
                case "Manual":
                    return SelectType.Manual;
                default:
                    return SelectType.Auto;
            }
        }
        public static TriggerType GetTriggerType(string triggerType)
        {
            switch (triggerType)
            {
                case "Attack":
                    return TriggerType.Attack;
                case "Damage":
                    return TriggerType.Damage;
                case "Come":
                    return TriggerType.Come;
                case "Exist":
                    return TriggerType.Exist;
                case "Left":
                    return TriggerType.Left;
                case "ToHand":
                    return TriggerType.ToHand;
                case "Abondan":
                    return TriggerType.Abondan;
                case "Replace":
                    return TriggerType.Replace;
                case "Click":
                    return TriggerType.Click;
                case "Skill":
                    return TriggerType.Skill;
                case "StageReady":
                    return TriggerType.StageReady;
                case "StageDraw":
                    return TriggerType.StageDraw;
                case "StageMain":
                    return TriggerType.StageMain;
                case "StageEnd":
                    return TriggerType.StageEnd;
                case "StageAbondan":
                    return TriggerType.StageAbondan;
                case "Rinne":
                    return TriggerType.Rinne;
                default:
                    return TriggerType.Click;
            }
        }
        public static TargetSource GetTargetSource(string targetSource)
        {
            switch(targetSource)
            {
                case "Constant":
                    return TargetSource.Constant;
                case "Trigger":
                    return TargetSource.Trigger;
                case "Target":
                    return TargetSource.Target;
                case "Origin":
                    return TargetSource.Origin;
                case "Select":
                    return TargetSource.Select;
                default:
                    return TargetSource.Constant;
            }
        }
        public static SkillStage GetSkillStage(string skillStage)
        {
            switch (skillStage)
            {
                case "OnSite":
                    return SkillStage.OnSite;
                case "OnHand":
                    return SkillStage.OnHand;
                case "FreeSite":
                    return SkillStage.FreeSite;
                case "FreeHand":
                    return SkillStage.FreeHand;
                case "OnDeadSite":
                    return SkillStage.OnDeadSite;
                case "OnDeadHand":
                    return SkillStage.OnDeadHand;
                case "FreeDeadSite":
                    return SkillStage.FreeDeadSite;
                case "FreeDeadHand":
                    return SkillStage.FreeDeadHand;
                case "NoChainSite":
                    return SkillStage.NoChainSite;
                case "NoChainHand":
                    return SkillStage.NoChainHand;
                default:
                    return SkillStage.NoChainSite;
            }
        }
        public static bool GetExcute(string excute)
        {
            switch(excute)
            {
                case "Player":
                    return true;
                case "Enemy":
                    return false;
                default:
                    return true;
            }
        }
    }
}

