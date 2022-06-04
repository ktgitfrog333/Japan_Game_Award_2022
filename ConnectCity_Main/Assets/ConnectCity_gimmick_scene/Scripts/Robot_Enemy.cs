using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Audio;
using Main.Common.LevelDesign;
using Main.Common.Const;
using Main.Common;
using Main.UI;

namespace Gimmick
{
    /// <summary>
    /// ロボットの敵
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Robot_Enemy : MonoBehaviour
    {
        /// <summary>移動速度</summary>
        [SerializeField] private float moveSpeed = 4f;
        ///// <summary>移動距離</summary>
        //[SerializeField] private float distance;
        /// <summary>キャラクター制御</summary>
        [SerializeField] private CharacterController _characterCtrl;
        /// <summary>プレイヤーのアニメーション</summary>
        [SerializeField] private Animator _playerAnimation;
        /// <summary>移動方向切り替え間隔</summary>
        [SerializeField] private float direChgDelaySec = 2f;
        /// <summary>パーティクルシステムの配列</summary>
        [SerializeField] private GameObject diedLight;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, 0.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirection = Vector3.down;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistance = 0.8f;
        [SerializeField] private Vector3 rayOriginOffsetLeft = new Vector3(.1f, 0f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirectionLeft = Vector3.left;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistanceLeft = .8f;
        ///// <summary>接地判定用のレイ　当たり判定の最大距離（プレス用）</summary>
        //[SerializeField] private float rayMaxDistanceLeftLong = 1.6f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffsetRight = new Vector3(-.1f, 0f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirectionRight = Vector3.right;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistanceRight = .8f;
        ///// <summary>接地判定用のレイ　当たり判定の最大距離（プレス用）</summary>
        //[SerializeField] private float rayMaxDistanceRightLong = 1.6f;
        /// <summary>敵を吹き飛ばす先の座標（死亡状態）</summary>
        [SerializeField] private Vector3 smashedPosition = Vector3.back * 100f;
        /// <summary>死亡状態</summary>
        private bool _isDead;

        private void Reset()
        {
            if (_characterCtrl == null)
                _characterCtrl = GetComponent<CharacterController>();
            if (_playerAnimation == null)
                _playerAnimation = GetComponent<Animator>();
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        public void Initialize()
        {
            if (_isDead)
                _isDead = false;
            if (!_characterCtrl.enabled)
                _characterCtrl.enabled = true;
        }

        void Start()
        {
            // 移動先の座標（X軸の移動のみ）
            var moveVelocity = new Vector3();

            // 一定時間での切り替え
            var horizontalDire = new IntReactiveProperty(1);
            Observable.Timer(System.TimeSpan.Zero, System.TimeSpan.FromSeconds(direChgDelaySec))
                .Subscribe(_ => horizontalDire.Value *= -1)
                .AddTo(gameObject);

            // 移動入力に応じて移動座標をセット
            horizontalDire.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    moveVelocity = Vector3.left * x * moveSpeed;
                    // 移動時にx座標反転（元オブジェクト補正）
                    transform.LookAt(transform.position + new Vector3((moveVelocity.x)/* * -1f*/, 0f, 0f));
                });

            // 空中にいる際の移動座標をセット
            this.UpdateAsObservable()
                .Where(_ => !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                .Subscribe(_ => moveVelocity.y += Physics.gravity.y * Time.deltaTime);

            // 移動
            this.FixedUpdateAsObservable()
                // 近くにブロックが存在しない
                // <<左にブロックが有り　かつ　>>右に移動
                // >>右にブロックが有り　かつ　<<左に移動
                .Where(_ => (!LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffsetLeft, rayDirectionLeft, rayMaxDistanceLeft, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffsetRight, rayDirectionRight, rayMaxDistanceRight, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE))) ||
                    (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffsetLeft, rayDirectionLeft, rayMaxDistanceLeft, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) &&
                    0f < moveVelocity.x) ||
                    (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffsetRight, rayDirectionRight, rayMaxDistanceRight, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) &&
                    moveVelocity.x < 0f))
                .Subscribe(_ => {
                    if (!PlayPlayerAnimation(moveVelocity)) Debug.LogError("移動アニメーション処理に失敗");
                    if (_characterCtrl.enabled)
                        _characterCtrl.Move(moveVelocity * Time.deltaTime);
                });
            
            // プレイヤーを死亡させる
            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER) && !_isDead)
                .Subscribe(async _ =>
                {
                    _isDead = true;
                    await LevelOwner.Instance.DeadPlayerFromRobotEnemies();
                    SceneOwner.Instance.SetSceneIdUndo();
                    UIOwner.Instance.EnableDrawLoadNowFadeOutTrigger();
                });
        }

        /// <summary>
        /// プレイヤーのアニメーションを再生
        /// </summary>
        /// <param name="velocity">移動先</param>
        /// <returns>成功／失敗</returns>
        private bool PlayPlayerAnimation(Vector3 velocity)
        {
            // 歩行状態
            if (velocity.y <= 0f)
                _playerAnimation.SetFloat(Robot_EnemyAnimator.PARAMETERS_MOVESPEED, Mathf.Abs(velocity.x));
            return true;
        }

        /// <summary>
        /// ゲームオブジェクトからプレイヤー操作を実行
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveRobotEnemyFromLevelOwner(Vector3 moveVelocity)
        {
            if (_characterCtrl == null)
                return false;
            if (_characterCtrl.enabled)
                _characterCtrl.Move(moveVelocity);
            return true;
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DeadPlayerFromLevelOwner()
        {
            // 圧死時のパーティクル
            Instantiate(diedLight, transform.position, Quaternion.identity);
            // 圧死音SE
            SfxPlay.Instance.PlaySFX(ClipToPlay.se_player_dead);
            _characterCtrl.enabled = false;
            transform.localPosition = smashedPosition;
            return true;
        }
    }
    /// <summary>
    /// プレイヤーのAnimatorで使用するパラメータを管理
    /// </summary>
    public class Robot_EnemyAnimator
    {
        /// <summary>
        /// 移動速度のパラメータ
        /// </summary>
        public static readonly string PARAMETERS_MOVESPEED = "MoveSpeed";
    }
}
