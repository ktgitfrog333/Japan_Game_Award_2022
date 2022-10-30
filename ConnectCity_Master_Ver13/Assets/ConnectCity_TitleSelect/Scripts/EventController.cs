using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.InputSystem;

namespace TitleSelect
{
    /// <summary>
    /// イベントコントローラー
    /// </summary>
    public class EventController : MonoBehaviour
    {
        /// <summary>
        /// 実行イベントの監視
        /// 0 : Selected
        /// 1 : DeSelected
        /// 2 : Submited
        /// 3 : Canceled
        /// 4 : AnyKeysPushed
        /// </summary>
        public IntReactiveProperty EventRP { get; set; } = new IntReactiveProperty(-1);

        /// <summary>
        /// 選択された時に発火するイベント
        /// </summary>
        public void Selected()
        {
            EventRP.Value = (int)EventCommand.Selected;
        }

        /// <summary>
        /// 選択されなかった時に発火するイベント
        /// </summary>
        public void DeSelected()
        {
            EventRP.Value = (int)EventCommand.DeSelected;
        }

        /// <summary>
        /// 確定時に発火するイベント
        /// </summary>
        public void Submited()
        {
            EventRP.Value = (int)EventCommand.Submited;
        }

        /// <summary>
        /// キャンセル時に発火するイベント
        /// </summary>
        public void Canceled()
        {
            EventRP.Value = (int)EventCommand.Canceled;
        }

        /// <summary>
        /// いずれかのキー入力時に発火するイベント
        /// </summary>
        public void AnyKeys()
        {
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                        (Gamepad.current != null && (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                            Gamepad.current.buttonNorth.wasPressedThisFrame ||
                            Gamepad.current.buttonEast.wasPressedThisFrame ||
                            Gamepad.current.buttonWest.wasPressedThisFrame ||
                            Gamepad.current.leftShoulder.wasPressedThisFrame ||
                            Gamepad.current.rightShoulder.wasPressedThisFrame ||
                            Gamepad.current.leftTrigger.wasPressedThisFrame ||
                            Gamepad.current.rightTrigger.wasPressedThisFrame ||
                            Gamepad.current.startButton.wasPressedThisFrame ||
                            Gamepad.current.selectButton.wasPressedThisFrame)))
                    {
                        EventRP.Value = (int)EventCommand.AnyKeysPushed;
                    }
                });
        }
    }

    /// <summary>
    /// コマンドの種類
    /// </summary>
    public enum EventCommand
    {
        /// <summary>選択された</summary>
        Selected,
        /// <summary>選択解除された</summary>
        DeSelected,
        /// <summary>実行された</summary>
        Submited,
        /// <summary>キャンセルされた</summary>
        Canceled,
        /// <summary>いずれかのキーの入力</summary>
        AnyKeysPushed,
    }
}
