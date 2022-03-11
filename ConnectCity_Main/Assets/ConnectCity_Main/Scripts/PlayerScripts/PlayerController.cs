using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Common.Const;

/// <summary>
/// プレイヤー操作制御
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
    private static readonly Vector3 ISGROUNDED_RAY_ORIGIN_OFFSET = new Vector3(0f, 0.1f);
    /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
    private static readonly Vector3 ISGROUNDED_RAY_DIRECTION = Vector3.down;
    /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
    private static readonly float ISGROUNDED_RAY_MAX_DISTANCE = 0.8f;

    /// <summary>位置・スケール</summary>
    private Transform _transform;
    /// <summary>キャラクター制御</summary>
    private CharacterController _characterCtrl;
    /// <summary>移動させる位置・スケール</summary>
    private Vector3 _moveVelocity;

    /// <summary>移動速度</summary>
    [SerializeField] private float moveSpeed = 4f;
    /// <summary>ジャンプ速度</summary>
    [SerializeField] private float jumpSpeed = 5f;
    /// <summary>ジャンプ状態</summary>
    private bool jumped;

    void Start()
    {
        _transform = transform;
        _characterCtrl = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _moveVelocity.x = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL) * moveSpeed;
        //_moveVelocity.z = Input.GetAxis(VERTICAL) * speed;
        _transform.LookAt(_transform.position + new Vector3(_moveVelocity.x, 0f, _moveVelocity.z));
        if (!jumped && Input.GetButtonDown(InputConst.INPUT_CONSTJUMP) && LevelDecision.IsGrounded(_transform.position, ISGROUNDED_RAY_ORIGIN_OFFSET, ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE))
            jumped = true;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    /// <summary>
    /// キャラクターを動かす
    /// </summary>
    private void MoveCharacter()
    {
        if (LevelDecision.IsGrounded(_transform.position, ISGROUNDED_RAY_ORIGIN_OFFSET, ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE))
        {
            // ジャンプ
            if (jumped)
            {
                _moveVelocity.y = jumpSpeed;
                // T.B.D ジャンプSEがはいるが、ひとまず決定音を鳴らす
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                jumped = false;
            }
        }
        else
        {
            _moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        _characterCtrl.Move(_moveVelocity * Time.deltaTime);
    }
}

/// <summary>
/// レベル共通判定
/// </summary>
public static class LevelDecision
{
    /// <summary>
    /// 接地判定
    /// </summary>
    /// <param name="postion">位置・スケール</param>
    /// <param name="rayOriginOffset">始点</param>
    /// <param name="rayDirection">終点</param>
    /// <param name="rayMaxDistance">最大距離</param>
    /// <returns>レイのヒット判定の有無</returns>
    public static bool IsGrounded(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance)
    {
        var ray = new Ray(postion + rayOriginOffset, rayDirection);
        //Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
        var raycastHits = new RaycastHit[1];
        var hitCount = Physics.RaycastNonAlloc(ray, raycastHits, rayMaxDistance);
        return hitCount >= 1f;
    }
}
