using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.UI
{
    /// <summary>
    /// アクションモード
    /// </summary>
    public class ActionMode : MonoBehaviour
    {
        /// <summary>ポーズのアクションモード</summary>
        [SerializeField] public PauseActionMode pauseMode;
        /// <summary>クリアのアクションモード</summary>
        [SerializeField] public ClearActionMode clearMode;
    }

    /// <summary>
    /// ポーズアクション
    /// HierarchyにあるPauseScreen > BackScreen内のゲームオブジェクトを配列した場合の順番と揃える
    ///     [0]GamePause
    ///     [1]GameBackButton   ★
    ///     [2]GameRedoButton   ★
    ///     [3]GameSelectButton ★
    ///     [4]GameCheckButton  ★
    /// </summary>
    public enum PauseActionMode
    {
        /// <summary>ゲームに戻る</summary>
        BackAction = 1,
        /// <summary>ステージをやり直す</summary>
        RedoAction = 2,
        /// <summary>他のステージを選ぶ</summary>
        SelectAction = 3,
        /// <summary>遊び方の確認</summary>
        CheckAction = 4
    }

    /// <summary>
    /// クリアアクション
    /// HierarchyにあるClearScreen > BackScreen内のゲームオブジェクトを配列した場合の順番と揃える
    ///     [0]StageClear
    ///     [1]GameRetryButton      ★
    ///     [2]GameSelectButton     ★
    ///     [3]GameProceedButton    ★
    /// </summary>
    public enum ClearActionMode
    {
        /// <summary>もう一度遊ぶ</summary>
        RetryAction = 1,
        /// <summary>他のステージを選ぶ</summary>
        SelectAction = 2,
        /// <summary>次のステージをを選ぶ</summary>
        ProceedAction = 3
    }
}
