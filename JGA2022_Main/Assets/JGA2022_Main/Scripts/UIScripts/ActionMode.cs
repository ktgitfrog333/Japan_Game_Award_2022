using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクションモード
/// </summary>
public class ActionMode : MonoBehaviour
{
    /// <summary>アクションモード</summary>
    [SerializeField] public PauseActionMode mode;
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
    /// <summary>もう一度遊ぶ</summary>
    RedoAction = 2,
    /// <summary>他のステージを選ぶ</summary>
    SelectAction = 3,
    /// <summary>遊び方の確認</summary>
    CheckAction = 4
}
