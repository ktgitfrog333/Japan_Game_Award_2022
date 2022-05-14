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
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, .16f);
        /// <summary>SE設定</summary>
        [SerializeField] private ClipToPlay turretEnemiesSE = ClipToPlay.se_laser_No1;

        private void Start()
        {
            var distance = new FloatReactiveProperty();
            var coroutine = ShotBeamStretch(distance);
            // インターバルしつつ3秒後に実行
            Observable.Timer(System.TimeSpan.Zero, System.TimeSpan.FromSeconds(bulletInterval))
                .Subscribe(_ =>
                {
                    distance.Value = 0f;
                    StartCoroutine(coroutine);
                    SfxPlay.Instance.PlaySFX(turretEnemiesSE);
                })
                .AddTo(gameObject);

            var lineTran = transform.GetChild(2).GetChild(0);
            var lineRenderer = lineTran.GetComponent<LineRenderer>();
            var playerDead = false;
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
                .Select(_ => CheckRayHitObjectState(distance.Value))
                .Where(x => -1 < x)
                .Subscribe(async x =>
                {
                    // 何かに当たったらレーザーは伸びないでそのまま
                    StopCoroutine(coroutine);
                    if (0 == x && !playerDead)
                    {
                        playerDead = true;
                        // プレイヤーを死亡
                        await GameManager.Instance.DeadPlayerFromTurretEnemies();
                        SceneInfoManager.Instance.SetSceneIdUndo();
                        UIManager.Instance.EnableDrawLoadNowFadeOutTrigger();
                    }
                });
        }

        /// <summary>
        /// レイの当たり判定
        /// 貫通したレイヤー情報を元に実行ステータスを返す
        /// </summary>
        /// <param name="distance">レイの距離</param>
        /// <returns>ステータス（プレイヤー／ブロック系）</returns>
        private int CheckRayHitObjectState(float distance)
        {
            if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.GetChild(2).position, rayOriginOffset, LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection), distance, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
            {
                // プレイヤー
                return 0;
            }
            else if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.GetChild(2).position, rayOriginOffset, LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection), distance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.GetChild(2).position, rayOriginOffset, LevelDesisionIsObjected.GetVectorFromDirection(bulletDirection), distance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
            {
                // 動かないブロック or 空間操作ブロック or ぼろいブロック・天井
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// レーザーの距離を伸ばす
        /// </summary>
        /// <param name="distance">距離</param>
        /// <returns>コルーチン</returns>
        private IEnumerator ShotBeamStretch(FloatReactiveProperty distance)
        {
            Debug.Log("ShotBeamStretch");
            while (true)
            {
                distance.Value += bulletSpeed;
                yield return new WaitForSeconds(bulletSubInterval);
            }
        }
    }
}
