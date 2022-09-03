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
        ///// <summary>空間操作ブロックの移動先チェックオブジェクトグループ一時格納</summary>
        //private GameObject _checkCbSmallGroup;

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
                // T.B.D 移動方法はひとまずエレベーター式を採用（※タクシー式に戻す可能性もあり、複数のオブジェクトごとにフラグ管理する可能性もある）
                var isMoving = new BoolReactiveProperty();
                // 確認用
                isMoving.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x => Debug.Log(x));
                for (var i = 0; i < parent.childCount; i++)
                {
                    Transform warp = parent.GetChild(i);
                    warp.OnTriggerEnterAsObservable()
                        .Do(_ => Debug.Log("OnTriggerEnter"))
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
                        .Do(_ => Debug.Log("OnTriggerExit"))
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
                Debug.Log($"接触：{environment.name}");
                if (!ChangeFakeStatic(environment))
                    throw new System.Exception("静的変化の失敗");
                if (environment.CompareTag(TagConst.TAG_NAME_PLAYER) || environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    //Debug.Log(environment.transform.position);
                    // 一度触れたワープポイントの中心に移動させる
                    environment.transform.position = warp.Equals(warpA) ? warpA.position : warpB.position;
                    //Debug.Log(environment.transform.position);
                    // 移動先のワープへ配置
                    environment.transform.position = warp.Equals(warpA) ? warpB.position : warpA.position;
                    //Debug.Log(environment.transform.position);
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    // 最終配置のポジションを算出
                    var lastPosition = warp.Equals(warpA) ?
                        GetCalcWarpPosition(environment.transform.parent.position, warpA.position, warpB.position) :
                        GetCalcWarpPosition(environment.transform.parent.position, warpB.position, warpA.position);
                    //var check = new BoolReactiveProperty();
                    _checkObject = InstanceCheckCube(environment.transform.parent, lastPosition, warp, warpA, warpB);
                }
                else
                    throw new System.Exception("非対象オブジェクト");
                if (!ChangeDynamic(environment))
                    throw new System.Exception("動的変化の失敗");
                //Debug.LogError("移動の完了");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

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
                    var result = new List<bool>();
                    foreach (Transform child in group.transform)
                    {
                        var r = child.GetComponent<CheckCbSmall>().CheckSpace();
                        result.Add(r);
                        if (r)
                            // 一度でもTrueが返却された場合は処理を抜ける
                            break;
                    }
                    if (result.Where(q => q)
                        .Select(q => q)
                        .ToArray().Length < 1)
                    {
                        Debug.Log("オブジェクトを削除");
                        Destroy(group);

                        // 一度、触れたワープポイントの中心に移動させる
                        originParent.position = warp.Equals(warpA) ? warpA.position : warpB.position;
                        //Debug.Log(environment.transform.parent.position);
                        // 一度、移動先のワープポイントの中心へ配置
                        originParent.position = warp.Equals(warpA) ? warpB.position : warpA.position;
                        //Debug.Log(environment.transform.parent.position);
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
