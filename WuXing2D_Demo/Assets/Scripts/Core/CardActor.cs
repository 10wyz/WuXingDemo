using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using System;
using WuXingTool;
using WuXingSkill;
using UnityEngine.UI;
using WuXingDisplay;


namespace WuXingCore
{
    public sealed class CardActor
    {
        /// <summary>
        /// 卡牌从卡组到手牌
        /// </summary>
        /// <param name="gamePlayer"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool DeckToHand(GamePlayer triggerPlayer, GamePlayer targetPlayer, GameObject triggerCard, GameObject targetCard)
        {
            if(targetCard != null)
            {
                targetPlayer.AddHandCard(targetCard);
                targetCard.GetComponent<Card>().OnAction(triggerCard, targetCard, TriggerType.ToHand);
            }
            return true;
        }
        /// <summary>
        /// 卡牌从卡组到场地
        /// </summary>
        /// <param name="gamePlayer"></param>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public static bool DeckToSite(GamePlayer originPlayer, GamePlayer targetPlayer, GameObject targetCard, GameObject targetSite)
        {
            if(targetCard != null)
            {
                targetCard.GetComponent<Card>().BeCome(null);
                targetPlayer.AddSiteCard(targetCard, targetSite);
                targetCard.GetComponent<Card>().OnAction(targetCard, null, TriggerType.Come);
            }
            return true;
        }
        /// <summary>
        /// 卡牌从手牌到场地
        /// </summary>
        /// <param name="gamePlayer"></param>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public static bool HandToSite(GamePlayer originPlayer, GamePlayer targetPlayer, GameObject targetCard, GameObject targetSite)
        {
            targetCard.GetComponent<Card>().BeCome(null);
            originPlayer.RemoveHandCard(targetCard);
            targetPlayer.AddSiteCard(targetCard, targetSite);
            targetCard.GetComponent<Card>().OnAction(null, targetCard, TriggerType.Come);
            
            return true;
        }
        /// <summary>
        /// 卡牌从手牌到弃牌堆
        /// </summary>
        /// <param name="gamePlayer"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool HandToFold(GamePlayer originPlayer, GamePlayer targetPlayer, GameObject targetCard)
        {
            targetPlayer.RemoveHandCard(targetCard);
            targetPlayer.AddDeadCard(targetCard);
            targetCard.GetComponent<Card>().OnAction(null, targetCard, TriggerType.Abondan);
            
            return true;
        }
        /// <summary>
        /// 卡牌从手牌到卡组
        /// </summary>
        /// <param name="gamePlayer"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool HandToDeck(GamePlayer originPlayer, GamePlayer targetPlayer, GameObject targetCard)
        {
            originPlayer.RemoveHandCard(targetCard);
            if (targetPlayer.AddDeck(targetCard))
                GameObject.Destroy(targetCard);
            return true;
        }
        /// <summary>
        /// 卡牌从场地到手牌
        /// </summary>
        /// <param name="originplayer"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public static bool SiteToHand(GamePlayer originPlayer, GamePlayer targetPlayer, GameObject targetCard)
        {
            if(targetCard.GetComponent<Card>().OnAction(null, targetCard, TriggerType.Left))
            {
                if(targetCard.GetComponent<Card>().OnAction(targetCard, null, TriggerType.ToHand))
                {
                    originPlayer.RemoveSiteCard(targetCard, null);
                    targetPlayer.AddHandCard(targetCard);
                }
            }
            
            return true;
        }
        /// <summary>
        /// 卡牌从场地到弃牌堆
        /// </summary>
        /// <param name="originplayer"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public static bool SiteToFold(GamePlayer originPlayer, GamePlayer targetPlayer, GameObject targetCard)
        {
            if(targetCard != null)
            {
                targetPlayer.RemoveSiteCard(targetCard, null);
                targetPlayer.AddDeadCard(targetCard);
                targetCard.GetComponent<Card>().OnAction(null, targetCard, TriggerType.Left);
                //GameListener.Instance().GetSkillControl().RemoveChainSkill(targetCard);
                //GameObject.Destroy(targetCard);
            }
            
            return true;
        }
        /// <summary>
        /// 卡牌从场地到卡组
        /// </summary>
        /// <param name="originplayer"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public static bool SiteToDeck(GamePlayer originPlayer, GamePlayer targetPlayer, GameObject targetCard)
        {
            if (targetCard.GetComponent<Card>().OnAction(null, targetCard, TriggerType.Left))
            {
                originPlayer.RemoveSiteCard(targetCard, null);
                targetPlayer.AddDeck(targetCard);
                GameObject.Destroy(targetCard);
            }
            return true;
        }
        /// <summary>
        /// 手牌替换场上的牌
        /// </summary>
        /// <param name="triggerPlayer"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="triggerCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool HandReplaceSite(GamePlayer triggerPlayer, GamePlayer targetPlayer, GameObject triggerCard, GameObject targetCard)
        {
            GameObject site = targetCard.transform.parent.gameObject;
            CardActor.SiteToFold(triggerPlayer, targetPlayer, targetCard);
            CardActor.HandToSite(triggerPlayer, targetPlayer, triggerCard, site);
            triggerCard.GetComponent<Card>().OnAction(triggerCard, targetCard, TriggerType.Replace);

            return true;
        }



