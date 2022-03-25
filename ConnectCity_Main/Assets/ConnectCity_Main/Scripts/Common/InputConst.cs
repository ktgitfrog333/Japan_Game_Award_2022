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
        /// キーボードのRightKey,LeftKey,D,A/ゲームパッドの十字キー入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL = "Horizontal";
        /// <summary>
        /// ※未使用？
        /// </summary>
        public static readonly string INPUT_CONST_VERTICAL = "Vertical";
        /// <summary>
        /// キーボードのQ,E/ゲームパッドの左スティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_LS = "HorizontalLS";
        /// <summary>
        /// キーボードの2,W/ゲームパッドの左スティック入力・上下
        /// </summary>
        public static readonly string INPUT_CONST_VERTICAL_LS = "VerticalLS";
        /// <summary>
        /// キーボードのI,P/ゲームパッドの右スティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_RS = "HorizontalRS";
        /// <summary>
        /// キーボードの9,O/ゲームパッドの右スティック入力・上下
        /// </summary>
        public static readonly string INPUT_CONST_VERTICAL_RS = "VerticalRS";
        /// <summary>
        /// キーボードのEsc/ゲームパッドのStart
        /// </summary>
        public static readonly string INPUT_CONST_CANCEL = "Cancel";
        /// <summary>
        /// キーボードのSpace/ゲームパッドのAボタン
        /// </summary>
        public static readonly string INPUT_CONSTJUMP = "Jump";
        /// <summary>
        /// キーボードのR/ゲームパッドのRBボタン
        /// </summary>
        public static readonly string INPUT_CONSTSPACE = "Space";
    }
}
