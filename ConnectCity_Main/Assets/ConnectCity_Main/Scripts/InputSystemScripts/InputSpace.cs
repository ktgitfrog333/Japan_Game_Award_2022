using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.InputSystem
{
    /// <summary>
    /// 空間操作用のInputAction
    /// </summary>
    public class InputSpace : MonoBehaviour, IInputSystemsOwner
    {
        /// <summary>左空間アクセル入力</summary>
        private bool _manualLAxcel;
        /// <summary>左空間アクセル入力</summary>
        public bool ManualLAxcel => _manualLAxcel;
        /// <summary>
        /// ManualLAxcelのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnManualLAxcel(InputAction.CallbackContext context)
        {
            _manualLAxcel = context.ReadValueAsButton();
        }

        /// <summary>右空間アクセル入力</summary>
        private bool _manualRAxcel;
        /// <summary>右空間アクセル入力</summary>
        public bool ManualRAxcel => _manualRAxcel;
        /// <summary>
        /// ManualRAxcelのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnManualRAxcel(InputAction.CallbackContext context)
        {
            _manualRAxcel = context.ReadValueAsButton();
        }

        /// <summary>左空間キーボード操作入力</summary>
        private Vector2 _manualLMove;
        /// <summary>左空間キーボード操作入力</summary>
        public Vector2 ManualLMove => _manualLMove;
        /// <summary>
        /// ManualLMoveのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnManualLMove(InputAction.CallbackContext context)
        {
            _manualLMove = context.ReadValue<Vector2>();
        }

        /// <summary>右空間キーボード操作入力</summary>
        private Vector2 _manualRMove;
        /// <summary>右空間キーボード操作入力</summary>
        public Vector2 ManualRMove => _manualRMove;
        /// <summary>
        /// ManualRMoveのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnManualRMove(InputAction.CallbackContext context)
        {
            _manualRMove = context.ReadValue<Vector2>();
        }

        /// <summary>左空間コントローラー操作入力</summary>
        private Vector2 _autoLMove;
        /// <summary>左空間コントローラー操作入力</summary>
        public Vector2 AutoLMove => _autoLMove;
        /// <summary>
        /// AutoLMoveのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnAutoLMove(InputAction.CallbackContext context)
        {
            _autoLMove = context.ReadValue<Vector2>();
        }

        /// <summary>右空間コントローラー操作入力</summary>
        private Vector2 _autoRMove;
        /// <summary>右空間コントローラー操作入力</summary>
        public Vector2 AutoRMove => _autoRMove;
        /// <summary>
        /// AutoRMoveのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnAutoRMove(InputAction.CallbackContext context)
        {
            _autoRMove = context.ReadValue<Vector2>();
        }

        public void DisableAll()
        {
            _manualLAxcel = false;
            _manualRAxcel = false;
            _manualLMove = new Vector2();
            _manualRMove = new Vector2();
            _autoLMove = new Vector2();
            _autoRMove = new Vector2();
        }
    }
}
