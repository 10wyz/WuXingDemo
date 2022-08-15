using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;

namespace WuXingCore
{
    public class DeckActor
    {
        /// <summary>
        /// 从目标角色卡组抽一张卡
        /// </summary>
        /// <param name="player"></param>
        /// <param name="targetDeck"></param>
        /// <returns></returns>
        public static GameObject DrawRoleCard(GamePlayer player, GameObject targetDeck)
        {
            return player.DrawRoleCard(targetDeck);
        }
        /// <summary>
        /// 从目标角色卡组抽一张卡，不考虑上限
        /// </summary>
        /// <param name="player"></param>
        /// <param name="targetDeck"></param>
        /// <returns></returns>
        public static GameObject DrawRoleCardFree(GamePlayer player, GameObject targetDeck)
        {
            return player.DrawRoleCardFree(targetDeck);
        }
        /// <summary>
        /// 从卡组抽取目标卡牌，不考虑上限
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        public static GameObject DrawSpecificCard(GamePlayer player, CardInfo cardInfo)
        {
            return player.DrawSpecificCard(cardInfo);
        }


        /// <summary>
        /// 从主角卡组获得所有可抽取主角卡
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static List<GameObject> DrawLeaderCard(GamePlayer player)
        {
            return player.DrawLeaderCard();
        }
        /// <summary>
        /// 从法术卡组抽取一张法术卡
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static GameObject DrawSpellCard(GamePlayer player)
        {
            return player.DrawSpellCard();
        }
    }

}
