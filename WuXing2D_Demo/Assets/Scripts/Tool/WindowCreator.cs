using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingAssetManage;
using WuXingButton;
using WuXingWindow;
using WuXingCore;
using WuXingBase;

namespace WuXingTool
{
    public class WindowCreator
    {
        /// <summary>
        /// 构造技能选择窗口
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static GameObject CreateSkillSelectWinodw(GameObject targetCard, List<int> index)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 200 / 255f);
            windowBuilder.SetMainWindow("技能选择窗口", 400, 200, color);

            windowBuilder.AddText("Description", new Vector2(320, 120), new Vector2(0, 0), new Vector2(0.5f, 0.5f), "技能描述", 25, TextAnchor.UpperLeft);
            windowBuilder.AddButton("确定", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "确定", new NormalButtonFatory(), ButtonTag.SkillSeleckOk);
            windowBuilder.AddButton("取消", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "取消", new NormalButtonFatory(), ButtonTag.SkillSelectCancel);


            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            List<string> text = new List<string>();
            for (int i = 0; i < index.Count; i++)
            {
                Card card = targetCard.GetComponent<Card>();
                text.Add(card.GetSkillDecription(index[i]));
            }
            window.AddComponent<SkillSelectWindow>();
            window.GetComponent<SkillSelectWindow>().Initial(index, text);


            if (!GameListener.Instance().IsPlayerControl())
                DisplayActor.HideWindow(window);

