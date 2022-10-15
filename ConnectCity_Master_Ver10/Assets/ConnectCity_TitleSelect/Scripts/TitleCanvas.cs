using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace TitleSelect
{
    /// <summary>
    /// UIの制御
    /// </summary>
    public class TitleCanvas : MonoBehaviour
    {
        /// <summary>プッシュゲームスタートロゴ</summary>
        [SerializeField] private GameObject pushGameStartPanel;
        /// <summary>ゲーム開始／終了のロゴ</summary>
        [SerializeField] private GameObject gameStartExitPanel;
        /// <summary>ゲーム終了のはい／いいえのロゴ</summary>
        [SerializeField] private GameObject gameExitConfirmPanel;
        /// <summary>カーソルのアイコン</summary>
        [SerializeField] private GameObject pausePencilLogoImage;
        /// <summary>カーソルのアイコン位置の調整（ゲーム開始／ゲーム終了）</summary>
        [SerializeField] private float curPosOffsetGameStartExit = 360f;
        /// <summary>カーソルのアイコン位置の調整（はい／いいえ）</summary>
        [SerializeField] private float curPosOffsetGameExitConfirm = 264f;
        /// <summary>フェードのキャンバス</summary>
        [SerializeField] private GameObject fadeInOutCanvas;
        /// <summary>イベントシステム</summary>
        private EventSystem _eventSystem;

        private void Reset()
        {
            if (pushGameStartPanel == null)
                pushGameStartPanel = GameObject.Find("PushGameStartPanel");
            if (gameStartExitPanel == null)
                gameStartExitPanel = GameObject.Find("GameStartExitPanel");
            if (gameExitConfirmPanel == null)
                gameExitConfirmPanel = GameObject.Find("GameExitConfirmPanel");
            if (pausePencilLogoImage == null)
                pausePencilLogoImage = GameObject.Find("PausePencilLogoImage");
            if (fadeInOutCanvas == null)
                fadeInOutCanvas = GameObject.Find("FadeInOutCanvas");
        }

        private void Awake()
        {
            _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        private void Start()
        {
            // マウスボタンが押されたら最初の項目を固定で選択する
            Cursor.visible = false;

            // イベント登録
            var pushState = SetButtonEvent(pushGameStartPanel.transform, false);
            var gStartExitState =  SetButtonEvent(gameStartExitPanel.transform, false);
            var gExitConfirmState = SetButtonEvent(gameExitConfirmPanel.transform, true);

            if (pushState == null || gStartExitState == null || gExitConfirmState == null)
                Debug.LogError("ボタンイベント設定の失敗");

            // プッシュゲームスタート
            pushState[0].ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    if (x.Equals((int)EventCommand.AnyKeysPushed))
                    {
                        OpenClosePanel(false, pushGameStartPanel);
                        OpenClosePanel(true, gameStartExitPanel);
                        SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                    }
                });
            // ゲームを開始
            gStartExitState[0].ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    switch ((EventCommand)x)
                    {
                        case EventCommand.Selected:
                            pausePencilLogoImage.SetActive(true);
                            pausePencilLogoImage.transform.position = gameStartExitPanel.transform.GetChild(0).position + Vector3.left * curPosOffsetGameStartExit;
                            break;
                        case EventCommand.Canceled:
                            pausePencilLogoImage.SetActive(false);
                            OpenClosePanel(false, gameStartExitPanel);
                            OpenClosePanel(true, pushGameStartPanel);
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                            break;
                        case EventCommand.Submited:
                            Observable.FromCoroutine<bool>(observer => fadeInOutCanvas.transform.GetChild(0).GetComponent<FadeInOut>().Fadeout(observer))
                                .Subscribe(_ => SceneManager.LoadScene("SelectScene"))
                                .AddTo(gameObject);
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                            break;
                        default:
                            break;
                    }
                });
            // ゲームを終了
            gStartExitState[1].ObserveEveryValueChanged(x => x.Value)
               .Subscribe(x =>
                {
                    switch ((EventCommand)x)
                    {
                        case EventCommand.Selected:
                            pausePencilLogoImage.SetActive(true);
                            pausePencilLogoImage.transform.position = gameStartExitPanel.transform.GetChild(1).position + Vector3.left * curPosOffsetGameStartExit;
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            break;
                        case EventCommand.DeSelected:
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            break;
                        case EventCommand.Canceled:
                            pausePencilLogoImage.SetActive(false);
                            OpenClosePanel(false, gameStartExitPanel);
                            OpenClosePanel(true, pushGameStartPanel);
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                            break;
                        case EventCommand.Submited:
                            OpenClosePanel(false, gameStartExitPanel);
                            OpenClosePanel(true, gameExitConfirmPanel, true);
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                            break;
                        default:
                            break;
                    }
                });
            // ゲームを終了しますか？　＞　はい
            gExitConfirmState[0].ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    switch ((EventCommand)x)
                    {
                        case EventCommand.Selected:
                            pausePencilLogoImage.SetActive(true);
                            pausePencilLogoImage.transform.position = gameExitConfirmPanel.transform.GetChild(1).position + Vector3.left * curPosOffsetGameExitConfirm;
                            break;
                        case EventCommand.Canceled:
                            pausePencilLogoImage.SetActive(false);
                            OpenClosePanel(false, gameExitConfirmPanel);
                            OpenClosePanel(true, gameStartExitPanel);
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                            break;
                        case EventCommand.Submited:
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                            Observable.FromCoroutine<bool>(observer => fadeInOutCanvas.transform.GetChild(0).GetComponent<FadeInOut>().Fadeout(observer))
                                .Subscribe(_ => Quit())
                                .AddTo(gameObject);
                            break;
                        default:
                            break;
                    }
                });
            // ゲームを終了しますか？　＞　いいえ
            gExitConfirmState[1].ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    switch ((EventCommand)x)
                    {
                        case EventCommand.Selected:
                            pausePencilLogoImage.SetActive(true);
                            pausePencilLogoImage.transform.position = gameExitConfirmPanel.transform.GetChild(2).position + Vector3.left * curPosOffsetGameExitConfirm;
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            break;
                        case EventCommand.DeSelected:
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            break;
                        case EventCommand.Canceled:
                            pausePencilLogoImage.SetActive(false);
                            OpenClosePanel(false, gameExitConfirmPanel);
                            OpenClosePanel(true, gameStartExitPanel);
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                            break;
                        case EventCommand.Submited:
                            pausePencilLogoImage.SetActive(false);
                            OpenClosePanel(false, gameExitConfirmPanel);
                            OpenClosePanel(true, gameStartExitPanel);
                            SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                            break;
                        default:
                            break;
                    }
                });

            // 最初に必要な項目意外は無効
            OpenClosePanel(true, pushGameStartPanel);
            OpenClosePanel(false, gameStartExitPanel);
            OpenClosePanel(false, gameExitConfirmPanel);
            OpenClosePanel(false, pausePencilLogoImage);
        }

        /// <summary>
        /// ゲームシステムをシャットダウン
        /// </summary>
        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
