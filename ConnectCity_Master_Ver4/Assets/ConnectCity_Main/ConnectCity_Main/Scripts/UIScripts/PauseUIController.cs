using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using Main.Audio;

namespace Main.UI
{
    /// <summary>
    /// ポーズ画面UI操作クラス
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(EventTrigger))]
    [RequireComponent(typeof(ActionMode))]
    [RequireComponent(typeof(Image))]
    public class PauseUIController : MasterUIController
    {
        /// <summary>アクションモード</summary>
        [SerializeField] private ActionMode act;

        protected override void OnEnable()
        {
            if (!act)
                act = GetComponent<ActionMode>();
            base.OnEnable();
        }

        protected override void Initialize()
        {
            //base.Initialize();

            if (!act)
                act = GetComponent<ActionMode>();

            // 各項目によって変わる
            switch (act.pauseMode)
            {
                case PauseActionMode.BackAction:
                    if (!button)
                    {
                        button = GetComponent<Button>();
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
            button.OnCancelAsObservable()
                .Subscribe(_ => Canceled());
        }

        /// <summary>
        /// 選択項目の決定時に呼び出すメソッド
        /// </summary>
        public override void Submited()
        {
            // 各項目によって変わる
            switch (act.pauseMode)
            {
                case PauseActionMode.BackAction:
                    if (_menuClose == false)
                    {
                        _menuClose = true;
                        SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                        UIManager.Instance.CloseMenu();
                        button.enabled = false;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// キャンセル時に呼び出すメソッド
        /// </summary>
        public void Canceled()
        {
            if (_menuClose == false)
            {
                _menuClose = true;
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                UIManager.Instance.CloseMenu();
                button.enabled = false;
            }
        }
    }
}
