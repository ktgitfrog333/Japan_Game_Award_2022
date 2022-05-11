using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TitleSelect
{
    /// <summary>
    /// UIの制御
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary>プッシュゲームスタートロゴ</summary>
        [SerializeField] private GameObject pushGameStartPanel;
        /// <summary>ゲーム開始／終了のロゴ</summary>
        [SerializeField] private GameObject gameStartExitPanel;
        /// <summary>ゲーム終了のはい／いいえのロゴ</summary>
        [SerializeField] private GameObject gameExitConfirmPanel;
        /*[SerializeField] */private EventSystem _eventSystem;

        private void Reset()
        {
            if (pushGameStartPanel == null)
                pushGameStartPanel = GameObject.Find("PushGameStartPanel");
            if (gameStartExitPanel == null)
                gameStartExitPanel = GameObject.Find("GameStartExitPanel");
            if (gameExitConfirmPanel == null)
                gameExitConfirmPanel = GameObject.Find("GameExitConfirmPanel");
        }

        private void Awake()
        {
            _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            SetOnEnabledFirstSelected(_eventSystem, pushGameStartPanel.transform, false);
            pushGameStartPanel.SetActive(false);
            //SetOnEnabledFirstSelected(_eventSystem, gameStartExitPanel.transform, false);
            //SetOnEnabledFirstSelected(_eventSystem, gameExitConfirmPanel.transform, true);
        }

        /// <summary>
        /// パネル有効になった際のデフォルト選択をイベント登録
        /// </summary>
        /// <param name="eventSystemm">イベントシステム</param>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="titleFind">子要素の一番目にタイトルがあるか</param>
        private void SetOnEnabledFirstSelected(EventSystem eventSystemm, Transform parent, bool titleFind)
        {
            parent.OnEnableAsObservable()
                //.Do(_ => Debug.Log("有効"))
                .Subscribe(_ =>
                {
                    //Debug.Log(parent.transform.GetChild(titleFind ? 1 : 0).gameObject);
                    //Debug.Log(eventSystemm);
                    eventSystemm.SetSelectedGameObject(parent.transform.GetChild(titleFind ? 1 : 0).gameObject);
                });
        }

        private void Start()
        {
            // イベント登録
            var pushState = SetButtonEvent(pushGameStartPanel.transform, false);
            var gStartExitState =  SetButtonEvent(gameStartExitPanel.transform, false);
            var gExitConfirmState = SetButtonEvent(gameExitConfirmPanel.transform, true);

            if (pushState == null || gStartExitState == null || gExitConfirmState == null)
                Debug.LogError("ボタンイベント設定の失敗");

            pushState[0].ObserveEveryValueChanged(x => x.Value)
                .Do(_ => Debug.Log(pushGameStartPanel))
                .Subscribe(x => Debug.Log(x));
            gStartExitState[0].ObserveEveryValueChanged(x => x.Value)
                .Do(_ => Debug.Log(gameStartExitPanel))
                .Subscribe(x => Debug.Log(x));
            gStartExitState[1].ObserveEveryValueChanged(x => x.Value)
                .Do(_ => Debug.Log(gameStartExitPanel))
                .Subscribe(x => Debug.Log(x));
            gExitConfirmState[0].ObserveEveryValueChanged(x => x.Value)
                .Do(_ => Debug.Log(gameExitConfirmPanel))
                .Subscribe(x => Debug.Log(x));
            gExitConfirmState[1].ObserveEveryValueChanged(x => x.Value)
                .Do(_ => Debug.Log(gameExitConfirmPanel))
                .Subscribe(x => Debug.Log(x));

            // 最初に必要な項目意外は無効
            pushGameStartPanel.SetActive(true);
            gameStartExitPanel.SetActive(false);
            gameExitConfirmPanel.SetActive(false);
        }

        /// <summary>
        /// 子要素ごとのボタンイベントを登録する
        /// </summary>
        /// <param name="parent">親</param>
        /// <param name="titleFind">タイトル要素（ボタンとしての機能が無いもの）があるか</param>
        private IntReactiveProperty[] SetButtonEvent(Transform parent, bool titleFind)
        {
            var btnEventStateList = new List<IntReactiveProperty>();
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);

                // 最初の項目がタイトルならスキップ
                if (titleFind && i == 0)
                    continue;
                // イベント登録
                var btn = child.GetComponent<Button>();
                btn.OnSelectAsObservable()
                    .Subscribe(_ => child.GetComponent<EventController>().Selected());
                btn.OnDeselectAsObservable()
                    .Subscribe(_ => child.GetComponent<EventController>().DeSelected());
                btn.OnSubmitAsObservable()
                    .Subscribe(_ => child.GetComponent<EventController>().Submited());
                btn.OnCancelAsObservable()
                    .Subscribe(_ => child.GetComponent<EventController>().Canceled());

                btnEventStateList.Add(child.GetComponent<EventController>().EventRP);
            }

            return (btnEventStateList != null && 0 < btnEventStateList.Count) ? btnEventStateList.ToArray() : null;
        }
    }
}
