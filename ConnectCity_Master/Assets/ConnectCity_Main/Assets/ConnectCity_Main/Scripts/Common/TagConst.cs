using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common.Const
{
    /// <summary>
    /// TagManagerのタグ名を一覧化
    /// ・ファイルパス
    ///     $HOME/ProjectSettings/TagManager.asset
    /// </summary>
    public class TagConst
    {
        /// <summary>
        /// プレイヤーのタグ名
        /// </summary>
        public static readonly string TAG_NAME_PLAYER = "Player";
        /// <summary>
        /// 動くキューブのタグ名
        /// </summary>
        public static readonly string TAG_NAME_MOVECUBE = "MoveCube";
        /// <summary>
        /// 動くキューブグループのタグ名
        /// </summary>
        public static readonly string TAG_NAME_MOVECUBEGROUP = "MoveCubeGroup";
        /// <summary>
        /// レベルデザインのタグ名
        /// </summary>
        public static readonly string TAG_NAME_LEVELDESIGN = "LevelDesign";
    }
}
