using Main.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.UI
{
    /// <summary>
    /// クリア画面制御クラス
    /// </summary>
    public class ClearScreen : MasterScreen
    {
        /// <summary>UI操作スクリプト</summary>
        [SerializeField] private ClearUIController firstElement;
        /// <summary>ステージクリアのテキスト</summary>
        [SerializeField] private string stageClearText = "ステージクリア！";
        /// <summary>オールクリアのテキスト</summary>
        [SerializeField] private string gameAllClearText = "Congratulation！！";

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            Time.timeScale = 0f;
            if (SceneOwner.Instance.FinalStage)
            {
                transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = gameAllClearText;
                // 最終ステージの場合もう一度遊ぶを一番上にする
                firstElement = transform.GetChild(0).GetChild(2).GetComponent<ClearUIController>();
                firstObject = transform.GetChild(0).GetChild(2).gameObject;
            }
            else
            {
                transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = stageClearText;
                // デフォルトは次のステージへ進むを一番上にする
                firstElement = transform.GetChild(0).GetChild(1).GetComponent<ClearUIController>();
                firstObject = transform.GetChild(0).GetChild(1).gameObject;
            }
        }

        protected override void Initialize()
        {
            if (!firstElement)
                firstElement = transform.GetChild(0).GetChild(1).GetComponent<ClearUIController>();
            if (!firstObject)
                firstObject = transform.GetChild(0).GetChild(1).gameObject;
            base.Initialize();
        }

        protected override void SelectFirstElement()
        {
            firstElement.Selected();
        }

        /// <summary>
        /// 最初の項目を選択する外部インターフェース
        /// UIOwnerから呼ばれる想定
        /// </summary>
        public void AutoSelectContent()
        {
            if (!@event)
                @event = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            @event.SetSelectedGameObject(firstObject);
            SelectFirstElement();
        }
    }
}
