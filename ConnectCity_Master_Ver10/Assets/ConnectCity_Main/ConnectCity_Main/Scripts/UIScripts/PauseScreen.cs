using Main.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.UI
{
    /// <summary>
    /// メニュー画面制御クラス
    /// </summary>
    public class PauseScreen : MasterScreen
    {
        /// <summary>選択項目のUIスクリプト</summary>
        [SerializeField] private PauseUIController firstElement;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            @event.SetSelectedGameObject(firstObject);
            SelectFirstElement();
            Time.timeScale = 0f;
        }

        protected override void Initialize()
        {
            if (!firstElement)
                firstElement = transform.GetChild(0).GetChild(0).GetComponent<PauseUIController>();
            if (!firstObject)
                firstObject = GameObject.Find("GamePause");
            if (!@event)
                @event = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        protected override void SelectFirstElement()
        {
            firstElement.Selected();
        }

        ///// <summary>
        ///// システムから項目を自動で選択させたい場合に使用する
        ///// </summary>
        ///// <param name="mode">ポーズ画面の項目</param>
        //public void AutoSelectContent(PauseActionMode mode)
        //{
        //    var g = transform.GetChild(0).GetChild((int)mode).gameObject;
        //    @event.SetSelectedGameObject(g);
        //    g.GetComponent<PauseUIController>().Selected();
        //}
    }
}
