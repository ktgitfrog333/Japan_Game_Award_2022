using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Common.LevelDesign;
using Main.Common.Const;
using Main.Audio;
using System.Threading.Tasks;
using Main.Common;
using Main.Level;
using Main.InputSystem;

namespace Main.Player
{
    /// <summary>
    /// プレイヤー操作制御
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>移動速度</summary>
        [SerializeField] private float moveSpeed = 4f;
        /// <summary>ジャンプ速度</summary>
        [SerializeField] private float jumpSpeed = 6f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, 0.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirection = Vector3.down;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistance = 0.8f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayUpOriginOffset = new Vector3(0f, -.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayUpDirection = Vector3.up;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayUpMaxDistance = 0.8f;
        /// <summary>キャラクター制御</summary>
        [SerializeField] private CharacterController _characterCtrl;
        /// <summary>プレイヤーのアニメーション</summary>
        [SerializeField] private Animator _playerAnimation;
        /// <summary>パーティクルシステムの配列</summary>
        [SerializeField] private ParticleSystem[] _particleSystems;
        /// <summary>操作状況に応じて変化する光パーティクル</summary>
        [SerializeField] private ParticleSystem signalHeartLight;
        /// <summary>進む信号カラー</summary>
        [SerializeField] private Color GoLight = new Color(0f, 255f, 182f, 255f);
        /// <summary>停止信号カラー</summary>
        [SerializeField] private Color StopLight = new Color(255f, 70f, 0f, 255f);
        /// <summary>
        /// ジャンプSEの設定
        /// </summary>
        [SerializeField] private ClipToPlay _SEJump = ClipToPlay.se_player_jump_No1;
        /// <summary>圧死音SEの設定</summary>
        [SerializeField] private ClipToPlay _SEDead = ClipToPlay.se_player_dead;
        /// <summary>重力速度の最小値</summary>
        [SerializeField] private float minGravity = -1f;
        /// <summary>重力速度の最大値</summary>
        [SerializeField] private float maxGravity = -10f;
        /// <summary>ジャンプ状態</summary>
        private BoolReactiveProperty _isJumped = new BoolReactiveProperty();
        /// <summary>入力禁止</summary>
        private BoolReactiveProperty _inputBan = new BoolReactiveProperty();
        /// <summary>入力禁止</summary>
        public bool InputBan
        {
            get => _inputBan.Value;
            set => _inputBan.Value = value;
        }

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
                _playerAnimation = transform.GetChild(2).GetComponent<Animator>();
            if (_particleSystems.Length <= 0f)
            {
                var particleList = new List<ParticleSystem>();
                for (var i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<ParticleSystem>() != null)
                    {
                        particleList.Add(transform.GetChild(i).GetComponent<ParticleSystem>());
                    }
                }
                if (particleList.Count < 1) Debug.LogError("パーティクルのセットが失敗");
                _particleSystems = particleList.ToArray();
            }
            // 操作状態のパーティクル
            if (signalHeartLight == null)
                signalHeartLight = transform.GetChild(2).GetChild(3).GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            // 位置・スケールのキャッシュ
            var transform = base.transform;
            // 移動先の座標（X軸の移動、Y軸のジャンプのみ）
            var moveVelocity = new Vector3();