#endif

        }

        /// <summary>
        /// プッシュゲームスタートを開く
        /// </summary>
        /// <param name="openClose">開く（true）／閉じる（false）</param>
        /// <param name="panelObject">パネルオブジェクト</param>
        private void OpenClosePanel(bool openClose, GameObject panelObject)
        {
            OpenClosePanel(openClose, panelObject, false);
        }

        /// <summary>
        /// プッシュゲームスタートを開く
        /// </summary>
        /// <param name="openClose">開く（true）／閉じる（false）</param>
        /// <param name="panelObject">パネルオブジェクト</param>
        /// <param name="titleFind">子要素の一番目にタイトルがあるか</param>
        private void OpenClosePanel(bool openClose, GameObject panelObject, bool titleFind)
        {
            panelObject.SetActive(openClose);
            if (openClose)
                SetOnEnabledFirstSelected(_eventSystem, panelObject.transform, titleFind);
        }

        /// <summary>
        /// パネル有効になった際のデフォルト選択をイベント登録
        /// </summary>
        /// <param name="eventSystemm">イベントシステム</param>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="titleFind">子要素の一番目にタイトルがあるか</param>
        private void SetOnEnabledFirstSelected(EventSystem eventSystemm, Transform parent, bool titleFind)
        {
            eventSystemm.SetSelectedGameObject(parent.transform.GetChild(titleFind ? 1 : 0).gameObject);
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
                if (parent.Equals(pushGameStartPanel.transform))
                    child.GetComponent<EventController>().AnyKeys();

                btnEventStateList.Add(child.GetComponent<EventController>().EventRP);
            }

            return (btnEventStateList != null && 0 < btnEventStateList.Count) ? btnEventStateList.ToArray() : null;
        }
    }
}
