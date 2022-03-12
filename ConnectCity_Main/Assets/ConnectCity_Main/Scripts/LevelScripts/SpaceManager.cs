using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;

/// <summary>
/// 空間制御する
/// </summary>
public class SpaceManager : MonoBehaviour
{
    /// <summary>動かすブロックのオブジェクト配列</summary>
    private GameObject[] _moveCbs;
    /// <summary>空間操作に必要なRigidBody、Velocity</summary>
    private SpaceDirection2D _spaceDirections = new SpaceDirection2D();
    /// <summary>移動速度</summary>
    [SerializeField] private float moveSpeed = 3.5f;

    private void Start()
    {
        if (!FindMoveCubes()) Debug.Log("動かすブロックの取得失敗");
    }

    private void Update()
    {
        if (SetMoveVelocotyLeftAndRight())
        {
            if (!CheckPositionAndSetMoveCubesRigidbodies()) Debug.Log("制御対象RigidBody格納の失敗");
        }
        //else
        //    Debug.Log("操作入力無しまたは制御更新の失敗");
    }

    private void FixedUpdate()
    {
        if (0f < _spaceDirections.MoveVelocityLeftSpace.magnitude && _spaceDirections.RbsLeftSpace != null && 0 < _spaceDirections.RbsLeftSpace.Length)
        {
            foreach (var rb in _spaceDirections.RbsLeftSpace)
            {
                rb.AddForce(_spaceDirections.MoveVelocityLeftSpace + _spaceDirections.MoveVelocityLeftSpace * Time.deltaTime);
            }
        }
        if (0f < _spaceDirections.MoveVelocityRightSpace.magnitude && _spaceDirections.RbsRightSpace != null && 0 < _spaceDirections.RbsRightSpace.Length)
        {
            foreach (var rb in _spaceDirections.RbsRightSpace)
            {
                rb.AddForce(_spaceDirections.MoveVelocityRightSpace + _spaceDirections.MoveVelocityRightSpace * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// 動かすブロックをリストで保持
    /// </summary>
    /// <returns>処理結果の成功／失敗</returns>
    private bool FindMoveCubes()
    {
        var objs = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBE);
        if (0 < objs.Length)
        {
            _moveCbs = objs;
            return true;
        }
        else
            return false;
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
    private bool CheckPositionAndSetMoveCubesRigidbodies()
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
