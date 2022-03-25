using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;
using UniRx;
using UniRx.Triggers;
using Common.LevelDesign;

/// <summary>
/// ゴールエリア
/// </summary>
public class GoalPoint : MonoBehaviour
{
    /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
    private static readonly Vector3 ISGROUNDED_RAY_ORIGIN_OFFSET = new Vector3(0f, 0.1f);
    /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
    private static readonly Vector3 ISGROUNDED_RAY_DIRECTION = Vector3.down;
    /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
    private static readonly float ISGROUNDED_RAY_MAX_DISTANCE = 1.5f;

    private void Start()
    {
        // プレイヤーオブジェクトがゴールに触れる
        this.OnTriggerEnterAsObservable()
            .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
            .Select(_ => PlayClearDirectionAndOpenClearScreen())
            .Where(x => !x)
            .Subscribe(_ => Debug.Log("ゴール演出エラー発生"));
    }

    /// <summary>
    /// クリア演出の再生、クリア画面の表示
    /// </summary>
    /// <returns>成功／失敗</returns>
    private bool PlayClearDirectionAndOpenClearScreen()
    {
        if (LevelDesisionIsObjected.IsGrounded(transform.position, ISGROUNDED_RAY_ORIGIN_OFFSET, ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE))
        {
            // T.B.D プレイヤー操作を停止する処理を追加
            // T.B.D ゴール演出を入れるなら追加
            SfxPlay.Instance.PlaySFX(ClipToPlay.me_game_clear);
            UIManager.Instance.OpenClearScreen();

            return true;
        }
        else
        {
            Debug.Log("ゴール下に足場がありません");
            return false;
        }
    }
}
