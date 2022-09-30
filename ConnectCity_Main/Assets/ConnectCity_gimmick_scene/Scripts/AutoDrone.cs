using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UniRx;
using UniRx.Triggers;
using Main.Common.LevelDesign;
using Main.Common.Const;
using Main.Level;
using DG.Tweening;
using Main.UI;
using System.Linq;

namespace Gimmick
{
    /// <summary>
    /// 自動追尾ドローン
    /// SphereCollider：静的ブロック、プレイヤーへの接触判定
    /// Rigidbody：静的ブロックなどRigidbodyを持たないオブジェクトへトリガー判定を許可させるため
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class AutoDrone : MonoBehaviour, IOwner
    {
        /// <summary>移動速度</summary>
        [SerializeField] private float moveSpeed = 2f;
        /// <summary>境界を超えない境目</summary>
        [SerializeField] private float borderOffset = 1f;
        /// <summary>戻る距離のレベル</summary>
        [SerializeField] private float returnLevel = 1f;
        /// <summary>加速速度（戻る場合）</summary>
        [SerializeField] private float returnDashSpeed = 20f;
        /// <summary>休息時間</summary>
        [SerializeField] private float returnDelay = 1.5f;
        /// <summary>揺れアニメーションの時間</summary>
        [SerializeField] private float shakeDuration = .5f;
        /// <summary>揺れアニメーションの振れ幅強さ</summary>
        [SerializeField] private float shakeStrength = 30f;
        /// <summary>揺れアニメーションの振動長さ</summary>
        [SerializeField] private int shakeVibrato = 5;
        /// <summary>揺れアニメーションの振れ幅ランダム</summary>
        [SerializeField] private float shakeRandomness = 35f;
        /// <summary>衝突エフェクトのプレハブ</summary>
        [SerializeField] private GameObject droneCollisionEffectPrefab;
        /// <summary>コライダー対象とするオブジェクト</summary>
        [SerializeField] private string[] collierTagNames = { TagConst.TAG_NAME_FREEZECUBE };
        /// <summary>操作可能／不可フラグ</summary>
        private BoolReactiveProperty _moveEnable = new BoolReactiveProperty();
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        /// <summary>
        /// ドローンの操作制御
        /// </summary>
        /// <param name="active">操作可否フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool SetAutoDroneMoveEnable(bool active)
        {
            try
            {
                if (_moveEnable != null)
                    _moveEnable.Value = active;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool ManualStart()
        {
            try
            {
                _moveEnable = new BoolReactiveProperty();
                var currentTransform = transform;

                // 左空間／右空間のどちらに存在するか
                var spaceOwner = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner;
                var spaceOwnTransform = spaceOwner.transform;
                var spacePlace = new IntReactiveProperty();
                var spacePlaceDefault = new IntReactiveProperty();
                spacePlace.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                        if (-1 < x)
                            spacePlaceDefault.Value = x;
                        else
                        {
                            if (_moveEnable.Value)
                            {
                                _moveEnable.Value = false;
                                DOVirtual.DelayedCall(returnDelay, () => _moveEnable.Value = true);
                                currentTransform.DOShakeRotation(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);

                                if (((Direction2D)spacePlaceDefault.Value).Equals(Direction2D.Left))
                                    transform.localPosition += Vector3.left * returnLevel;
                                else
                                    transform.localPosition += Vector3.right * returnLevel;
                            }
                        }
                    })
                    .AddTo(_compositeDisposable);
                var position = new Vector3ReactiveProperty();
                position.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x => spacePlace.Value = LevelDesisionIsObjected.CheckPositionAndGetDirection2D(spaceOwnTransform, x, borderOffset))
                    .AddTo(_compositeDisposable);

                // 左空間の操作状態／右空間の操作状態
                var halo = (Behaviour)GetComponent("Halo");
                var isMoving = new BoolReactiveProperty[2];
                isMoving[0] = new BoolReactiveProperty();
                isMoving[1] = new BoolReactiveProperty();
                foreach (var m in isMoving)
                    m.ObserveEveryValueChanged(x => x.Value)
                        .Subscribe(_ =>
                        {
                            halo.enabled = 0 < isMoving.Where(x => x != null && x.Value)
                                .Select(x => x)
                                .ToArray().Length;
                        })
                        .AddTo(_compositeDisposable);
                this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        position.Value = transform.localPosition;

                        var player = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().Player;
                        if (player != null)
                        {
                            var spaceInput = LevelDesisionIsObjected.SetMoveVelocotyLeftAndRight();
                            for (var i = 0; i < spaceInput.Length; i++)
                            {
                                if (0f < spaceInput[i].magnitude && _moveEnable.Value)
                                {
                                    Movement((Direction2D)i, spacePlace, currentTransform, player.transform.localPosition, spaceOwner.GetComponent<SpaceOwner>().SpaceDirections, isMoving[i]);
                                }
                                else
                                {
                                    isMoving[i].Value = false;
                                }
                            }
                        }
                    })
                    .AddTo(_compositeDisposable);

