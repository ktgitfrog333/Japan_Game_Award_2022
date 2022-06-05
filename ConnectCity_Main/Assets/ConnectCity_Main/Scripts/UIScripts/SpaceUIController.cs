using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Main.Audio;
using Main.Common;

namespace Main.UI
{
    /// <summary>
    /// 空間操作可能な境界UI
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class SpaceUIController : MasterUIController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Initialize()
        {
            if (button == null)
                button = GetComponent<Button>();
        }

        /// <summary>
        /// イベントトリガーを設定する
        /// </summary>
        protected override void EntryEventTrigger()
        {
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
            if (!Close()) Debug.Log("操作エラー");
        }

        /// <summary>
        /// キャンセル時に呼び出すメソッド
        /// </summary>
        public void Canceled()
        {
            if (!Close()) Debug.Log("操作エラー");
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool Close()
        {
            if (_menuClose == false)
            {
                _menuClose = true;
                GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(ClipToPlay.se_cancel);
                GameManager.Instance.UIOwner.GetComponent<UIOwner>().CloseSpaceScreen();
                button.enabled = false;
            }

            return true;
        }
    }
}
