using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;

/// <summary>
/// ゲームオブジェクト間の関数実行の指令はこのオブジェクトを仲介する
/// T.B.D SFXやPauseScreenなどシングルトンにしてしまっている物はここに集約させる？
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>クラス自身</summary>
    private static GameManager instance;
    /// <summary>シングルトンのインスタンス</summary>
    public static GameManager Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        // シングルトンのため複数生成禁止
        if (null != instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    /// <summary>
    /// プレイヤーのゲームオブジェクト
    /// </summary>
    [SerializeField] private GameObject player;

    private void Reset()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag(TagConst.TAG_NAME_PLAYER);
    }

    /// <summary>
    /// 空間操作オブジェクトからプレイヤーオブジェクトへ
    /// プレイヤー操作指令を実行して、実行結果を返却する
    /// </summary>
    /// <param name="moveVelocity">移動座標</param>
    /// <returns>成功／失敗</returns>
    public bool MoveCharactorFromSpaceManager(Vector3 moveVelocity)
    {
        try
        {
            return player.GetComponent<PlayerController>().MoveChatactorFromGameManager(moveVelocity) ? true : false;
        }
        catch
        {
            return false;
        }
    }
}
