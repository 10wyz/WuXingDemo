using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WuXingBase;
using WuXingSkill;
using WuXingNetwork;

namespace WuXingCore
{
    public class NotifyPara
    {
    }
    public class EventPara : NotifyPara
    {
        public GameObject m_origin;
        public GameObject m_trigger;
        public GameObject m_target;
        public TriggerType m_triggerType;
        public PlayerType m_playerType;

    }
    public class StatesPara : NotifyPara
    {
        public GameStates m_gameStates;
    }
    public class ResistPara : NotifyPara
    {
        public GameObject m_target;
        public Resistance m_resistance;
    }
    public class SkillPara : NotifyPara
    {
        public GameObject m_target;
        public int m_index;
        public int m_num;
        public SkillConstraint m_skillConstraint;
    }
    public class CardPara : NotifyPara
    {
        public GameObject m_target;
        public List<SanCai> m_owner;
        public List<WuXing> m_property;
        public PlayerType m_playerType;
        public bool m_isadd;
        public CardPara()
        {
            m_owner = new List<SanCai>();
            m_property = new List<WuXing>();
            m_isadd = true;
        }
    }
    public class PlayerPara : NotifyPara
    {
        public PlayerType m_playerType;
        public List<string> m_camp;
        public bool m_isadd;

        public PlayerPara()
        {
            m_camp = new List<string>();
            m_isadd = true;
        }
    }
    public class SitePara : NotifyPara
    {
        public SiteConstraint m_siteConstraint;
        public PlayerType m_playerType; 
    }
    public class WindowPara : NotifyPara
    {
        public GameObject m_window;
    }
    public class ImmunePara : NotifyPara
    {
        public GameObject m_target;
        public SkillImmune m_skillImmune;
    }

    public enum ControlEventID {evt_game_finish,evt_card_click, evt_stage_end, evt_card_abandon, evt_button_click, evt_skill_cancel}
    public enum ChainEventID 
    {   
        evt_left, 
        evt_destory, 
        evt_come,
        evt_drawspell, 
        evt_endselect, 
        evt_startselect, 
        evt_onskill, 
        evt_handdiscard, 
        evt_endchain, 
        evt_startchain, 
        evt_clear_deadcard,
        evt_drawrole,
        evt_setresistance,
        evt_drawspecific,
        evt_skillnum,
        evt_skillconstraint,
        evt_tohand,
        evt_siteproperty,
        evt_cardpara,
        evt_playerpara,
        evt_siteconstraint,
        evt_epochactive,
        evt_epochclose,
        evt_setwindow,
        evt_showcard,
        evt_setskillimmune,
        evt_animastart,
        evt_animaend,
    }
    public class GameListener
    {
        private static GameListener instance = null;

        private GameStates m_nowState;
        private GamePlayer m_player;
        private GamePlayer m_enemy;

        private SkillControl m_skillControl;
        private ScreenPlay m_screenPlay; 
        private GameNetworkManager m_networkManager;

        public static GameListener Instance()
        {
            if(instance == null)
                instance = new GameListener();

            return instance;
        }
        private GameListener()
        {
            m_nowState = new PlayerReadyStates();
            m_networkManager = null;
        }
        public void ClearListener()
        {
            instance = null;
        }



        /// <summary>
        /// 下个阶段事件
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private bool Event_StageEnd(NotifyPara notifyEvent)
        {
            EventPara eventPara = (EventPara)notifyEvent;
            if (m_nowState.IsChangeStageEnable())
            {
                m_nowState.EndProcess(m_player, m_enemy);
                GameStates playerStates = m_nowState.GetNextStates();
                m_nowState = playerStates;
                eventPara.m_trigger.GetComponent<Transform>().GetComponentInChildren<Text>().text = m_nowState.GetCurrentStatusName();
                m_nowState.PreProcess(m_player, m_enemy);
            }
            
            return true;
        }
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        /// <returns></returns>
        private bool Event_GameFinish()
        {
            m_nowState = new GameFinishStates();
            GameObject.Find("stage").GetComponentInChildren<Text>().text = m_nowState.GetCurrentStatusName();
            return true;
        }
        
