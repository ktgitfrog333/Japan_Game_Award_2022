using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Common.LevelDesign;
using Main.Common.Const;
using Main.Audio;

namespace Main.Player
{
    /// <summary>
    /// プレイヤー操作制御
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>移動速度</summary>
        [SerializeField] private float moveSpeed = 4f;
        /// <summary>ジャンプ速度</summary>
        [SerializeField] private float jumpSpeed = 5f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, 0.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirection = Vector3.down;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistance = 0.8f;
        /// <summary>キャラクター制御</summary>
        [SerializeField] private CharacterController _characterCtrl;
        /// <summary>プレイヤーのアニメーション</summary>
        [SerializeField] private Animator _playerAnimation;
        /// <summary>パーティクルシステムの配列</summary>
        [SerializeField] private ParticleSystem[] _particleSystems;
        /// <summary>ジャンプ状態</summary>
        private BoolReactiveProperty _isJumped = new BoolReactiveProperty();

        private void Reset()
        {
            Initialize();
        }

        /// <summary>
        /// 初期設定
        /// キャラクターコントローラー／アニメータ／パーティクルシステム
        /// </summary>
        private void Initialize()
        {
            if (_characterCtrl == null)
                _characterCtrl = GetComponent<CharacterController>();
            if (_playerAnimation == null)
                _playerAnimation = GetComponent<Animator>();
            if (_particleSystems.Length <= 0f)
            {
                var particleList = new List<ParticleSystem>();
                for (var i = 0; i < transform.childCount; i++)
                {
                    particleList.Add(transform.GetChild(i).GetComponent<ParticleSystem>());
                }
                if (particleList.Count < 1) Debug.LogError("パーティクルのセットが失敗");
                _particleSystems = particleList.ToArray();
            }
        }

        private void Start()
        {
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
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
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
                    if (!_particleSystems[(int)PlayerEffectIdx.RunDust].gameObject.activeSelf)
                        _particleSystems[(int)PlayerEffectIdx.RunDust].gameObject.SetActive(true);
                    _particleSystems[(int)PlayerEffectIdx.RunDust].Play();
                    _isJumped.Value = false;
                });
            // ジャンプ状態から着地した時
            this.UpdateAsObservable()
                .Where(_ => !_isJumped.Value &&
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                .Select(_ => moveVelocity.y < 0f)
                .Where(x => x)
                .Subscribe(x => {
                    moveVelocity.y = 0f;
                // T.B.D 着地SEが入るが、ひとまずメニューを閉じる音を鳴らす
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_close);
                    if (!_particleSystems[(int)PlayerEffectIdx.RunDust].gameObject.activeSelf)
                        _particleSystems[(int)PlayerEffectIdx.RunDust].gameObject.SetActive(true);
                    _particleSystems[(int)PlayerEffectIdx.RunDust].Play();
                });
            // 空中にいる際の移動座標をセット
            this.UpdateAsObservable()
                .Where(_ => !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                .Subscribe(_ => moveVelocity.y += Physics.gravity.y * Time.deltaTime);

            // 移動
            this.FixedUpdateAsObservable()
                .Subscribe(_ => {
                    if (!PlayPlayerAnimation(moveVelocity)) Debug.LogError("移動アニメーション処理に失敗");
                    _characterCtrl.Move(moveVelocity * Time.deltaTime);
                });

            // デバッグ用
            //this.UpdateAsObservable()
            //    .Subscribe(_ => _moveVelocity = moveVelocity);
        }

        //[SerializeField] private Vector3 _moveVelocity;

        /// <summary>
        /// プレイヤーのアニメーションを再生
        /// </summary>
        /// <param name="velocity">移動先</param>
        /// <returns>成功／失敗</returns>
        private bool PlayPlayerAnimation(Vector3 velocity)
        {
            // 歩行状態
            if (velocity.y <= 0f)
                _playerAnimation.SetFloat(PlayerAnimator.PARAMETERS_MOVESPEED, Mathf.Abs(velocity.x));
            // ジャンプ
            _playerAnimation.SetFloat(PlayerAnimator.PARAMETERS_JUMPSPEED, Mathf.Abs(velocity.y));
            return true;
        }

        /// <summary>
        /// 足が地面に付く
        /// アニメーションのトリガー
        /// </summary>
        public void OnFootGround()
        {
            if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
            {
                if (!_particleSystems[(int)PlayerEffectIdx.RunDust].gameObject.activeSelf)
                    _particleSystems[(int)PlayerEffectIdx.RunDust].gameObject.SetActive(true);
                _particleSystems[(int)PlayerEffectIdx.RunDust].Play();
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
            }
        }

        /// <summary>
        /// ゲームオブジェクトからプレイヤー操作を実行
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveChatactorFromGameManager(Vector3 moveVelocity)
        {
            if (_characterCtrl == null)
                return false;
            _characterCtrl.Move(moveVelocity);
            return true;
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DeadPlayerFromGameManager()
        {
            // T.B.D プレイヤーの死亡演出
            return true;
        }
    }
    /// <summary>
    /// プレイヤーのAnimatorで使用するパラメータを管理
    /// </summary>
    public class PlayerAnimator
    {
        /// <summary>
        /// 移動速度のパラメータ
        /// </summary>
        public static readonly string PARAMETERS_MOVESPEED = "MoveSpeed";
        /// <summary>
        /// ジャンプのパラメータ
        /// </summary>
        public static readonly string PARAMETERS_JUMPSPEED = "JumpSpeed";
    }
    /// <summary>
    /// プレイヤーのエフェクトで使用するエフェクト配列のインデックスを管理
    /// </summary>
    public enum PlayerEffectIdx
    {
        /// <summary>移動エフェクト</summary>
        RunDust
    }
}
