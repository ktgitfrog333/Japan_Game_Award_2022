using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UniRx;
using UniRx.Triggers;
using Main.Common.Const;
using System.Linq;
using Main.Level;
using Main.Common.LevelDesign;

namespace Gimmick
{
    /// <summary>
    /// ワープゲート
    /// ペアによって管理する
    /// ワープAからはワープBへ移動する（逆パターンも含む）
    /// </summary>
    public class WarpGatesPair : MonoBehaviour, IOwner
    {
        /// <summary>コライダー対象とするオブジェクト</summary>
        [SerializeField] private string[] collierTagNames = { TagConst.TAG_NAME_PLAYER, TagConst.TAG_NAME_MOVECUBE, TagConst.TAG_NAME_ROBOT_EMEMY };
        /// <summary>空間操作ブロックの移動先チェックオブジェクトのプレハブ</summary>
        [SerializeField] private GameObject checkCbSmallPrefab;
        /// <summary>空間操作ブロックの移動先チェックオブジェクトグループのプレハブ</summary>
        [SerializeField] private GameObject checkCbSmallGroupPrefab;
        /// <summary>一辺を描画するレイのマップ</summary>
        [SerializeField] private CubeRaysMap[] raysMap;
        /// <summary>一辺を描画するレイの距離</summary>
        [SerializeField] private float distance;