        /// <summary>
        /// 进入选择卡牌阶段
        /// </summary>
        /// <param name="gameObject">触发该事件的技能对象</param>
        /// <returns></returns>
        private bool Event_StartSelect(NotifyPara notifyEvent)
        {
            EventPara eventPara = (EventPara)notifyEvent;
            //m_nowState.EndProcess(m_player, m_enemy);
            GameStates playerStates = null;
            if (eventPara.m_playerType == PlayerType.Player)
                playerStates = new PlayerSelectStates(eventPara.m_origin, eventPara.m_trigger, m_nowState);
            else
                playerStates = new EnemySelectStates(eventPara.m_origin, eventPara.m_trigger, m_nowState);

            playerStates.SetBeforeStates(m_nowState);
            m_nowState.SetAfterStates(playerStates);
            m_nowState = playerStates;
            playerStates.PreProcess(m_player, m_enemy);

            return true;
        }
        /// <summary>
        /// 进入连锁选择阶段
        /// </summary>
        /// <param name="notifyEvent"></param>
        /// <returns></returns>
        private bool Event_StartChain(NotifyPara notifyEvent)
        {
            //string temp = "开始连锁：";
            //if (m_nowState.GetBeforeStates() != null)
            //    temp = temp + m_nowState.GetBeforeStates().GetCurrentStatusName() + "/";
            //else
            //    temp += "Null/";
            //temp = temp + m_nowState.GetCurrentStatusName() + "/";
            //if (m_nowState.GetAfterStates() != null)
            //    temp = temp + m_nowState.GetAfterStates().GetCurrentStatusName();
            //else
            //    temp += "Null";
            //Debug.Log(temp);

            EventPara eventPara = (EventPara)notifyEvent;
            GameStates playerStates = null;
            if (eventPara.m_playerType == PlayerType.Player)
            {
                playerStates = new PlayerChainStates(eventPara, m_nowState);
            }
            else
            {
                playerStates = new EnemyChainStates(eventPara, m_nowState);
            }
            playerStates.SetBeforeStates(m_nowState);
            m_nowState.SetAfterStates(playerStates);
            m_nowState = playerStates;
            playerStates.PreProcess(m_player, m_enemy);

            

            return true;
        }
        /// <summary>
        /// 进入动画阶段
        /// </summary>
        /// <param name="notifyEvent"></param>
        /// <returns></returns>
        private bool Event_StartAnima(NotifyPara notifyEvent)
        {
            //string temp = "开始动画：";
            //if (m_nowState.GetBeforeStates() != null)
            //    temp = temp + m_nowState.GetBeforeStates().GetCurrentStatusName() + "/";
            //else
            //    temp += "Null/";
            //temp = temp + m_nowState.GetCurrentStatusName() + "/";
            //if (m_nowState.GetAfterStates() != null)
            //    temp = temp + m_nowState.GetAfterStates().GetCurrentStatusName();
            //else
            //    temp += "Null";
            //Debug.Log(temp);

            EventPara eventPara = (EventPara)notifyEvent;
            GameStates playerStates = null;
            if (m_nowState.GetPlayer(m_player, m_enemy).GetPlayerType() == PlayerType.Player)
                playerStates = new PlayerAnimaStates(eventPara, m_nowState);
            else
                playerStates = new EnemyAnimaStates(eventPara, m_nowState);

            playerStates.SetBeforeStates(m_nowState);
            m_nowState.SetAfterStates(playerStates);
            m_nowState = playerStates;
            playerStates.PreProcess(m_player, m_enemy);

            
            return true;
        }
        /// <summary>
        /// 结束选择卡牌阶段
        /// </summary>
        /// <returns></returns>
        private bool Event_EndSelect(NotifyPara notifyEvent)
        {
            StatesPara statesPara = (StatesPara)notifyEvent;
            if(statesPara.m_gameStates.GetAfterStates() == null)
            {
                GameStates beforeStates = statesPara.m_gameStates.GetBeforeStates();
                beforeStates.SetAfterStates(null);
                m_nowState = beforeStates;
                statesPara.m_gameStates.EndProcess(m_player, m_enemy);
                m_nowState.ReEntry(m_player, m_enemy);
            }
            else
            {
                GameStates beforeStates = statesPara.m_gameStates.GetBeforeStates();
                GameStates afterStates = statesPara.m_gameStates.GetAfterStates();
                beforeStates.SetAfterStates(afterStates);
                afterStates.SetBeforeStates(beforeStates);
                statesPara.m_gameStates.EndProcess(m_player, m_enemy);
            }

            return true;
        }
        /// <summary>
        /// 结束连锁选择阶段
        /// </summary>
        /// <param name="notifyEvent"></param>
        /// <returns></returns>
        private bool Event_EndChain(NotifyPara notifyEvent)
        {
            //string temp = "结束连锁：";
            //if (m_nowState.GetBeforeStates() != null)
            //    temp = temp + m_nowState.GetBeforeStates().GetCurrentStatusName() + "/";
            //else
            //    temp += "Null/";
            //temp = temp + m_nowState.GetCurrentStatusName() + "/";
            //if (m_nowState.GetAfterStates() != null)
            //    temp = temp + m_nowState.GetAfterStates().GetCurrentStatusName();
            //else
            //    temp += "Null";
            //Debug.Log(temp);

            StatesPara statesPara = (StatesPara)notifyEvent;
            if (statesPara.m_gameStates.GetAfterStates() == null)
            {
                GameStates beforeStates = statesPara.m_gameStates.GetBeforeStates();
                beforeStates.SetAfterStates(null);
                m_nowState = beforeStates;
                statesPara.m_gameStates.EndProcess(m_player, m_enemy);
                m_nowState.ReEntry(m_player, m_enemy);
            }
            else
            {
                GameStates beforeStates = statesPara.m_gameStates.GetBeforeStates();
                GameStates afterStates = statesPara.m_gameStates.GetAfterStates();
                beforeStates.SetAfterStates(afterStates);
                afterStates.SetBeforeStates(beforeStates);
                statesPara.m_gameStates.EndProcess(m_player, m_enemy);
            }

            

            return true;
        }
        /// <summary>
        /// 结束动画阶段
        /// </summary>
        /// <param name="notifyEvent"></param>
        /// <returns></returns>
        private bool Event_EndAnima(NotifyPara notifyEvent)
        {
            //string temp = "结束动画：";
            //if (m_nowState.GetBeforeStates() != null)
            //    temp = temp + m_nowState.GetBeforeStates().GetCurrentStatusName() + "/";
            //else
            //    temp += "Null/";
            //temp = temp + m_nowState.GetCurrentStatusName() + "/";
            //if (m_nowState.GetAfterStates() != null)
            //    temp = temp + m_nowState.GetAfterStates().GetCurrentStatusName();
            //else
            //    temp += "Null";
            //Debug.Log(temp);

            StatesPara statesPara = (StatesPara)notifyEvent;
            if (statesPara.m_gameStates.GetAfterStates() == null)
            {
                GameStates beforeStates = statesPara.m_gameStates.GetBeforeStates();
                beforeStates.SetAfterStates(null);
                m_nowState = beforeStates;
                statesPara.m_gameStates.EndProcess(m_player, m_enemy);
                m_nowState.ReEntry(m_player, m_enemy);
            }
            else
            {
                GameStates beforeStates = statesPara.m_gameStates.GetBeforeStates();
                GameStates afterStates = statesPara.m_gameStates.GetAfterStates();
                beforeStates.SetAfterStates(afterStates);
                afterStates.SetBeforeStates(beforeStates);
                statesPara.m_gameStates.EndProcess(m_player, m_enemy);
            }

            

            return true;
        }
        


