using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common.Const
{
    /// <summary>
    /// TagManagerのタグ名を一覧化（レイヤー）
    /// ・ファイルパス
    ///     $HOME/ProjectSettings/TagManager.asset
    /// </summary>
    public class LayerConst
    {
        /// <summary>
        /// プレイヤーのレイヤー名
        /// </summary>
        public static readonly string LAYER_NAME_PLAYER = "Player";
        /// <summary>
        /// 静的オブジェクトのレイヤー名
        /// </summary>
        public static readonly string LAYER_NAME_FREEZE = "Freeze";
        /// <summary>
        /// MoveCubeのレイヤー名
        /// </summary>
        public static readonly string LAYER_NAME_MOVECUBE = "MoveCube";
        /// <summary>
        /// 敵ギミックのレイヤー名
        /// </summary>
        public static readonly string LAYER_NAME_ROBOTENEMIES = "Robot_Ememy";
        /// <summary>
        /// ワープのレイヤー名
        /// </summary>
        public static readonly string LAYER_NAME_WARPGATE = "WarpGate";
    }
}
