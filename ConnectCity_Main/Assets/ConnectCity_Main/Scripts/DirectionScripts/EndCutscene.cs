using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System.Threading.Tasks;

namespace Main.Direction
{
    /// <summary>
    /// ゴール演出
    /// </summary>
    public class EndCutscene : MonoBehaviour
    {
        /// <summary>ゴールの位置</summary>
        [SerializeField] private GameObject[] goalPositions;
        /// <summary>各カットの待機時間（秒）</summary>
        [SerializeField] private float[] cutsWaitForSec = { 1.5f, .5f, .5f };
        /// <summary>ブラックホールパーティクルのプレハブ</summary>
        [SerializeField] private GameObject blackHolePrefab;
        /// <summary>キューブパーティクルのプレハブ</summary>
        [SerializeField] private GameObject diffusionCubesPrefab;
        /// <summary>カメラの拡大率</summary>
        [SerializeField] private float fieldOfViewVolume = 20f;
        /// <summary>拡大アニメーションの生存期間</summary>
        [SerializeField] private float zoomDuration = .6f;
        /// <summary>拡散キューブのパーティクルが向かうターゲット当たり判定</summary>
        [SerializeField] private Vector3 boxColliderSize = new Vector3(0.5f, 0.5f, 0.5f);
        /// <summary>追尾対象</summary>
        public Transform Target => goalPositions[0].transform;

        private void Reset()
        {
            goalPositions = GameObject.FindGameObjectsWithTag("GoalPoint");
        }

        private void Start()
        {
            // カメラをズーム
            // ブラックホールと光が出現
            var cut1 = new BoolReactiveProperty();
            Observable.FromCoroutine<bool>(observer => InstanceBlackHole(observer))
                .Subscribe(x => cut1.Value = x)
                .AddTo(gameObject);
            // プレイヤーを非表示にしてキューブのパーティクル出現
            // 扉の向こうへキューブが向かう
            var cut2 = new BoolReactiveProperty();
            cut1.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    Observable.FromCoroutine<bool>(observer => InstanceDiffusion(observer))
                        .Subscribe(x => cut2.Value = x)
                        .AddTo(gameObject);
                });
            cut2.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    // T.B.D ステージクリアロゴを表示させる
                    Debug.Log("ステージクリアロゴが出現");
                });
        }

        /// <summary>
        /// ＜カット1＞
        /// カメラをズーム
        /// ブラックホールを生成
        /// </summary>
        /// <param name="observer">任意発行のObservber</param>
        /// <returns>コルーチン</returns>
        private IEnumerator InstanceBlackHole(System.IObserver<bool> observer)
        {
            // カメラのズーム
            var camera = Camera.main;
            var zoomCompCnt = new IntReactiveProperty();
            camera.transform.DOLocalMove(new Vector3(Target.position.x, camera.transform.position.y, camera.transform.position.z), zoomDuration)
                .OnComplete(() => zoomCompCnt.Value++);
            camera.DOFieldOfView(fieldOfViewVolume, zoomDuration)
                .OnComplete(() => zoomCompCnt.Value++);
            zoomCompCnt.ObserveEveryValueChanged(x => x.Value)
                .Where(x => x == 2)
                .Subscribe(async _ =>
                {
                    // ブラックホールのパーティクルを生成
                    Instantiate(blackHolePrefab, Target.position, Quaternion.identity);

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
        private IEnumerator InstanceDiffusion(System.IObserver<bool> observer)
        {
            // T.B.D GameManagerからプレイヤーを検出
            var player = GameObject.FindGameObjectWithTag("Player");
            player.transform.GetChild(2).gameObject.SetActive(false);
            var obj = Instantiate(diffusionCubesPrefab, player.transform.position, Quaternion.identity);
            var particle = obj.GetComponent<ParticleSystem>();

            yield return new WaitForSeconds(cutsWaitForSec[1]);

            var complated = false;
            var targetTrigger = new GameObject("DiffusionCubesTrigger");
            targetTrigger.transform.position = Target.position;
            targetTrigger.AddComponent<BoxCollider>().size = boxColliderSize;
            var coroutine = TechnicalParticleMotion.CoroutineMoveShootTarget(particle, Target);
            obj.OnParticleCollisionAsObservable()
                .Where(x => x.name.Equals("DiffusionCubesTrigger") && !complated)
                .Subscribe(async _ =>
                {
                    complated = true;
                    Destroy(targetTrigger);
                    StopCoroutine(coroutine);
                    Destroy(obj);

                    await Task.Delay(((int)cutsWaitForSec[2]) * 1000);

                    // 生成後にOnNextを発行
                    observer.OnNext(true);
                });
            StartCoroutine(coroutine);
            // OnNext任意発行のためコルーチンのタイミングでフックさせない
            yield return null;
        }
    }
}
