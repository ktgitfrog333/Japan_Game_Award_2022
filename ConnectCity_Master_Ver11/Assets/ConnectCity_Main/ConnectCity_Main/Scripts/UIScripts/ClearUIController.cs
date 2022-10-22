using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Main.Audio;
using Main.Common;

namespace Main.UI
{
    /// <summary>
    /// クリア画面UI操作クラス
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(EventTrigger))]
    [RequireComponent(typeof(ActionMode))]
    [RequireComponent(typeof(Image))]
    public class ClearUIController : MasterUIController
    {
        /// <summary>アクションモード</summary>
        [SerializeField] private ActionMode act;
        /// <summary>リトライのSEパターン</summary>
        [SerializeField] private ClipToPlay retrySEPattern = ClipToPlay.se_retry_No1;

        protected override void OnEnable()
        {
            if (!act)
                act = GetComponent<ActionMode>();
            base.OnEnable();
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!act)
                act = GetComponent<ActionMode>();

            // 各項目によって変わる
            switch (act.clearMode)
            {
                case ClearActionMode.RetryAction:
                    if (!button)
                    {
                        button = GetComponent<Button>();
                        var n = new Navigation();
                        if (!GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().FinalStage)
                        {
                            n.mode = Navigation.Mode.Explicit;
                            n.selectOnUp = transform.parent.GetChild((int)ClearActionMode.ProceedAction).GetComponent<Button>();
                            n.selectOnDown = transform.parent.GetChild((int)ClearActionMode.SelectAction).GetComponent<Button>();
                        }
                        else
                        {
                            // T.B.D 最終ステージの設定を適用
                        }
                        button.navigation = n;
                    }

                    break;
                case ClearActionMode.SelectAction:
                    if (!button)
                    {
                        button = GetComponent<Button>();
                        var n = new Navigation();
                        if (!GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().FinalStage)
                        {
                            n.mode = Navigation.Mode.Explicit;
                            n.selectOnUp = transform.parent.GetChild((int)ClearActionMode.RetryAction).GetComponent<Button>();
                            n.selectOnDown = transform.parent.GetChild((int)ClearActionMode.ProceedAction).GetComponent<Button>();
                        }
                        else
                        {
                            // T.B.D 最終ステージの設定を適用
                        }
                        button.navigation = n;
                    }

                    break;
                case ClearActionMode.ProceedAction:
                    if (!button)
                    {
                        button = GetComponent<Button>();
                        var n = new Navigation();
                        n.mode = Navigation.Mode.Explicit;
                        n.selectOnUp = transform.parent.GetChild((int)ClearActionMode.SelectAction).GetComponent<Button>();
                        n.selectOnDown = transform.parent.GetChild((int)ClearActionMode.RetryAction).GetComponent<Button>();
                        button.navigation = n;
                    }

                    break;
                default:
                    Debug.Log("アクションモード未設定");
                    Debug.Log("オブジェクト名:[" + name + "]");
                    break;
            }
        }

        /// <summary>
        /// イベントトリガーを設定する
        /// </summary>
        protected override void EntryEventTrigger()
        {
            button.OnSelectAsObservable()
                .Subscribe(_ => Selected());
            button.OnDeselectAsObservable()
                .Subscribe(_ => Deselected());
            button.OnSubmitAsObservable()
                .Subscribe(_ => Submited());
        }

        /// <summary>
        /// 選択項目の決定時に呼び出すメソッド
        /// </summary>
        public override async void Submited()
        {
            // 各項目によって変わる
            switch (act.clearMode)
            {
                case ClearActionMode.RetryAction:
                    if (_menuClose == false)
                    {
                        _menuClose = true;
                        GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(retrySEPattern);
                        GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SetSceneIdUndo();
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().EnableDrawLoadNowFadeOutTrigger();

                        button.enabled = false;
                        await PlayFlashingMotion();
                        Deselected();
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().CloseClearScreen();
                    }
                    break;
                case ClearActionMode.SelectAction:
                    if (_menuClose == false)
                    {
                        GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(ClipToPlay.se_decided);
                        GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SetSelectSceneNameIdFromMain_Scene();
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().EnableDrawLoadNowFadeOutTrigger();
                        _menuClose = true;
                        button.enabled = false;
                        await PlayFlashingMotion();
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().CloseClearScreen();
                    }
                    break;
                case ClearActionMode.ProceedAction:
                    if (_menuClose == false)
                    {
                        _menuClose = true;
                        GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(ClipToPlay.se_decided);
                        GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SetSceneIdNext();
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().EnableDrawLoadNowFadeOutTrigger();

                        button.enabled = false;
                        await PlayFlashingMotion();
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().CloseClearScreen();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