            return window;
        }
        /// <summary>
        /// 构造连锁确定窗口
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateChainCheckWindow()
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("连锁确定窗口", 200, 100, color);
            windowBuilder.AddText("Text", new Vector2(150, 40), new Vector2(0, 0.2f), new Vector2(0.5f, 0.5f), "有可发动技能，是否发动");
            windowBuilder.AddButton("确定", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "确定", new NormalButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("取消", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "取消", new NormalButtonFatory(), ButtonTag.ChainCheckCancel);


            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<ChainCheckWindow>();


            if (!GameListener.Instance().IsPlayerControl())
                DisplayActor.HideWindow(window);

            return window;
        }
        /// <summary>
        /// 构造卡牌选择窗口
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static GameObject CreateCardSelectWindow(List<GameObject> card)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("卡牌选择窗口", 400, 200, color);
            windowBuilder.AddScrollbar("滚动条", new Vector2(400, 20), new Vector2(0, -1), new Vector2(0.5f, 0));
            windowBuilder.AddButton("关闭", new Vector2(40, 40), new Vector2(1, 1), new Vector2(1, 1), "", new NormalButtonFatory("Circle"), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<CardSelectWindow>();
            window.GetComponent<CardSelectWindow>().Initial(card);

            if (!GameListener.Instance().IsPlayerControl())
                DisplayActor.HideWindow(window);

            return window;
        }
        /// <summary>
        /// 构造技能回滚窗口
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateRollBackWindow(int flag)
        {
            WindowBuilder windowBuilder = new WindowBuilder();

            if (flag == 0)
            {
                Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
                windowBuilder.SetMainWindow("技能回滚窗口", 200, 100, color);
                windowBuilder.AddText("Text", new Vector2(180, 40), new Vector2(0, 0.3f), new Vector2(0.5f, 0.5f), "未选择达到需求量的卡牌，是否回退技能");
                windowBuilder.AddButton("回退", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "回退", new NormalButtonFatory(), ButtonTag.SkillRollBackOk);
                windowBuilder.AddButton("继续发动", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "继续发动", new NormalButtonFatory(), ButtonTag.SkillRollBackForce);
            }
            else
            {
                Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
                windowBuilder.SetMainWindow("技能回滚窗口", 200, 100, color);
                windowBuilder.AddText("Text", new Vector2(180, 40), new Vector2(0, 0.3f), new Vector2(0.5f, 0.5f), "未选择达到需求量的卡牌，是否继续发动");
                windowBuilder.AddButton("继续发动", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "继续发动", new NormalButtonFatory(), ButtonTag.SkillRollBackForce);
                windowBuilder.AddButton("取消", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "取消", new NormalButtonFatory(), ButtonTag.Normal);
            }

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<RollBackWindow>();


            if (!GameListener.Instance().IsPlayerControl())
                DisplayActor.HideWindow(window);

            return window;
        }
        /// <summary>
        /// 构造选项选择窗口
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static GameObject CreateOptionSelectWindow(List<string> options, int num, int type)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("选项选择窗口", 300, 150, color);
            windowBuilder.AddButton("确定", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "确定", new NormalButtonFatory(), ButtonTag.OptionSelectOk);
            windowBuilder.AddButton("取消", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "取消", new NormalButtonFatory(), ButtonTag.OptionSelectCancel);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<OptionSelectWindow>();
            window.GetComponent<OptionSelectWindow>().Initial(options, num, type);


            if (!GameListener.Instance().IsPlayerControl())
                DisplayActor.HideWindow(window);

            return window;
        }
        /// <summary>
        /// 构造卡组窗口
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateCardDeckWindow(CardDeck cardDeck, int num)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("卡组窗口", 560, 600, color);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<RectTransform>().localPosition;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<CardDeckWindow>();
            window.GetComponent<CardDeckWindow>().Initial(cardDeck, num);

            if (!GameListener.Instance().IsPlayerControl())
                DisplayActor.HideWindow(window);

            return window;
        }




        /// <summary>
        /// 构造创建房间窗口
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateNewRoomWindow(GameObject parent)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("创建房间窗口", 300, 150, color);
            windowBuilder.AddInputField("房间名", new Vector2(200, 30), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f));
            windowBuilder.AddButton("创建", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "创建", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("取消", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "取消", new MenuButtonFatory(), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<CreateRoomWindow>();

            return window;
        }
        /// <summary>
        /// 构造加入房间窗口
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateJoinRoomWindow(GameObject parent)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("加入房间窗口", 300, 150, color);
            windowBuilder.AddInputField("房间名", new Vector2(200, 30), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f));
            windowBuilder.AddButton("加入", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "加入", new MenuButtonFatory(), ButtonTag.SkillSeleckOk);
            windowBuilder.AddButton("取消", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "取消", new MenuButtonFatory(), ButtonTag.SkillSelectCancel);

            GameObject window = windowBuilder.GetWindow();
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<JoinRoomWindow>();

            return window;
        }
        /// <summary>
        /// 构造房间窗口
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateRoomWindow(GameObject parent)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("房间窗口", 1000, 600, color);
            windowBuilder.AddText("RoomNameTitle", new Vector2(200, 100), new Vector2(-0.9f, 0.8f), new Vector2(0, 0.5f), "房间名", 60);
            windowBuilder.AddText("RoomName", new Vector2(500, 100), new Vector2(-0.5f, 0.8f), new Vector2(0, 0.5f), "", 60);
            windowBuilder.AddText("MasterTitle", new Vector2(200, 100), new Vector2(-0.9f, 0.4f), new Vector2(0, 0.5f), "主机名", 60);
            windowBuilder.AddText("PlayerTitle", new Vector2(200, 100), new Vector2(-0.9f, 0), new Vector2(0, 0.5f), "客机名", 60);
            windowBuilder.AddText("MasterName", new Vector2(500, 100), new Vector2(-0.5f, 0.4f), new Vector2(0, 0.5f), "", 60);
            windowBuilder.AddText("PlayerName", new Vector2(500, 100), new Vector2(-0.5f, 0), new Vector2(0, 0.5f), "", 60);
            windowBuilder.AddText("MasterReady", new Vector2(200, 100), new Vector2(0.5f, 0.4f), new Vector2(0, 0.5f), "未就绪", 60);
            windowBuilder.AddText("PlayerReady", new Vector2(200, 100), new Vector2(0.5f, 0), new Vector2(0, 0.5f), "未就绪", 60);
            windowBuilder.AddButton("开始游戏", new Vector2(200, 60), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "开始游戏", new MenuButtonFatory(), ButtonTag.SkillSeleckOk);
            windowBuilder.AddButton("退出房间", new Vector2(200, 60), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "退出房间", new MenuButtonFatory(), ButtonTag.SkillSelectCancel);


            GameObject window = windowBuilder.GetWindow();
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<RoomWindow>();

            return window;
        }
        /// <summary>
        /// 构造选择先后手窗口
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateSelectSenteWindow()
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("先后手选择窗口", 300, 150, color);
            windowBuilder.AddText("Tip", new Vector2(250, 40), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), "", 20);
            windowBuilder.AddButton("先手", new Vector2(85, 30), new Vector2(-0.5f, 0), new Vector2(0.5f, 0.5f), "先手", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("后手", new Vector2(85, 30), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f), "后手", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("确定", new Vector2(85, 30), new Vector2(0, -0.95f), new Vector2(0.5f, 0), "确定", new MenuButtonFatory(), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<SelectSenteWindow>();

            return window;
        }
        /// <summary>
        /// 构造选择阴阳派系窗口
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateSelectCliqueWindow()
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("派系选择窗口", 300, 150, color);
            windowBuilder.AddText("Tip", new Vector2(250, 40), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), "", 20);
            windowBuilder.AddButton("阳", new Vector2(85, 30), new Vector2(-0.5f, 0), new Vector2(0.5f, 0.5f), "阳", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("阴", new Vector2(85, 30), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f), "阴", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("确定", new Vector2(85, 30), new Vector2(0, -0.95f), new Vector2(0.5f, 0), "确定", new MenuButtonFatory(), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("GameWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<SelectCliqueWindow>();

            return window;
        }
        /// <summary>
        /// 构造图像设置窗口
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateImageSettingWindow(GameObject parent)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 166 / 255f);
            windowBuilder.SetMainWindow("图像设置窗口", 300, 150, color);

            windowBuilder.AddDropdown("分辨率", new Vector2(150, 30), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f));
            windowBuilder.AddDropdown("全屏设置", new Vector2(150, 30), new Vector2(0f, 0.1f), new Vector2(0.5f, 0.5f));
            windowBuilder.AddButton("确定", new Vector2(85, 30), new Vector2(-0.5f, -0.95f), new Vector2(0.5f, 0), "确定", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("取消", new Vector2(85, 30), new Vector2(0.5f, -0.95f), new Vector2(0.5f, 0), "取消", new MenuButtonFatory(), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<ImageSettingWindow>();

            return window;
        }
        /// <summary>
        /// 构造游戏内菜单窗口
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateInGameMenuWindow(GameObject parent)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 1);
            windowBuilder.SetMainWindow("菜单窗口", 300, 500, color);
            windowBuilder.AddButton("返回游戏", new Vector2(200, 50), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), "返回游戏", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("图像设置", new Vector2(200, 50), new Vector2(0, 0), new Vector2(0.5f, 0.5f), "图像设置", new MenuButtonFatory(), ButtonTag.Normal);
            windowBuilder.AddButton("投降", new Vector2(200, 50), new Vector2(0, -0.5f), new Vector2(0.5f, 0.5f), "投降", new MenuButtonFatory(), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<InGameMenuWindow>();

            return window;
        }
        /// <summary>
        /// 胜利显示窗口
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateGameOverWindow(string text)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 0);
            windowBuilder.SetMainWindow("游戏结束窗口", 400, 200, color);
            windowBuilder.AddText("胜利标志", new Vector2(400, 100), new Vector2(0, 0), new Vector2(0.5f, 0.5f), text, 60, TextAnchor.MiddleCenter);
            windowBuilder.AddButton("确定", new Vector2(85, 30), new Vector2(0, -0.95f), new Vector2(0.5f, 0), "确定", new MenuButtonFatory(), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("MenuWindow");
            window.transform.SetParent(parent.transform);
            window.transform.SetSiblingIndex(0);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<GameOverWindow>();

            return window;
        }
        /// <summary>
        /// 事纪卡牌一览窗口
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public static GameObject CreateEpochBrowseWindow(GamePlayer player, GamePlayer enemy)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 1);
            windowBuilder.SetMainWindow("事件浏览窗口", 475, 300, color);
            windowBuilder.AddScrollbar("滚动条", new Vector2(475, 20), new Vector2(0, -1), new Vector2(0.5f, 0));
            windowBuilder.AddButton("关闭", new Vector2(40, 40), new Vector2(1, 1), new Vector2(1, 1), "", new MenuButtonFatory("Circle"), ButtonTag.Normal);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("TipWindow");
            window.transform.SetParent(parent.transform);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;


            window.AddComponent<EpochBrowseWindow>();
            window.GetComponent<EpochBrowseWindow>().Initial(player, enemy);

            return window;
        }


        public static GameObject CreateWaitReconnectWindow(int time, string name)
        {
            WindowBuilder windowBuilder = new WindowBuilder();
            Color color = new Color(105 / 255f, 88 / 255f, 236 / 255f, 1);
            windowBuilder.SetMainWindow("等待重连窗口", 400, 200, color);
            windowBuilder.AddText("胜利标志", new Vector2(400, 100), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), name, 35, TextAnchor.MiddleCenter);
            windowBuilder.AddText("时间显示", new Vector2(400, 100), new Vector2(0, -0.5f), new Vector2(0.5f, 0.5f), "", 60, TextAnchor.MiddleCenter);

            GameObject window = windowBuilder.GetWindow();
            GameObject parent = GameObject.Find("MenuWindow");
            window.transform.SetParent(parent.transform);
            window.transform.SetSiblingIndex(0);
            window.GetComponent<RectTransform>().localPosition = Vector3.zero;
            window.GetComponent<RectTransform>().localScale = Vector3.one;

            window.AddComponent<WaitReconnectWindow>();
            window.GetComponent<WaitReconnectWindow>().Initial(time);

            return window;
        }
    }
}

