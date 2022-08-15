using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using UnityEngine.UI;
using WuXingDisplay;
using System.Threading;

namespace WuXingCore
{
    public class DisplayActor
    {
        /// <summary>
        /// 细节显示选中卡牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool SetFocusCard(GameObject targetCard)
        {
            if (targetCard == null)
                return false;

            if (targetCard.GetComponent<Card>().GetCardCategory() == CardCategory.FakeCard)
                return false;

            GameObject clearCard = GameObject.Find("ClearCard");

            clearCard.GetComponent<DetailCardDisplay>().UpdateImage(targetCard);
            


            return true;
        }
        /// <summary>
        /// 闪烁显示开启
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool OpenTwinkleCard(GameObject targetCard)
        {
            if(GameListener.Instance().IsPlayerControl())
            {
                Card card = targetCard.GetComponent<Card>();
                int flag = targetCard.GetComponent<Animator>().GetInteger("TwinkleEnable");
                
                targetCard.GetComponent<Animator>().SetInteger("TwinkleEnable", flag + 1);

            }
            return true;
        }
        /// <summary>
        /// 闪烁显示关闭
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool CloseTwinkleCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            int flag = card.GetComponent<Animator>().GetInteger("TwinkleEnable");
            if (flag > 0)
                card.GetComponent<Animator>().SetInteger("TwinkleEnable", flag - 1);

            return true;
        }
        /// <summary>
        /// 护盾显示
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool DisplayCardShield(GameObject targetCard)
        {

            Transform shieldTrans = targetCard.transform.Find("Shield");
            GameObject shield = null;
            if (shieldTrans == null)
            {
                shield = SundryCreator.CreateShield(targetCard);
            }
            else
            {
                shield = shieldTrans.gameObject;
            }


            Resistance resistance = targetCard.GetComponent<Card>().GetResistance();
            shield.GetComponent<ShieldDisplay>().ShowShiled(resistance.GetShield());
            return true;
        }



        /// <summary>
        /// 抽出手牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool ExtractHandCard(GameObject targetCard)
        {
            if(GameListener.Instance().IsPlayerControl())
            {
                Transform transform = targetCard.transform;
                float moveLength = targetCard.GetComponent<RectTransform>().rect.height / 5f;
                transform.Translate(Vector3.up * moveLength, Space.Self);
            }
            return true;
        }
        /// <summary>
        /// 放回手牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool PutBackHandCard(GameObject targetCard)
        {
            if (GameListener.Instance().IsPlayerControl())
            {
                Transform transform = targetCard.transform;
                float moveLength = targetCard.GetComponent<RectTransform>().rect.height / 5f;
                transform.Translate(Vector3.down * moveLength, Space.Self);
            }
                
            return true;
        }
        /// <summary>
        /// 显示技能按钮
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool DisplaySkillButton(GameObject targetCard)
        {
            SkillButtonFactory buttonFactory = new SkillButtonFactory();
            GameObject skillbutton = buttonFactory.GetButton("SkillButton", WuXingButton.ButtonTag.Skill);
            skillbutton.name = "SkillButton";

            skillbutton.transform.SetParent(targetCard.transform, false);
            float offset = 10 + skillbutton.GetComponent<RectTransform>().rect.height / 2 + targetCard.GetComponent<RectTransform>().rect.height / 2;
            
            if(GameListener.Instance().IsPlayerControl())   
                skillbutton.transform.localPosition = new Vector3(0, offset, 0);
            else
                skillbutton.transform.localPosition = new Vector3(1000, 1000, 0);

            return true;
        }
        /// <summary>
        /// 消除技能按钮
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool HideSkillButton(GameObject targetCard)
        {
            Transform button = targetCard.GetComponent<Transform>().Find("SkillButton");
            if (button != null)
                GameObject.Destroy(button.gameObject);
            return true;
        }



        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool HideWindow(GameObject window)
        {
            window.GetComponent<RectTransform>().localPosition = new Vector3(1000, 1000, 0);
            return true;
        }
        /// <summary>
        /// 隐藏手牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool HideHandCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            CardInfo cardInfo = card.GetCardInfo();
            string Path = "Picture/Back/卡背-" + cardInfo.m_cardProperty;
            Sprite sp = Resources.Load<Sprite>(Path);
            targetCard.GetComponent<Image>().sprite = sp;

            return true;
        }
        /// <summary>
        /// 显示手牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool DisplayHandCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            CardInfo cardInfo = card.GetCardInfo();
            string Path = null;
            switch (card.GetCardCategory())
            {
                case CardCategory.LeaderCard:
                case CardCategory.RoleCard:
                case CardCategory.ExtraCard:
                    Path = "Picture/Card/" + cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;
                    break;
                case CardCategory.SpellCard:
                    Path = "Picture/Spell/术-" + cardInfo.m_cardName;
                    break;
                default:
                    return true;
            }

            Sprite sp = Resources.Load<Sprite>(Path);
            targetCard.GetComponent<Image>().sprite = sp;

            return true;
        }


        /// <summary>
        /// 技能发动动画显示
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="gameStates"></param>
        /// <returns></returns>
        public static bool DisplaySkillLaunchCard(GameObject originCard)
        {
            Card card = originCard.GetComponent<Card>();
            GameObject targetCard = CardCreator.CreateTempCard(card.GetCardInfo(), card.GetCardCategory(), card.GetPlayerType());

            GameObject parent = GameObject.Find("AnimationWindow").gameObject;
            targetCard.transform.SetParent(parent.transform);

            targetCard.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            targetCard.GetComponent<RectTransform>().localPosition = Vector3.zero;
            targetCard.GetComponent<RectTransform>().localScale = Vector3.one;
            
            targetCard.AddComponent<SkillAnimationDisplay>();

            EventPara eventPara = new EventPara();
            eventPara.m_origin = targetCard;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_animastart, eventPara);

            targetCard.GetComponent<Animator>().SetTrigger("SkillLaunch");
            return true;
        }
        /// <summary>
        /// 选择卡牌强调动画显示
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public static bool DisplaySelectEmphasize(GameObject targetCard)
        {
            targetCard.AddComponent<SelectEmphasizeDisplay>();


            EventPara eventPara = new EventPara();
            eventPara.m_origin = targetCard;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_animastart, eventPara);


            targetCard.GetComponent<Animator>().SetTrigger("SelectEmphasize");
            return true;
        }
        public static bool DisplayLaunchEmphasize(GameObject targetCard)
        {
            targetCard.AddComponent<LaunchEmphasizeDisplay>();


            EventPara eventPara = new EventPara();
            eventPara.m_origin = targetCard;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_animastart, eventPara);


            targetCard.GetComponent<Animator>().SetTrigger("LaunchEmphasize");
            return true;
        }


        /// <summary>
        /// 显示盘属性
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public static bool DisplayFieldProperty(SanCai sanCai, WuXing wuXing, PlayerType playerType)
        {
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<FieldDisplay>().DisplayFieldProperty(sanCai, wuXing, playerType);
            return true;
        }
        /// <summary>
        /// 显示控制玩家
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public static bool DisplayControlPlayer(PlayerType playerType)
        {
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<FieldDisplay>().DisplayControlPlayer(playerType);
            return true;
        }
    }
}

