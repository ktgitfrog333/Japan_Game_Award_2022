using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 空間制御する
/// </summary>
public class SpaceManager : MonoBehaviour
{
    /// <summary>空間操作に必要なRigidBody、Velocity</summary>
    private SpaceDirection2D _spaceDirections = new SpaceDirection2D();
    /// <summary>移動速度</summary>
    [SerializeField] private float moveSpeed = 3.5f;

    /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
    private static readonly Vector3 ISGROUNDED_RAY_ORIGIN_OFFSET = new Vector3(0f, -0.1f);
    /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
    private static readonly Vector3 ISGROUNDED_RAY_DIRECTION = Vector3.up;
    /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
    private static readonly float ISGROUNDED_RAY_MAX_DISTANCE = 0.9f;

    private void Start()
    {
        // 動かすブロックのリストを取得
        var objs = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBE);
        var objsGroup = GameObject.FindGameObjectWithTag(TagConst.TAG_NAME_MOVECUBEGROUP);
        if (!SetCollsion(objs)) Debug.Log("オブジェクト取得の失敗");
        // 移動入力、空間内のブロック座標をチェック
        this.UpdateAsObservable()
            .Where(_ => SetMoveVelocotyLeftAndRight())
            .Select(_ => !CheckPositionAndSetMoveCubesRigidbodies(objs))
            .Where(x => x)
            .Subscribe(_ => Debug.Log("制御対象RigidBody格納の失敗"));
        // 左空間の制御
        this.FixedUpdateAsObservable()
            .Where(_ => 0f < _spaceDirections.MoveVelocityLeftSpace.magnitude && _spaceDirections.RbsLeftSpace != null && 0 < _spaceDirections.RbsLeftSpace.Length)
            .Subscribe(_ =>
            {
                foreach (var rb in _spaceDirections.RbsLeftSpace)
                    rb.AddForce(_spaceDirections.MoveVelocityLeftSpace + _spaceDirections.MoveVelocityLeftSpace * Time.deltaTime);
            });
        // 右空間の制御
        this.FixedUpdateAsObservable()
            .Where(_ => 0f < _spaceDirections.MoveVelocityRightSpace.magnitude && _spaceDirections.RbsRightSpace != null && 0 < _spaceDirections.RbsRightSpace.Length)
            .Subscribe(_ =>
            {
                foreach (var rb in _spaceDirections.RbsRightSpace)
                    rb.AddForce(_spaceDirections.MoveVelocityRightSpace + _spaceDirections.MoveVelocityRightSpace * Time.deltaTime);
            });
        this.OnCollisionEnterAsObservable()
            .Select(x => CheckGroupAndAddGroup(x.transform, objsGroup))
            .Where(x => !x)
            .Subscribe(_ => Debug.Log("既に登録済みか、レベルデザイン内のオブジェクトではありません。"));
    }

    private bool CheckGroupAndAddGroup(Transform transform, GameObject gameObjectGroup)
    {
        if (transform.parent == null)
            return false;
        if (transform.parent != null && transform.parent.tag.Equals(TagConst.TAG_NAME_MOVECUBEGROUP))
            return false;
        transform.parent = gameObjectGroup.transform;
        return true;
    }

    /// <summary>
    /// 衝突判定をセット
    /// ブロックオブジェクトが見つからない場合は失敗を返却
    /// </summary>
    /// <param name="gameObjects">空間操作ブロックオブジェクト</param>
    /// <returns>成功／失敗</returns>
    private bool SetCollsion(GameObject[] gameObjects)
    {
        if (gameObjects == null || gameObjects.Length < -1) return false;

        foreach (var obj in gameObjects)
        {
            obj.UpdateAsObservable()
                .Where(_ => LevelDecision.IsOnPlayeredAndInfo(obj.transform.position, ISGROUNDED_RAY_ORIGIN_OFFSET, ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
                .Select(_ => GameManager.Instance.MoveCharactorFromSpaceManager(obj.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                .Subscribe(x => { if (!x) Debug.Log("プレイヤー操作指令の失敗"); });
        }

        return true;
    }

    /// <summary>
    /// 操作入力を元に制御情報を更新
    /// </summary>
    /// <returns>処理結果の成功／失敗</returns>
    private bool SetMoveVelocotyLeftAndRight()
    {
        var hztlL = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_LS);
        var vtclL = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_LS);
        var hztlR = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_RS);
        var vtclR = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_RS);

        if (0f < Mathf.Abs(hztlL) || 0f < Mathf.Abs(vtclL) || 0f < Mathf.Abs(hztlR) || 0f < Mathf.Abs(vtclR))
        {
            _spaceDirections.MoveVelocityLeftSpace = new Vector3(hztlL, vtclL) * moveSpeed;
            _spaceDirections.MoveVelocityRightSpace = new Vector3(hztlR, vtclR) * moveSpeed;

            return (0f < _spaceDirections.MoveVelocityLeftSpace.magnitude)
                || (0f < _spaceDirections.MoveVelocityRightSpace.magnitude) ? true : false;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 動かすブロックの位置が左空間・右空間かを調べて、各空間操作用のリストへ格納
    /// </summary>
    /// <returns>処理結果の成功／失敗</returns>
    private bool CheckPositionAndSetMoveCubesRigidbodies(GameObject[] _moveCbs)
    {
        if (0 < _moveCbs.Length)
        {
            var rbsLeft = new List<Rigidbody>();
            var rbsRight = new List<Rigidbody>();
            foreach (var obj in _moveCbs)
            {
                if (obj.transform.position.x < 0f)
                {
                    // 左空間
                    rbsLeft.Add(obj.GetComponent<Rigidbody>());
                }
                else if (0f < obj.transform.position.x)
                {
                    // 右空間
                    rbsRight.Add(obj.GetComponent<Rigidbody>());
                }
                else
                {
                    return false;
                }
            }
            // 動かす対象のRigidBodyを格納
            if (0 < rbsLeft.Count)
                _spaceDirections.RbsLeftSpace = rbsLeft.ToArray();
            else
                _spaceDirections.RbsLeftSpace = null;
            if (0 < rbsRight.Count)
                _spaceDirections.RbsRightSpace = rbsRight.ToArray();
            else
                _spaceDirections.RbsRightSpace = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}

/// <summary>
/// 空間操作に必要なRigidBody、Velocity
/// </summary>
public struct SpaceDirection2D
{
    /// <summary>左側のRigidBody</summary>
    public Rigidbody[] RbsLeftSpace { get; set; }
    /// <summary>右側のRigidBody</summary>
    public Rigidbody[] RbsRightSpace { get; set; }
    /// <summary>左側のVelocity</summary>
    public Vector3 MoveVelocityLeftSpace { get; set; }
    /// <summary>右側のVelocity</summary>
    public Vector3 MoveVelocityRightSpace { get; set; }
}
