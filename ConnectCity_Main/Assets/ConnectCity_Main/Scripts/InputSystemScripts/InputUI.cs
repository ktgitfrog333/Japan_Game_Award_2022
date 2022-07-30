using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.InputSystem
{
    /// <summary>
    /// UI用のInputAction
    /// </summary>
    public class InputUI : MonoBehaviour
    {
        /// <summary>ナビゲーション入力</summaryf>
        private Vector2 _navigated;
        /// <summary>ナビゲーション入力</summaryf>
        public Vector2 Navigated => _navigated;
        /// <summary>
        /// Navigationのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnNavigated(InputAction.CallbackContext context)
        {
            _navigated = context.ReadValue<Vector2>();
        }

        /// <summary>決定入力</summary>
        private bool _submited;
        /// <summary>決定入力</summary>
        public bool Submited => _submited;
        /// <summary>
        /// Pauseのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnSubmited(InputAction.CallbackContext context)
        {
            _submited = context.ReadValueAsButton();
        }

        /// <summary>キャンセル入力</summary>
        private bool _canceled;
        /// <summary>キャンセル入力</summary>
        public bool Canceled => _canceled;
        /// <summary>
        /// Cancelのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnCanceled(InputAction.CallbackContext context)
        {
            _canceled = context.ReadValueAsButton();
        }

        /// <summary>ポーズ入力</summary>
        private bool _paused;
        /// <summary>ポーズ入力</summary>
        public bool Paused => _paused;
        /// <summary>
        /// Pauseのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnPaused(InputAction.CallbackContext context)
        {
            _paused = context.ReadValueAsButton();
        }

        /// <summary>スペース入力</summary>
        private bool _spaced;
        /// <summary>スペース入力</summary>
        public bool Spaced => _spaced;
        /// <summary>
        /// Spaceのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnSpaced(InputAction.CallbackContext context)
        {
            _spaced = context.ReadValueAsButton();
        }

        /// <summary>アンドゥ入力</summary>
        private bool _undoed;
        /// <summary>アンドゥ入力</summary>
        public bool Undoed => _undoed;
        /// <summary>
        /// Undoのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnUndoed(InputAction.CallbackContext context)
        {
            SetStateByPushedAndReleased(context, _undoed);
        }

        /// <summary>セレクト入力</summary>
        private bool _selected;
        /// <summary>セレクト入力</summary>
        public bool Selected => _selected;
        /// <summary>
        /// Selectのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnSelected(InputAction.CallbackContext context)
        {
            SetStateByPushedAndReleased(context, _selected);
        }

        /// <summary>マニュアル入力</summary>
        private bool _manualed;
        /// <summary>マニュアル入力</summary>
        public bool Manualed => _manualed;
        /// <summary>
        /// Manualのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnManualed(InputAction.CallbackContext context)
        {
            SetStateByPushedAndReleased(context, _manualed);
        }

        /// <summary>
        /// ボタン押下、離すステートを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        /// <param name="state">ステート</param>
        private void SetStateByPushedAndReleased(InputAction.CallbackContext context, bool state)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    state = true;
                    break;
                case InputActionPhase.Canceled:
                    state = false;
                    break;
            }
        }
    }
}
