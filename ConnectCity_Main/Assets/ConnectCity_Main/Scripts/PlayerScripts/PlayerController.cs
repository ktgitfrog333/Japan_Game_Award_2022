using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;
using UniRx;
using UniRx.Triggers;

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

    /// <summary>キャラクター制御</summary>
    private CharacterController _characterCtrl;

    /// <summary>移動速度</summary>
    [SerializeField] private float moveSpeed = 4f;
    /// <summary>ジャンプ速度</summary>
    [SerializeField] private float jumpSpeed = 5f;
    /// <summary>ジャンプ状態</summary>
    private BoolReactiveProperty _isJumped = new BoolReactiveProperty();

    void Start()
    {
        _characterCtrl = GetComponent<CharacterController>();
        // 位置・スケールのキャッシュ
        var transform = base.transform;
        // 移動先の座標（X軸の移動、Y軸のジャンプのみ）
        var moveVelocity = new Vector3();

        // 移動入力に応じて移動座標をセット
        this.UpdateAsObservable()
            .Select(_ => Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL) * moveSpeed)
            .Subscribe(x =>
            {
                moveVelocity.x = x;
                transform.LookAt(transform.position + new Vector3(moveVelocity.x, 0f, 0f));
            });
        // ジャンプ入力に応じてジャンプフラグをセット
        this.UpdateAsObservable()
            .Where(_ => !_isJumped.Value &&
                LevelDecision.IsGrounded(transform.position, ISGROUNDED_RAY_ORIGIN_OFFSET, ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE))
            .Select(_ => Input.GetButtonDown(InputConst.INPUT_CONSTJUMP))
            .Where(x => x)
            .Subscribe(x => _isJumped.Value = x);
        // ジャンプフラグ切り替え
        _isJumped.Where(x => x)
            .Subscribe(_ =>
            {
                moveVelocity.y = jumpSpeed;
                // T.B.D ジャンプSEがはいるが、ひとまず決定音を鳴らす
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                _isJumped.Value = false;
            });
        // 空中にいる際の移動座標をセット
        this.UpdateAsObservable()
            .Where(_ => !LevelDecision.IsGrounded(transform.position, ISGROUNDED_RAY_ORIGIN_OFFSET, ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE))
            .Subscribe(_ => moveVelocity.y += Physics.gravity.y * Time.deltaTime);
        // 移動
        this.FixedUpdateAsObservable()
            .Subscribe(_ => _characterCtrl.Move(moveVelocity * Time.deltaTime));
    }
}
/// <summary>
/// レベル共通判定
/// </summary>
public class LevelDecision
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
