using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System.Threading.Tasks;
using Main.Common;

namespace Main.Direction
{
    /// <summary>
    /// ゴール演出
    /// </summary>
    public class EndCutscene : MonoBehaviour
    {
        /// <summary>各カットの待機時間（秒）</summary>
        [SerializeField] private float[] cutsWaitForSec = { 1.5f, .5f, .5f };
        /// <summary>ブラックホールパーティクルのプレハブ</summary>
        [SerializeField] private GameObject blackHolePrefab;
        /// <summary>ブラックホールパーティクルのプレハブ一時格納</summary>
        private GameObject instanceBlackHolePrefab;
        /// <summary>キューブパーティクルのプレハブ</summary>
        [SerializeField] private GameObject diffusionCubesPrefab;
        /// <summary>カメラの拡大率</summary>
        [SerializeField] private float fieldOfViewVolume = 20f;
        /// <summary>拡大アニメーションの生存期間</summary>
        [SerializeField] private float zoomDuration = .6f;
        /// <summary>拡散キューブのパーティクルが向かうターゲット当たり判定</summary>
        [SerializeField] private Vector3 boxColliderSize = new Vector3(0.5f, 0.5f, 0.5f);

        /// <summary>
        /// 初期処理
        /// </summary>
        public IEnumerator Initialize(System.IObserver<bool> observer)
        {
            var target = GameManager.Instance.GoalPoint.transform;

            // カメラをズーム
            // ブラックホールと光が出現
            var cut1 = new BoolReactiveProperty();
            Observable.FromCoroutine<bool>(observer => InstanceBlackHole(observer, target))
                .Subscribe(x => cut1.Value = x)
                .AddTo(gameObject);
            // プレイヤーを非表示にしてキューブのパーティクル出現
            // 扉の向こうへキューブが向かう
            var cut2 = new BoolReactiveProperty();
            cut1.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    Observable.FromCoroutine<bool>(observer => InstanceDiffusion(observer, target))
                        .Subscribe(x => cut2.Value = x)
                        .AddTo(gameObject);
                });
            cut2.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    observer.OnNext(true);
                });
            yield return null;
        }

        /// <summary>
        /// プレハブが残っていた場合は削除
        /// </summary>
        public bool DestroyParticleFromFadeScreen()
        {
            try
            {
                if (instanceBlackHolePrefab != null)
                    Destroy(instanceBlackHolePrefab);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ＜カット1＞
        /// カメラをズーム
        /// ブラックホールを生成
        /// </summary>
        /// <param name="observer">任意発行のObservber</param>
        /// <returns>コルーチン</returns>
        private IEnumerator InstanceBlackHole(System.IObserver<bool> observer, Transform target)
        {
            // カメラのズーム
            var camera = Camera.main;
            var zoomCompCnt = new IntReactiveProperty();
            camera.transform.DOLocalMove(new Vector3(target.position.x, target.position.y, camera.transform.position.z), zoomDuration)
                .OnComplete(() => zoomCompCnt.Value++);
            camera.DOFieldOfView(fieldOfViewVolume, zoomDuration)
                .OnComplete(() => zoomCompCnt.Value++);
            zoomCompCnt.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x == 2)
                .Subscribe(async _ =>
                {
                    // ブラックホールのパーティクルを生成
                    instanceBlackHolePrefab = Instantiate(blackHolePrefab, target.position, Quaternion.identity);

                    await Task.Delay(((int)cutsWaitForSec[0]) * 1000);

                    // 生成後にOnNextを発行
                    observer.OnNext(true);
                });
            // OnNext任意発行のためコルーチンのタイミングでフックさせない
            yield return null;
        }

        /// <summary>
        /// ＜カット2＞
        /// プレイヤーがキューブのパーティクルへ変化
        /// ブラックホールへ吸い込まれる
        /// </summary>
        /// <param name="observer">任意発行のObservber</param>
        /// <returns>コルーチン</returns>
        private IEnumerator InstanceDiffusion(System.IObserver<bool> observer, Transform target)
        {
            var player = GameManager.Instance.Player;
            player.transform.GetChild(2).gameObject.SetActive(false);
            var obj = Instantiate(diffusionCubesPrefab, player.transform.position, Quaternion.identity);
            var particle = obj.GetComponent<ParticleSystem>();

            yield return new WaitForSeconds(cutsWaitForSec[1]);

            var complated = false;
            var targetTrigger = new GameObject("DiffusionCubesTrigger");
            targetTrigger.transform.position = target.position;
            targetTrigger.AddComponent<BoxCollider>().size = boxColliderSize;
            var coroutine = TechnicalParticleMotion.CoroutineMoveShootTarget(particle, target);
            obj.OnParticleCollisionAsObservable()
                .Where(x => x.name.Equals("DiffusionCubesTrigger") && !complated)
                .Subscribe(async _ =>
                {
                    complated = true;

                    await Task.Delay(((int)cutsWaitForSec[2]) * 1000);

                    Destroy(targetTrigger);
                    StopCoroutine(coroutine);
                    Destroy(obj);
                    // 生成後にOnNextを発行
                    observer.OnNext(true);
                });
            StartCoroutine(coroutine);
            // OnNext任意発行のためコルーチンのタイミングでフックさせない
            yield return null;
        }
    }
}
