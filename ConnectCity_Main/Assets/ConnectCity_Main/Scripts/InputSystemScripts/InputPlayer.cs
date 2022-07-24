using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.InputSystem
{
    /// <summary>
    /// プレイヤー用のInputAction
    /// </summary>
    [RequireComponent(typeof(InputAction))]
    public class InputPlayer : MonoBehaviour
    {
        /// <summary>移動量</summary>
        private Vector2 _moved;
        /// <summary>移動量</summary>
        public Vector2 Moved => _moved;
        /// <summary>
        /// Moveのアクションに応じて移動量を更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnMoved(InputAction.CallbackContext context)
        {
            _moved = context.ReadValue<Vector2>();
        }

        /// <summary>ジャンプ入力</summary>
        private bool _jumped;
        /// <summary>ジャンプ入力</summary>
        public bool Jumped => _jumped;
        /// <summary>
        /// Jumpのアクションに応じてフラグを更新
        /// </summary>
        /// <param name="context">コールバック</param>
        public void OnJumped(InputAction.CallbackContext context)
        {
            _jumped = context.ReadValueAsButton();
        }
    }
}