        /// <summary>
        /// 卡牌破坏卡牌
        /// </summary>
        /// <param name="originplayer"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool CardDestoryCard(GamePlayer triggerPlayer, GamePlayer targetPlayer, GameObject triggerCard, GameObject targetCard)
        {
            if(targetCard.GetComponent<Card>().BeDestroy(triggerCard))
            {
                targetPlayer.RemoveSiteCard(targetCard, null);
                targetPlayer.AddDeadCard(targetCard);
                //targetPlayer.AddFold(targetCard);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 卡牌使卡牌离场
        /// </summary>
        /// <param name="originPlayer"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool CardLeftCard(GamePlayer triggerPlayer, GamePlayer targetPlayer, GameObject triggerCard, GameObject targetCard)
        {
            targetPlayer.RemoveSiteCard(targetCard, null);
            targetPlayer.AddDeadCard(targetCard);
            //targetPlayer.AddFold(targetCard);
            return true;
        }
        /// <summary>
        /// 卡牌丢弃手牌
        /// </summary>
        /// <returns></returns>
        public static bool CardAbandonCard(GamePlayer triggerPlayer, GamePlayer targetPlayer, GameObject triggerCard, GameObject targetCard)
        {
            if (targetCard.GetComponent<Card>().BeAbandon(triggerCard))
            {
                targetPlayer.RemoveHandCard(targetCard);
                targetPlayer.AddDeadCard(targetCard);
                //targetPlayer.AddFold(targetCard);
                return true;
            }
            else
            {
                return false;
            }

            
        }
        /// <summary>
        /// 卡牌使卡牌加入手卡
        /// </summary>
        /// <param name="triggerPlayer"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="triggerCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool CardToHandCard(GamePlayer cardPlayer, GamePlayer targetPlayer, GameObject triggerCard, GameObject targetCard)
        {
            if (targetCard != null)
            {
                switch(targetCard.GetComponent<Card>().GetCardType())
                {
                    case CardType.SiteCard:
                        cardPlayer.RemoveSiteCard(null, targetCard);
                        triggerCard.GetComponent<Card>().OnAction(triggerCard, targetCard, TriggerType.Left);
                        break;
                    case CardType.HandCard:
                        cardPlayer.RemoveHandCard(targetCard);
                        triggerCard.GetComponent<Card>().OnAction(triggerCard, targetCard, TriggerType.Abondan);
                        break;
                    case CardType.DeadHand:
                    case CardType.DeadSite:
                        cardPlayer.RemoveDeadCard(targetCard);
                        break;
                }

                targetPlayer.AddHandCard(targetCard);
            }
            return true;
        }
        /// <summary>
        /// 卡牌使卡牌登场
        /// </summary>
        /// <param name="cardPlayer"></param>
        /// <param name="sitePlayer"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public static bool CardComeCard(GamePlayer cardPlayer, GamePlayer sitePlayer, GameObject originCard, GameObject targetCard, GameObject targetSite)
        {
            if(targetCard != null)
            {
                switch (targetCard.GetComponent<Card>().GetCardType())
                {
                    case CardType.SiteCard:
                        cardPlayer.RemoveSiteCard(null, targetCard);
                        originCard.GetComponent<Card>().OnAction(originCard, targetCard, TriggerType.Left);
                        sitePlayer.AddSiteCardFree(targetCard, targetSite);
                        originCard.GetComponent<Card>().OnAction(originCard, targetCard, TriggerType.Come);
                        break;
                    case CardType.HandCard:
                        cardPlayer.RemoveHandCard(targetCard);
                        originCard.GetComponent<Card>().OnAction(originCard, targetCard, TriggerType.Abondan);
                        sitePlayer.AddSiteCardFree(targetCard, targetSite);
                        originCard.GetComponent<Card>().OnAction(originCard, targetCard, TriggerType.Come);
                        break;
                    case CardType.DeadHand:
                    case CardType.DeadSite:
                        cardPlayer.RemoveDeadCard(targetCard);
                        cardPlayer.RemoveHandCard(targetCard);
                        sitePlayer.AddSiteCardFree(targetCard, targetSite);
                        originCard.GetComponent<Card>().OnAction(originCard, targetCard, TriggerType.Come);
                        break;
                }
            }
            return true;
        }
        


        public static bool SetCardResistance(GameObject targetCard, Resistance resistance)
        {
            return targetCard.GetComponent<Card>().SetResistance(resistance);
        }
        public static bool SetCardSkillNum(GameObject targetCard, int index, int num)
        {
            return targetCard.GetComponent<Card>().SetSkillNum(index, num);
        }
        public static bool SetCardSkillConstraint(GameObject targetCard, SkillConstraint skillConstraint)
        {
            return targetCard.GetComponent<Card>().SetSkillConstraint(skillConstraint);
        }
        public static bool SetCardSkillImmune(GameObject targetCard, SkillImmune skillImmune)
        {
            return targetCard.GetComponent<Card>().SetSkillImmune(skillImmune);
        }



    }
}

