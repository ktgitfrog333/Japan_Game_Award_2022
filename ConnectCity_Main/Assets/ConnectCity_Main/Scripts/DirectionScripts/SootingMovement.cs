using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Main.Common;

namespace Main.Direction
{
    /// <summary>
    /// 拡散されたパーティクルがターゲットに向かってVelocity制御により移動させる
    /// </summary>
    public class SootingMovement : MonoBehaviour
    {
        /// <summary>拡散パーティクルのプレハブ</summary>
        [SerializeField] private GameObject diffusionParticlePrefab;
        /// <summary>パーティクル第二モーション実行フラグ</summary>
        private BoolReactiveProperty _success = new BoolReactiveProperty();
        /// <summary>ステージ開始のカットシーン</summary>
        [SerializeField] private GameObject startCutscene;
        /// <summary>プレイヤーの位置</summary>
        [SerializeField] private GameObject[] playerPositions;

        /// <summary>追尾対象</summary>
        public Transform Target => playerPositions[SceneInfoManager.Instance.SceneIdCrumb.Current].transform;

        /// <summary>
        /// 流星のような挙動の再生
        /// ダミーのアニメーションからフラグを更新
        /// </summary>
        public void OnAnimShoot()
        {
            _success.Value = true;
        }

        private void Reset()
        {
            if (startCutscene == null)
                startCutscene = GameObject.Find("StartCutscene");
            playerPositions = GameObject.FindGameObjectsWithTag("Player");
        }

        private void Start()
        {
            _success.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    var diffusonShootingStar = GetComponent<ParticleSystem>();
                    var subParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
                    var main = subParticle.main;
                    main.startSize = .3f;

                    // プレイヤー生成位置に拡散エフェクトを発生させる
                    var complated = false;
                    var targetTrigger = new GameObject("ShootingTargetTrigger");
                    targetTrigger.transform.position = Target.position;
                    targetTrigger.AddComponent<BoxCollider>();
                    var coroutine = TechnicalParticleMotion.CoroutineMoveShootTarget(diffusonShootingStar, Target);
                    this.OnParticleCollisionAsObservable()
                        .Where(x => x.name.Equals("ShootingTargetTrigger") && !complated)
                        .Subscribe(_ =>
                        {
                            complated = true;
                            // プレイヤーを有効にする
                            GameManager.Instance.Player.SetActive(true);
                            if (!InstanceDiffusion())
                                Debug.LogError("拡散パーティクル生成の失敗");
                            Destroy(targetTrigger);
                            StopCoroutine(coroutine);
                            diffusonShootingStar.Stop();
                            _success.Value = false;
                            startCutscene.GetComponent<StartCutscene>().StopPlayAbleFromSootingMovement();
                        });

                    StartCoroutine(coroutine);

                    Observable.FromCoroutine<bool>(observer => TechnicalParticleMotion.CoroutineMoveShootTargetLimit(observer, .5f))
                        .Where(_ => !complated)
                        .Subscribe(x =>
                        {
                            complated = x;
                            // プレイヤーを有効にする
                            GameManager.Instance.Player.SetActive(true);
                            if (!InstanceDiffusion())
                                Debug.LogError("拡散パーティクル生成の失敗");
                            Destroy(targetTrigger);
                            StopCoroutine(coroutine);
                            diffusonShootingStar.Stop();
                            _success.Value = false;
                            startCutscene.GetComponent<StartCutscene>().StopPlayAbleFromSootingMovement();
                        })
                        .AddTo(gameObject);
                });
        }

        /// <summary>
        /// 拡散パーティクル生成
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool InstanceDiffusion()
        {
            var par = Instantiate(diffusionParticlePrefab, Target.position, Quaternion.identity);
            DOVirtual.DelayedCall(3f, () => Destroy(par));
            return true;
        }
    }

    /// <summary>
    /// パーティクルモーション
    /// </summary>
    public class TechnicalParticleMotion
    {
        /// <summary>
        /// 対象まで追尾する
        /// </summary>
        /// <param name="ps">パーティクル</param>
        /// <returns>コルーチン</returns>
        public static IEnumerator CoroutineMoveShootTarget(ParticleSystem ps, Transform target)
        {
            var loop = 0;
            while (true)
            {
                var particles = new ParticleSystem.Particle[ps.main.maxParticles];
                int count = ps.GetParticles(particles);
                for (var i = 0; i < count; i++)
                {
                    var forward = ps.transform.TransformPoint(particles[i].velocity);
                    var position = ps.transform.TransformPoint(particles[i].position);

                    // ターゲットへのベクトル
                    var direction = (target.TransformPoint(target.position) - position).normalized;

                    var period = 1 - (particles[i].remainingLifetime / particles[i].startLifetime);

                    particles[i].velocity = ps.transform.InverseTransformPoint(forward + direction * period);
                }
                ps.SetParticles(particles, count);
                yield return new WaitForSeconds(.01f);
                // ループ制限
                loop++;
                if (1000 < loop) break;
            }
        }

        /// <summary>
        /// 演出のタイムリミット
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public static IEnumerator CoroutineMoveShootTargetLimit(System.IObserver<bool> observer, float limitTime)
        {
            yield return new WaitForSeconds(limitTime);
            observer.OnNext(true);
        }
    }
}
