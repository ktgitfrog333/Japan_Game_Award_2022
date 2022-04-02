using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common.Const
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
        /// キーボードのf/ゲームパッドは未使用
        /// </summary>
        public static readonly string INPUT_CONST_LS_COM = "LSCom";
        /// <summary>
        /// キーボードのj/ゲームパッドは未使用
        /// </summary>
        public static readonly string INPUT_CONST_RS_COM = "RSCom";
        /// <summary>
        /// キーボードは未使用/ゲームパッドの左スティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_LS_KEYBOD = "HorizontalLSKeyBod";
        /// <summary>
        /// キーボードは未使用/ゲームパッドの左スティック入力・上下
        /// </summary>
        public static readonly string INPUT_CONST_VERTICAL_LS_KEYBOD = "VerticalLSKeyBod";
        /// <summary>
        /// キーボードのk,;/ゲームパッドの右スティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_RS_KEYBOD = "HorizontalRSKeyBod";
        /// <summary>
        /// キーボードのo,l/ゲームパッドの右スティック入力・上下
        /// </summary>
        public static readonly string INPUT_CONST_VERTICAL_RS_KEYBOD = "VerticalRSKeyBod";
        /// <summary>
        /// キーボードは未使用/ゲームパッドの左スティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_LS = "HorizontalLS";
        /// <summary>
        /// キーボードは未使用/ゲームパッドの左スティック入力・上下
        /// </summary>
        public static readonly string INPUT_CONST_VERTICAL_LS = "VerticalLS";
        /// <summary>
        /// キーボードのk,;/ゲームパッドの右スティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_RS = "HorizontalRS";
        /// <summary>
        /// キーボードのo,l/ゲームパッドの右スティック入力・上下
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