        /// <summary>
        /// 注册玩家
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        public void Login(GamePlayer player, GamePlayer enemy, SkillControl skillControl, ScreenPlay screenPlay, bool isFirst)
        {
            m_player = player;
            m_enemy = enemy;

            m_skillControl = skillControl;
            m_screenPlay = screenPlay;

            if (isFirst)
            {
                m_nowState = new PlayerReadyStates();
                m_nowState.PreProcess(m_player, m_enemy);
            }
            else
            {
                m_nowState = new EnemyReadyStates();
                m_nowState.PreProcess(m_player, m_enemy);
            }
                

            m_networkManager = GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>();
        }



        /// <summary>
        /// 返回双方玩家场上的所有牌
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public List<GameObject> GetSiteCard()
        {
            List<GameObject> siteCard = new List<GameObject>();
            siteCard.AddRange(m_player.GetSiteCardAll());
            siteCard.AddRange(m_enemy.GetSiteCardAll());
            return siteCard;
        }
        /// <summary>
        /// 返回双方玩家手牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetHandCard()
        {
            List<GameObject> handCard = new List<GameObject>();
            handCard.AddRange(m_player.GetHandCardAll());
            handCard.AddRange(m_enemy.GetHandCardAll());
            return handCard;
        }
        /// <summary>
        /// 返回双方玩家死亡卡牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetDeadCard()
        {
            List<GameObject> deadCard = new List<GameObject>();
            deadCard.AddRange(m_player.GetDeadCardAll());
            deadCard.AddRange(m_enemy.GetDeadCardAll());
            return deadCard;
        }
        /// <summary>
        /// 获取对应玩家场上阵营数量
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetCampNum(PlayerType playerType)
        {
            switch(playerType)
            {
                case PlayerType.Player:
                    return m_player.GetCampNum();
                case PlayerType.Enemy:
                    return m_enemy.GetCampNum();
                default:
                    return null;
            }
        }
        public SkillControl GetSkillControl()
        {
            return m_skillControl;
        }
        public CardDeck GetCardDeck()
        {
            return m_player.GetCardDeck();
        }
        /// <summary>
        /// 获得卡组中的角色卡
        /// </summary>
        /// <returns></returns>
        public List<CardInfo> GetDeckRoleCardAll()
        {
            return m_player.GetDeckRoleCardAll();
        }
        /// <summary>
        /// 获得卡组中的额外卡
        /// </summary>
        /// <returns></returns>
        public List<CardInfo> GetDeckExtraCardAll()
        {
            return m_player.GetDeckExtraCardAll();
        }
        /// <summary>
        /// 获取卡牌所属玩家
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public GamePlayer GetPlayer(GameObject target)
        {
            Card card = target.GetComponent<Card>();
            if(card.GetPlayerType() == m_player.GetPlayerType())
            {
                return m_player;
            }
            else
            {
                return m_enemy;
            }
        }
        public GamePlayer GetPlayer()
        {
            return m_player;
        }
        /// <summary>
        /// 获取卡牌所属的敌对玩家
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public GamePlayer GetEnemy(GameObject target)
        {
            Card card = target.GetComponent<Card>();
            if (card.GetPlayerType() != m_player.GetPlayerType())
            {
                return m_player;
            }
            else
            {
                return m_enemy;
            }
        }
        /// <summary>
        /// 获取场地标志牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetSiteSignal()
        {
            List<GameObject> list = new List<GameObject>();
            list.AddRange(m_player.GetSiteSignal());
            list.AddRange(m_enemy.GetSiteSignal());
            return list;
        }
        /// <summary>
        /// 获取纪行卡牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetEpochCard()
        {
            List <GameObject> list = new List<GameObject>();
            list.AddRange(m_player.GetEpochCard());
            list.AddRange(m_enemy.GetEpochCard());
            return list;
        }
        /// <summary>
        /// 获得剧本特定时间标志
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetScreenEventFlag(int index)
        {
            return m_screenPlay.IsConditionEnable(index);
        }
        /// <summary>
        /// 返回网络管理器
        /// </summary>
        /// <returns></returns>
        public GameNetworkManager GetGameNetworkManager()
        {
            return m_networkManager;
        }


