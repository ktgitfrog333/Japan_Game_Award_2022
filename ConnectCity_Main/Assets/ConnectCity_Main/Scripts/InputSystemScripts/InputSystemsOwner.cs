using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UnityEngine.InputSystem;
using System;

namespace Main.InputSystem
{
    /// <summary>
    /// InputSystemのオーナー
    /// </summary>
    public class InputSystemsOwner : MonoBehaviour, IGameManager
    {
        /// <summary>プレイヤー用のインプットイベント</summary>
        [SerializeField] private InputPlayer inputPlayer;
        /// <summary>プレイヤー用のインプットイベント</summary>
        public InputPlayer InputPlayer => inputPlayer;
        /// <summary>UI用のインプットイベント</summary>
        [SerializeField] private InputUI inputUI;
        /// <summary>UI用のインプットイベント</summary>
        public InputUI InputUI => inputUI;
        /// <summary>Space用のインプットイベント</summary>
        [SerializeField] private InputSpace inputSpace;
        /// <summary>Space用のインプットイベント</summary>
        public InputSpace InputSpace => inputSpace;
        /// <summary>インプットアクション</summary>
        private JGA2022_Main _inputActions;
        /// <summary>インプットアクション</summary>
        public JGA2022_Main InputActions => _inputActions;

        private void Reset()
        {
            inputPlayer = GetComponent<InputPlayer>();
            inputUI = GetComponent<InputUI>();
            inputSpace = GetComponent<InputSpace>();
        }

        public bool Initialize()
        {
            try
            {
                _inputActions = new JGA2022_Main();
                _inputActions.Player.MoveLeft.started += inputPlayer.OnMovedLeft;
                _inputActions.Player.MoveLeft.performed += inputPlayer.OnMovedLeft;
                _inputActions.Player.MoveLeft.canceled += inputPlayer.OnMovedLeft;
                _inputActions.Player.MoveRight.started += inputPlayer.OnMovedRight;
                _inputActions.Player.MoveRight.performed += inputPlayer.OnMovedRight;
                _inputActions.Player.MoveRight.canceled += inputPlayer.OnMovedRight;
                _inputActions.Player.Jump.started += inputPlayer.OnJumped;
                _inputActions.Player.Jump.performed += inputPlayer.OnJumped;
                _inputActions.Player.Jump.canceled += inputPlayer.OnJumped;
                _inputActions.UI.Pause.started += inputUI.OnPaused;
                _inputActions.UI.Pause.performed += inputUI.OnPaused;
                _inputActions.UI.Pause.canceled += inputUI.OnPaused;
                _inputActions.UI.Space.started += inputUI.OnSpaced;
                _inputActions.UI.Space.performed += inputUI.OnSpaced;
                _inputActions.UI.Space.canceled += inputUI.OnSpaced;
                _inputActions.UI.Undo.started += inputUI.OnUndoed;
                _inputActions.UI.Undo.canceled += inputUI.OnUndoed;
                _inputActions.UI.Select.started += inputUI.OnSelected;
                _inputActions.UI.Select.canceled += inputUI.OnSelected;
                _inputActions.UI.Manual.started += inputUI.OnManualed;
                _inputActions.UI.Manual.canceled += inputUI.OnManualed;
                _inputActions.Space.ManualLAxcel.performed += inputSpace.OnManualLAxcel;
                _inputActions.Space.ManualRAxcel.performed += inputSpace.OnManualRAxcel;
                _inputActions.Space.ManualLMove.started += inputSpace.OnManualLMove;
                _inputActions.Space.ManualLMove.performed += inputSpace.OnManualLMove;
                _inputActions.Space.ManualLMove.canceled += inputSpace.OnManualLMove;
                _inputActions.Space.ManualRMove.started += inputSpace.OnManualRMove;
                _inputActions.Space.ManualRMove.performed += inputSpace.OnManualRMove;
                _inputActions.Space.ManualRMove.canceled += inputSpace.OnManualRMove;
                _inputActions.Space.AutoLMove.started += inputSpace.OnAutoLMove;
                _inputActions.Space.AutoLMove.performed += inputSpace.OnAutoLMove;
                _inputActions.Space.AutoLMove.canceled += inputSpace.OnAutoLMove;
                _inputActions.Space.AutoRMove.started += inputSpace.OnAutoRMove;
                _inputActions.Space.AutoRMove.performed += inputSpace.OnAutoRMove;
                _inputActions.Space.AutoRMove.canceled += inputSpace.OnAutoRMove;

                _inputActions.Enable();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Exit()
        {
            try
            {
                _inputActions.Disable();
                inputPlayer.DisableAll();
                inputUI.DisableAll();
                inputSpace.DisableAll();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 各インプットのインターフェース
    /// </summary>
    public interface IInputSystemsOwner
    {
        /// <summary>
        /// 全ての入力をリセット
        /// </summary>
        public void DisableAll();
    }
}
