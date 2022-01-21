using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Const
{
    /// <summary>
    /// InputManagerのAxis名を一覧化
    /// ・メニューから開く
    ///     Edit > ProjectSettings... > Input Manager
    /// ・ファイルパス
    ///     $HOME/ProjectSettings/InputManager.asset
    /// </summary>
    public static class InputConst
    {
        /// <summary>
        /// キーボードのEsc/ゲームパッドのStart
        /// </summary>
        public static readonly string INPUT_CONST_MENU = "Menu";
        /// <summary>
        /// キーボードのRightKey,LeftKey,D,A/ゲームパッドのスティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL = "Horizontal";
        /// <summary>
        /// キーボードのEsc/ゲームパッドのStart
        /// </summary>
        public static readonly string INPUT_CONST_CANCEL = "Cancel";
    }
}