        /// <summary>
        /// 当前时我方玩家控制阶段
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerControl()
        {
            return m_nowState.GetPlayer(m_player, m_enemy).GetPlayerType() == PlayerType.Player;
        }
        public bool IsChainEnable()
        {
            return m_nowState.IsChainEnable();
        }
        /// <summary>
        /// 是否正在动画中
        /// </summary>
        /// <returns></returns>
        public bool IsAnimation()
        {
            return m_nowState.GetType() == typeof(AnimaStates);
        }


        /// <summary>
        /// 控制事件通知（对玩家操作的响应）
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public bool ControlEventNotify(ControlEventID eventID, NotifyPara notifyPara)
        {
            switch (eventID)
            {
                case ControlEventID.evt_stage_end:
                    Event_StageEnd(notifyPara);
                    return true;
                case ControlEventID.evt_game_finish:
                    Event_GameFinish();
                    return true;
                default:
                    m_nowState.ControlEventNotify(eventID, notifyPara, m_player, m_enemy);
                    return false;
            }
        }
        /// <summary>
        /// 连锁事件通知（对游戏对象的响应）
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public bool ChainEventNotify(ChainEventID eventID, NotifyPara notifyPara)
        {
            switch(eventID)
            {
                case ChainEventID.evt_startselect:
                    Event_StartSelect(notifyPara);
                    return true;
                case ChainEventID.evt_startchain:
                    Event_StartChain(notifyPara);
                    return true;
                case ChainEventID.evt_endselect:
                    Event_EndSelect(notifyPara);
                    return true;
                case ChainEventID.evt_endchain:
                    Event_EndChain(notifyPara);
                    return true;
                case ChainEventID.evt_animastart:
                    Event_StartAnima(notifyPara);
                    return true;
                case ChainEventID.evt_animaend:
                    Event_EndAnima(notifyPara);
                    return true;
                default:
                    return m_nowState.ChainEventNotify(eventID, notifyPara, m_player, m_enemy);
            }
        }
    }
}

