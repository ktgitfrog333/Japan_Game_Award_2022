using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Common.LevelDesign;
using Main.Common.Const;
using System.Threading.Tasks;
using Main.Common;
using Main.UI;
using Main.Audio;
using DG.Tweening;

namespace Gimmick
{
    /// <summary>
    /// レーザー砲
    /// </summary>
    public class TurretEnemies : MonoBehaviour
    {
        /// <summary>発射方向</summary>
        [SerializeField] private Direction bulletDirection = Direction.LEFT;
        /// <summary>発射速度</summary>
        [SerializeField] private float bulletSpeed = .5f;
        /// <summary>発射する間隔</summary>
        [SerializeField] private float bulletInterval = 4f;
        /// <summary>発射する間隔（サブ）</summary>
        [SerializeField] private float bulletSubInterval = .05f;
        /// <summary>弾がブロックに当たった時に縮まる距離</summary>
        [SerializeField] private float bulletDistanceDown = .1f;
        /// <summary>弾の生存期間</summary>
        [SerializeField] private float bulletDuration = 3f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, .16f);
        /// <summary>SE設定</summary>
        [SerializeField] private ClipToPlay turretEnemiesSE = ClipToPlay.se_laser_No1;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable;
        /// <summary>プレイヤーの死亡状態</summary>
        private bool _playerDead;

        /// <summary>
        /// 初期処理
        /// </summary>
        /// <returns>成功／失敗</returns>
        public void Initialize()
        {
            _compositeDisposable = new CompositeDisposable();
            if (_playerDead)
                _playerDead = false;
            var played = new BoolReactiveProperty().AddTo(_compositeDisposable);
            var distance = new FloatReactiveProperty().AddTo(_compositeDisposable);

            var coroutine = ShotBeamStretch(distance, played);
            // インターバルしつつ3秒後に実行
            Observable.Timer(System.TimeSpan.Zero, System.TimeSpan.FromSeconds(bulletInterval))
                .Subscribe(_ =>
                {
                    distance.Value = 0f;
                    played.Value = true;
                    DOVirtual.DelayedCall(bulletDuration, () =>
                    {
                        played.Value = false;
                    });
                    StartCoroutine(coroutine);
                    SfxPlay.Instance.PlaySFX(turretEnemiesSE);
                })
                .AddTo(_compositeDisposable)
                .AddTo(gameObject);

            var lineTran = transform.GetChild(2).GetChild(0);
            var lineRenderer = lineTran.GetComponent<LineRenderer>();
            this.UpdateAsObservable()
                .Do(_ =>
                {
                    var pos = new Vector3[]
                    {
                        transform.GetChild(2).position + rayOriginOffset,
                        transform.GetChild(2).position + rayOriginOffset + LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection) * distance.Value,
                    };
                    lineRenderer.useWorldSpace = true;
                    lineRenderer.SetPositions(pos);
                })
                .Subscribe(_ =>
                {
                    var disVal = CheckRayHitObjectState(distance.Value);
                    if (-1 < disVal)
                    {
                        // 何かに当たったら一旦とめる
                        StopCoroutine(coroutine);
                        if (0 == disVal && !_playerDead)
                        {
                            _playerDead = true;
                            // プレイヤーを死亡させる
                            DeadPlayer();
                        }
                        else if (1 == disVal)
                        {
                            distance.Value -= 0f < distance.Value ? bulletDistanceDown * Time.deltaTime : 0f;
                        }
                        else if (2 == disVal)
                        {
                            distance.Value = GetRayHitDistance(distance.Value, LayerConst.LAYER_NAME_MOVECUBE);
                        }
                    }
                    else
                    {
                        // 当たっていた何かが無くなったら再度伸ばす
                        StartCoroutine(coroutine);
                    }
                })
                .AddTo(_compositeDisposable);
        }

        /// <summary>
        /// 疑似的な終了
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool OnImitationDestroy()
        {
            _compositeDisposable.Clear();
            return true;
        }

        /// <summary>
        /// プレイヤーを死亡
        /// </summary>
        private async void DeadPlayer()
        {
            await GameManager.Instance.DeadPlayerFromTurretEnemies();
            SceneInfoManager.Instance.SetSceneIdUndo();
            UIManager.Instance.EnableDrawLoadNowFadeOutTrigger();
        }

        /// <summary>
        /// レイの当たり判定
        /// 貫通したレイヤー情報を元に実行ステータスを返す
        /// </summary>
        /// <param name="distance">レイの距離</param>
        /// <returns>ステータス（プレイヤー／ブロック（静的））／ブロック（動的）</returns>
        private int CheckRayHitObjectState(float distance)
        {
            if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.GetChild(2).position, rayOriginOffset, LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection), distance, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
            {
                // プレイヤー
                return 0;
            }
            else if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.GetChild(2).position, rayOriginOffset, LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection), distance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)))
            {
                // ブロック（静的オブジェクト）
                return 1;
            }
            else if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.GetChild(2).position, rayOriginOffset, LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection), distance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
            {
                // ブロック（動的オブジェクト）
                return 2;
            }
            return -1;
        }

        /// <summary>
        /// レイの当たり判定
        /// 指定した方向と距離とレイヤーから衝突したポイントの距離を返す
        /// ※横方向にレーザーを飛ばしている時に上下から空間操作ブロックが来て遮らせる挙動を実現
        /// </summary>
        /// <param name="distance">距離</param>
        /// <param name="layerName">レイヤー名</param>
        /// <returns>ポイント距離</returns>
        private float GetRayHitDistance(float distance, string layerName)
        {
            Ray ray = new Ray(transform.GetChild(2).position + rayOriginOffset, LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, distance, LayerMask.GetMask(layerName)))
            {
                //Debug.Log(hit.point);
            }
            return Vector3.Distance(hit.point, transform.GetChild(2).position);
        }

        /// <summary>
        /// レーザーの距離を伸ばす
        /// </summary>
        /// <param name="distance">距離</param>
        /// <returns>コルーチン</returns>
        private IEnumerator ShotBeamStretch(FloatReactiveProperty distance, BoolReactiveProperty played)
        {
            while (true)
            {
                if (played.Value)
                    distance.Value += bulletSpeed * Time.deltaTime;
                else
                    distance.Value = 0f;
                yield return new WaitForSeconds(bulletSubInterval);
            }
        }
    }
}
