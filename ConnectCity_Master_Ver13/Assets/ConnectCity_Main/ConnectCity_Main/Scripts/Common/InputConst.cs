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
        /// キーボードのr/ゲームパッドは未使用
        /// </summary>
        public static readonly string INPUT_CONST_LS_COM = "LSCom";
        /// <summary>
        /// キーボードのu/ゲームパッドは未使用
        /// </summary>
        public static readonly string INPUT_CONST_RS_COM = "RSCom";
        /// <summary>
        /// キーボードのq,e/ゲームパッドは未使用
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_LS_KEYBOD = "HorizontalLSKeyBod";
        /// <summary>
        /// キーボードの2,w/ゲームパッドは未使用
        /// </summary>
        public static readonly string INPUT_CONST_VERTICAL_LS_KEYBOD = "VerticalLSKeyBod";
        /// <summary>
        /// キーボードのi,p/ゲームパッドは未使用
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_RS_KEYBOD = "HorizontalRSKeyBod";
        /// <summary>
        /// キーボードの9,o/ゲームパッドは未使用
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
        /// キーボードは未使用/ゲームパッドの右スティック入力・左右
        /// </summary>
        public static readonly string INPUT_CONST_HORIZONTAL_RS = "HorizontalRS";
        /// <summary>
        /// キーボードは未使用/ゲームパッドの右スティック入力・上下
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
        /// <summary>
        /// キーボードのBackSpace/ゲームパッドのBackボタン
        /// </summary>
        public static readonly string INPUT_CONSTUNDO = "Undo";
        /// <summary>
        /// キーボードのH/ゲームパッドのYボタン
        /// </summary>
        public static readonly string INPUT_CONSTSELECT = "Select";
        /// <summary>
        /// キーボードのG/ゲームパッドのXボタン
        /// </summary>
        public static readonly string INPUT_CONSTMANUAL = "Manual";
    }
}