            _inputBan.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    signalHeartLight.Stop();
                    var m = signalHeartLight.main;
                    signalHeartLight.Play();
                    if (!x)
                        m.startColor = GoLight;
                    else
                    {
                        m.startColor = StopLight;
                        moveVelocity = Vector3.zero;
                    }
                });
            // 移動入力に応じて移動座標をセット
            this.UpdateAsObservable()
                .Where(_ => !_inputBan.Value)
                .Select(_ => GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputPlayer.Moved.x /*Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL)*/ * moveSpeed)
                .Subscribe(x =>
                {
                    moveVelocity.x = x;
                    // 移動時にx座標反転（元オブジェクト補正）
                    transform.LookAt(transform.position + new Vector3((moveVelocity.x) * -1f, 0f, 0f));
                });
            // ジャンプ入力に応じてジャンプフラグをセット
            this.UpdateAsObservable()
                .Where(_ => !_inputBan.Value)
                .Where(_ => !_isJumped.Value &&
                    (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE))) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayUpOriginOffset, rayUpDirection, rayUpMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rayUpOriginOffset, rayUpDirection, rayUpMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                .Select(_ => GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputPlayer.Jumped /*Input.GetButtonDown(InputConst.INPUT_CONSTJUMP)*/)
                .Where(x => x)
                .Subscribe(x => _isJumped.Value = x);
            // ジャンプフラグ切り替え
            _isJumped.Where(x => x)
                .Subscribe(_ =>
                {
                    moveVelocity.y = jumpSpeed;
                    GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(_SEJump);
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
                .Select(_ => moveVelocity.y < minGravity)
                .Where(x => x)
                .Subscribe(x =>
                {
                    moveVelocity.y = minGravity;
                });
            // 空中にいる際の移動座標をセット
            var rOrgOffAry = LevelDesisionIsObjected.GetTwoPointHorizontal(rayOriginOffset, .5f);
            this.UpdateAsObservable()
                .Where(_ => !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[0], rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[1], rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[0], rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[1], rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                .Subscribe(_ => moveVelocity.y = moveVelocity.y < maxGravity ? maxGravity : moveVelocity.y + Physics.gravity.y * Time.deltaTime);
            // 天井・ブロックへの衝突
            var rUpOrgOffAry = LevelDesisionIsObjected.GetTwoPointHorizontal(rayUpOriginOffset, .5f);
            this.UpdateAsObservable()
                .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rUpOrgOffAry[0], rayUpDirection, rayUpMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rUpOrgOffAry[1], rayUpDirection, rayUpMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rUpOrgOffAry[0], rayUpDirection, rayUpMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rUpOrgOffAry[1], rayUpDirection, rayUpMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                .Subscribe(_ =>
                {
                    moveVelocity.y = moveVelocity.y < maxGravity ? maxGravity : moveVelocity.y + Physics.gravity.y * Time.deltaTime;
                    if (_characterCtrl.enabled)
                        _characterCtrl.Move(moveVelocity * Time.deltaTime);
                });

            // 移動
            this.FixedUpdateAsObservable()
                .Subscribe(_ => {
                    if (!PlayPlayerAnimation(moveVelocity)) Debug.LogError("移動アニメーション処理に失敗");
                    if (_characterCtrl.enabled)
                        _characterCtrl.Move(moveVelocity * Time.deltaTime);
                });
            // 死亡時からのリセット場合
            // メッシュが無効になっているなら有効にする
            // 死亡パーティクルが有効になっているなら無効にする
            // キャラクターコントローラーが有効になっているなら有効にする
            this.OnEnableAsObservable()
                .Subscribe(_ =>
                {
                    var model = transform.GetChild(2);
                    if (!model.gameObject.activeSelf)
                        model.gameObject.SetActive(true);
                    if (_particleSystems[(int)PlayerEffectIdx.DiedLight].gameObject.activeSelf)
                        _particleSystems[(int)PlayerEffectIdx.DiedLight].gameObject.SetActive(false);
                    if (!GetComponent<CharacterController>().enabled)
                        GetComponent<CharacterController>().enabled = true;
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
            }
        }

        /// <summary>
        /// ゲームオブジェクトからプレイヤー操作を実行
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveChatactor(Vector3 moveVelocity)
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
        public async Task<bool> DeadPlayer()
        {
            var model = transform.GetChild(2);
            if (model.gameObject.activeSelf)
            {
                model.gameObject.SetActive(false);
                GetComponent<CharacterController>().enabled = false;
            }
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
            if (!_particleSystems[(int)PlayerEffectIdx.DiedLight].gameObject.activeSelf)
                _particleSystems[(int)PlayerEffectIdx.DiedLight].gameObject.SetActive(true);
            // 圧死音SE
            GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(_SEDead);
            GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SetSpaceOwnerInputBan(true);
            _inputBan.Value = true;
            await Task.Delay(3000);
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
        /// <summary>死亡エフェクト</summary>
        , DiedLight
    }
}
