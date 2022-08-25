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
        /// <summary>
        /// SpaceManagerのタグ名
        /// </summary>
        public static readonly string TAG_NAME_SPACEMANAGER = "SpaceManager";
        /// <summary>
        /// チュートリアル表示トリガーのタグ名
        /// </summary>
        public static readonly string TAG_NAME_TUTORIALTRIGGER = "TutorialTrigger";
        /// <summary>
        /// ゴールポイントのタグ名
        /// </summary>
        public static readonly string TAG_NAME_GOALPOINT = "GoalPoint";
        /// <summary>
        /// コネクトカウントダウンUIのタグ名
        /// </summary>
        public static readonly string TAG_NAME_CONNECTCOUNTSCREEN = "ConnectCountScreen";
        /// <summary>
        /// 敵ロボットのタグ名
        /// </summary>
        public static readonly string TAG_NAME_ROBOT_EMEMY = "Robot_Ememy";
        /// <summary>
        /// ぼろいブロック・天井のタグ名
        /// </summary>
        public static readonly string TAG_NAME_BREAKBLOOK = "BreakBlook";
        /// <summary>
        /// レーザー砲のタグ名
        /// </summary>
        public static readonly string TAG_NAME_TURRETENEMIES = "TurretEnemies";
        /// <summary>
        /// 動くキューブ（ゴースト）のタグ名
        /// </summary>
        public static readonly string TAG_NAME_MOVECUBEGHOST = "MoveCubeGhost";
        /// <summary>
        /// 重力ギミックのタグ名
        /// </summary>
        public static readonly string TAG_NAME_CHANGEGRAVITY = "ChangeGravity";
        /// <summary>
        /// 自動追尾ドローンギミックのタグ名
        /// </summary>
        public static readonly string TAG_NAME_AUTODRONE = "AutoDrone";
        /// <summary>
        /// 静的ブロックのタグ名
        /// </summary>
        public static readonly string TAG_NAME_FREEZECUBE = "FreezeCube";
    }
}
