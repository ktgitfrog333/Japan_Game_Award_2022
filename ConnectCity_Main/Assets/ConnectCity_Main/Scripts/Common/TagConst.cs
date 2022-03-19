using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Const
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
        /// 動くブロックのタグ名
        /// </summary>
        public static readonly string TAG_NAME_MOVECUBE = "MoveCube";

        /// <summary>
        /// 動くブロックグループのタグ名
        /// </summary>
        public static readonly string TAG_NAME_MOVECUBEGROUP = "MoveCubeGroup";
    }
}