                // 静的オブジェクトに衝突したら後退する
                ParticleSystem particle;
                var pool = new PoolGroup();
                pool.Pools = new Transform[1];
                pool.Pools[0] = GameObject.Find(droneCollisionEffectPrefab.name + "Pool") != null ? GameObject.Find(droneCollisionEffectPrefab.name + "Pool").transform : new GameObject(droneCollisionEffectPrefab.name + "Pool").transform;
                var isDead = new BoolReactiveProperty();
                this.OnTriggerEnterAsObservable()
                    .Subscribe(async x =>
                    {
                        if (0 < collierTagNames.Where(q => x.CompareTag(q))
                            .Select(x => x)
                            .ToArray().Length)
                        {
                            if (_moveEnable.Value)
                            {
                                _moveEnable.Value = false;
                                DOVirtual.DelayedCall(returnDelay, () => _moveEnable.Value = true);
                                currentTransform.DOShakeRotation(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);
                                if (pool.Pools[0].childCount <= 0)
                                {
                                    var obj = Instantiate(droneCollisionEffectPrefab, x.ClosestPointOnBounds(currentTransform.position), Quaternion.identity, pool.Pools[0]);
                                    particle = obj.GetComponent<ParticleSystem>();
                                }
                                else
                                {
                                    particle = pool.Pools[0].GetChild(0).GetComponent<ParticleSystem>();
                                    particle.transform.position = x.ClosestPointOnBounds(currentTransform.position);
                                }
                                particle.Play();

                                transform.localPosition = Vector3.MoveTowards(currentTransform.localPosition, x.transform.localPosition, -1f * returnDashSpeed * Time.deltaTime);
                            }
                        }
                        if (x.CompareTag(TagConst.TAG_NAME_PLAYER) && !isDead.Value)
                        {
                            // プレイヤーを死亡させる
                            isDead.Value = true;
                            await GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().DestroyPlayer();
                            GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SetSceneIdUndo();
                            GameManager.Instance.UIOwner.GetComponent<UIOwner>().EnableDrawLoadNowFadeOutTrigger();
                        }
                    })
                    .AddTo(_compositeDisposable);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// ドローンの移動制御
        /// </summary>
        /// <param name="direction">どちらの空間が操作されたか</param>
        /// <param name="spacePlace">ドローンが存在する空間</param>
        /// <param name="transform">ドローンのTransform</param>
        /// <param name="targetPosition">追跡対象のローカル位置座標</param>
        /// <param name="spaceDirection2D">空間操作の情報</param>
        private void Movement(Direction2D direction, IntReactiveProperty spacePlace, Transform transform, Vector3 targetPosition, SpaceDirection2D spaceDirection2D, BoolReactiveProperty isMoving)
        {
            // 空間操作ブロックがある
            if (IsMovingRigidbodies(spaceDirection2D.RbsLeftSpace) && (direction.Equals(Direction2D.Left) && spacePlace.Value.Equals((int)Direction2D.Left)) ||
                IsMovingRigidbodies(spaceDirection2D.RbsRightSpace) && (direction.Equals(Direction2D.Right) && spacePlace.Value.Equals((int)Direction2D.Right)))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
                isMoving.Value = true;
            }
        }

        /// <summary>
        /// 空間操作中のブロック判定（自然移動は含まない）
        /// </summary>
        /// <param name="rigidbodies">Rigidbodyの配列</param>
        /// <returns>動きあり／動きなし</returns>
        private bool IsMovingRigidbodies(Rigidbody[] rigidbodies)
        {
            try
            {
                if (rigidbodies != null && 0 < rigidbodies.Length)
                    foreach (var group in rigidbodies)
                    {
                        var magn = group.velocity.magnitude;
                        if (0f < magn)
                            return true;
                    }
                return false;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public bool Exit()
        {
            try
            {
                _compositeDisposable.Clear();
                return true;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}
