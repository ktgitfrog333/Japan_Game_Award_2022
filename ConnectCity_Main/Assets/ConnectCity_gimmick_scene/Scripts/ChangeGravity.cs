using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;
using Main.Common;
using Main.Common.LevelDesign;

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

            localGravity = LevelDesisionIsObjected.GetVectorFromDirection(direction) * moveSpeed;

            this.OnTriggerStayAsObservable()
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
        private bool MovePlayerFromChangeGravity(Vector3 localGravity)
        {
            return GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MoveCharactorFromGravityController(localGravity);
        }
    }
}
