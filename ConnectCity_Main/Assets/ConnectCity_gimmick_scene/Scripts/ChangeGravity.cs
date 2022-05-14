using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;
using Main.Common;

namespace Gimmick
{
    /// <summary>
    /// 重力操作ギミック
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ChangeGravity : MonoBehaviour
    {
        /// <summary>移動速度</summary>
        [SerializeField] private float moveSpeed = .3f;
        /// <summary>重力の方向（設定値）</summary>
        [SerializeField] private Direction direction;

        private void Start()
        {
            var localGravity = new Vector3();

            switch (direction)
            {
                case Direction.UP:
                    // 重力を上向きにセット
                    localGravity = Vector3.up * moveSpeed;
                    break;
                case Direction.DOWN:
                    // 重力を下向きにセット
                    localGravity = Vector3.down * moveSpeed;
                    break;
                case Direction.LEFT:
                    // 重力を左向きにセット
                    localGravity = Vector3.left * moveSpeed;
                    break;
                case Direction.RIGHT:
                    // 重力を右向きにセット
                    localGravity = Vector3.right * moveSpeed;
                    break;
                default:
                    break;
            }

            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                .Subscribe(_ =>
                {
                    if (!MovePlayerFromChangeGravity(localGravity))
                        Debug.LogError("重力操作処理の失敗");
                });
            this.OnTriggerExitAsObservable()
                .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                .Subscribe(_ =>
                {
                    if (!MovePlayerFromChangeGravity(localGravity))
                        Debug.LogError("重力操作処理の失敗");
                });
        }

        /// <summary>
        /// 重力操作ギミックオブジェクトからプレイヤーオブジェクトへ
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MovePlayerFromChangeGravity(Vector3 localGravity)
        {
            return GameManager.Instance.MoveCharactorFromGravityController(localGravity);
        }
    }

    public enum Direction
    {
        /// <summary>上</summary>
        UP
    /// <summary>下</summary>
    , DOWN
    /// <summary>左</summary>
    , LEFT
    /// <summary>右</summary>
    , RIGHT
    }
}