        /// <summary>
        /// 一辺を描画するレイのマップ
        /// originPosition
        /// direction
        /// </summary>
        [System.Serializable]
        public class CubeRaysMap
        {
            /// <summary>ゴーストキューブのローカルPath</summary>
            [SerializeField] private Vector3[] dotPosition;
            /// <summary>ゴーストキューブのローカルPath</summary>
            public Vector3[] DotPosition => dotPosition;
        }

        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        /// <summary>移動チェック用オブジェクト</summary>
        private GameObject _checkObject;

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool ManualStart()
        {
            try
            {
                var parent = transform;
                var warpA = parent.GetChild(0);
                var warpB = parent.GetChild(1);
                // T.B.D 移動方法はひとまず単一エレベーター式を採用（※複数タクシー式（複数のオブジェクトごとにフラグ管理）に戻す可能性もあり）
                var isMoving = new BoolReactiveProperty();
                // 確認用
                //isMoving.ObserveEveryValueChanged(x => x.Value)
                //    .Subscribe(x => Debug.Log(x));
                for (var i = 0; i < parent.childCount; i++)
                {
                    Transform warp = parent.GetChild(i);
                    warp.OnTriggerEnterAsObservable()
                        //.Do(_ => Debug.Log("OnTriggerEnter"))
                        .Where(x => !isMoving.Value &&
                            0 < collierTagNames.Where(q => x.CompareTag(q)).Select(q => q).ToArray().Length)
                        .Select(x => x.gameObject)
                        .Subscribe(x =>
                        {
                            isMoving.Value = true;
                            if (!WarpCollisionEnvironment(x, warp, warpA, warpB))
                                throw new System.Exception("衝突エンバイロメント制御の失敗");
                        })
                        .AddTo(_compositeDisposable);
                    warp.OnTriggerExitAsObservable()
                        //.Do(_ => Debug.Log("OnTriggerExit"))
                        .Where(x => isMoving.Value &&
                            0 < collierTagNames.Where(q => x.CompareTag(q)).Select(q => q).ToArray().Length)
                        .Subscribe(_ =>
                        {
                            isMoving.Value = false;
                            if (_checkObject != null)
                                Destroy(_checkObject);
                        })
                        .AddTo(_compositeDisposable);
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 衝突した対象オブジェクトをワープさせる
        /// </summary>
        /// <param name="environment">エンバイロメント</param>
        /// <param name="warp">接触ワープ</param>
        /// <param name="warpA">ワープA（比較用）</param>
        /// <param name="warpB">ワープB（比較用）</param>
        /// <returns>成功／失敗</returns>
        private bool WarpCollisionEnvironment(GameObject environment, Transform warp, Transform warpA, Transform warpB)
        {
            try
            {
                //Debug.Log($"接触：{environment.name}");
                if (!ChangeFakeStatic(environment))
                    throw new System.Exception("静的変化の失敗");
                if (environment.CompareTag(TagConst.TAG_NAME_PLAYER) || environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    // 一度触れたワープポイントの中心に移動させる
                    environment.transform.position = warp.Equals(warpA) ? warpA.position : warpB.position;
                    // 移動先のワープへ配置
                    environment.transform.position = warp.Equals(warpA) ? warpB.position : warpA.position;
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    // 最終配置のポジションを算出
                    var lastPosition = warp.Equals(warpA) ?
                        GetCalcWarpPosition(environment.transform.parent.position, warpA.position, warpB.position) :
                        GetCalcWarpPosition(environment.transform.parent.position, warpB.position, warpA.position);
                    _checkObject = InstanceCheckCube(environment.transform.parent, lastPosition, warp, warpA, warpB);
                }
                else
                    throw new System.Exception("非対象オブジェクト");
                if (!ChangeDynamic(environment))
                    throw new System.Exception("動的変化の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 移動先当たり判定のレイを生成
        /// ワープ対象のオブジェクトがワープ先に移動できるか否かをチェックする
        /// </summary>
        /// <param name="originParent">移動対象オブジェクトの親</param>
        /// <param name="lastPosition">移動先の位置</param>
        /// <param name="warp">現在触れているワープ</param>
        /// <param name="warpA">ワープA</param>
        /// <param name="warpB">ワープB</param>
        /// <returns>当たり判定用プレハブのクローン</returns>
        private GameObject InstanceCheckCube(Transform originParent, Vector3 lastPosition, Transform warp, Transform warpA, Transform warpB)
        {
            try
            {
                if (originParent.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                {
                    var group = Instantiate(checkCbSmallGroupPrefab, lastPosition, Quaternion.identity, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current));
                    foreach (Transform child in originParent)
                    {
                        Instantiate(checkCbSmallPrefab, warp.Equals(warpA) ?
                            GetCalcWarpPosition(child.position, warpA.position, warpB.position) :
                            GetCalcWarpPosition(child.position, warpB.position, warpA.position), Quaternion.identity, group.transform);
                    }
                    if (group.transform.childCount < 1)
                        throw new System.Exception("子オブジェクトがありません");
                    var isCollision = false;
                    foreach (Transform child in group.transform)
                    {
                        isCollision = CheckSpace(child);
                        if (isCollision)
                            // 一度でもTrueが返却された場合は処理を抜ける
                            break;
                    }
                    if (!isCollision)
                    {
                        //Debug.Log("オブジェクトを削除");
                        Destroy(group);

                        // 一度、触れたワープポイントの中心に移動させる
                        originParent.position = warp.Equals(warpA) ? warpA.position : warpB.position;
                        // 一度、移動先のワープポイントの中心へ配置
                        originParent.position = warp.Equals(warpA) ? warpB.position : warpA.position;
                        // 最終配置のポジションへ調整
                        originParent.position = lastPosition;
                    }
                    else
                        foreach (Transform child in group.transform)
                            child.GetComponent<LineRenderer>().enabled = true;

                    return group;
                }
                throw new System.Exception("親オブジェクトがグループではありません");
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 空間のチェック
        /// </summary>
        /// <param name="child">チェック用ブロック</param>
        /// <returns>接触あり／無し</returns>
        public bool CheckSpace(Transform child)
        {
            try
            {
                var isCollision = false;
                foreach (var rayMap in raysMap)
                {
                    if (isCollision)
                        return isCollision;
                    isCollision = IsHitCheckCube(child.position, rayMap.DotPosition[0], rayMap.DotPosition[1], distance, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)) ||
                        IsHitCheckCube(child.position, rayMap.DotPosition[0], rayMap.DotPosition[1], distance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                        IsHitCheckCube(child.position, rayMap.DotPosition[0], rayMap.DotPosition[1], distance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) ||
                        IsHitCheckCube(child.position, rayMap.DotPosition[0], rayMap.DotPosition[1], distance, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES))
                        ;
                }

                return isCollision;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 接地判定
        /// </summary>
        /// <param name="postion">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <returns>レイのヒット判定の有無</returns>
        private bool IsHitCheckCube(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask)
        {
            var hits = new RaycastHit[1];
            if (layerMask < 0)
                return IsGrounded(postion, rayOriginOffset, rayDirection, rayMaxDistance);

            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            var hitCount = Physics.RaycastNonAlloc(ray, hits, rayMaxDistance, layerMask);
            //Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            return hitCount >= 1f;
        }

        /// <summary>
        /// 接地判定
        /// </summary>
        /// <param name="postion">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <returns>レイのヒット判定の有無</returns>
        private bool IsGrounded(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance)
        {
            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            //Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var raycastHits = new RaycastHit[1];
            var hitCount = Physics.RaycastNonAlloc(ray, raycastHits, rayMaxDistance);
            return hitCount >= 1f;
        }

        /// <summary>
        /// オブジェクトスケールが１×１でない場合もある
        /// その際は移動値の計算結果を返す
        /// </summary>
        /// <param name="originPosition">対象オブジェクトの位置</param>
        /// <param name="fromPosition">移動元の位置</param>
        /// <param name="toPosition">移動先の位置</param>
        /// <returns>移動値</returns>
        private Vector3 GetCalcWarpPosition(Vector3 originPosition, Vector3 fromPosition, Vector3 toPosition)
        {
            return originPosition + (toPosition - fromPosition);
        }

        /// <summary>
        /// 対象オブジェクトを疑似的な静的オブジェクトにする
        /// ※物理シミュレーションをオフにしないとワープなどの瞬間移動が行えない為
        /// </summary>
        /// <param name="environment">エンバイロメント</param>
        /// <returns>成功／失敗</returns>
        private bool ChangeFakeStatic(GameObject environment)
        {
            try
            {
                var owner = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>();
                if (environment.CompareTag(TagConst.TAG_NAME_PLAYER))
                {
                    if (!owner.MoveCharactorPlayer(Vector3.zero))
                        throw new System.Exception("プレイヤーの外部制御の失敗");
                    if (!owner.ChangeCharactorControllerStatePlayer(false))
                        throw new System.Exception("プレイヤーのCharactorControllerステータス変更の失敗");
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    if (!owner.MoveCharactorRobotEnemy(Vector3.zero, environment))
                        throw new System.Exception("敵の外部制御の失敗");
                    if (!owner.ChangeCharactorControllerStateRobotEnemy(false, environment))
                        throw new System.Exception("敵のCharactorControllerステータス変更の失敗");
                    if (!owner.ChangeCapsuleColliderStateRobotEnemy(false, environment))
                        throw new System.Exception("敵のカプセルコライダーステータス変更の失敗");
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    if (!owner.MoveRigidBodyMoveCube(Vector3.zero, environment))
                        throw new System.Exception("空間操作の外部制御の失敗");
                    if (!owner.ChangeRigidBodyStateMoveCube(true, environment))
                        throw new System.Exception("空間操作のRigidbodyステータス変更の失敗");
                    if (!owner.ChangeBoxColliderStateMoveCube(false, environment))
                        throw new System.Exception("空間操作のボックスコライダーステータス変更の失敗");
                }
                else
                    throw new System.Exception("非対象オブジェクト");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 対象オブジェクトを動的オブジェクトにする
        /// </summary>
        /// <param name="environment">エンバイロメント</param>
        /// <returns>成功／失敗</returns>
        private bool ChangeDynamic(GameObject environment)
        {
            try
            {
                var owner = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>();
                if (environment.CompareTag(TagConst.TAG_NAME_PLAYER))
                {
                    if (!owner.ChangeCharactorControllerStatePlayer(true))
                        throw new System.Exception("プレイヤーのCharactorControllerステータス変更の失敗");
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    if (!owner.ChangeCharactorControllerStateRobotEnemy(true, environment))
                        throw new System.Exception("敵のCharactorControllerステータス変更の失敗");
                    if (!owner.ChangeCapsuleColliderStateRobotEnemy(true, environment))
                        throw new System.Exception("敵のカプセルコライダーステータス変更の失敗");
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    if (!owner.ChangeRigidBodyStateMoveCube(false, environment))
                        throw new System.Exception("空間操作のRigidbodyステータス変更の失敗");
                    if (!owner.ChangeBoxColliderStateMoveCube(true, environment))
                        throw new System.Exception("空間操作のボックスコライダーステータス変更の失敗");
                }
                else
                    throw new System.Exception("非対象オブジェクト");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
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
                Debug.LogException(e);
                return false;
            }
        }
    }
}
