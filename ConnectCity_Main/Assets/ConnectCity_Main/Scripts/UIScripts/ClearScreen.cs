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
        /// <summary>ステージクリアのスプライト</summary>
        [SerializeField] private Sprite _stageClear;
        /// <summary>オールクリアのスプライト</summary>
        [SerializeField] private Sprite _gameAllClear;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            Time.timeScale = 0f;
            if (SceneInfoManager.Instance.FinalStage)
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _gameAllClear;
            else
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _stageClear;
        }

        protected override void Initialize()
        {
            if (!firstElement)
                firstElement = transform.GetChild(0).GetChild(1).GetComponent<ClearUIController>();
            if (!firstObject)
                firstObject = GameObject.Find("GameRetryButton");
            base.Initialize();
        }

        protected override void SelectFirstElement()
        {
            firstElement.Selected();
        }

        /// <summary>
        /// 最初の項目を選択する外部インターフェース
        /// UIManagerから呼ばれる想定
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
