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
        /// <summary>プレイヤーと敵のレイ</summary>
        [SerializeField] private CubeRaysMap isUnderMoveCube;
        /// <summary>プレイヤーのレイの距離</summary>
        [SerializeField] private float isOnUnderDistance;
        /// <summary>プレイヤーと敵のレイ</summary>
        [SerializeField] private CubeRaysMap isOnPlayerAndEnemyRay;
        /// <summary>移動用のオブジェクト名</summary>
        private readonly string MOVE_PARENT_NAME = "MoveParentName";
        /// <summary>空間操作ブロック１ブロック分の当たり判定</summary>
        [SerializeField] private Vector3 isExitedMoveCube;
        /// <summary>空間操作ブロック１ブロック分の当たり判定</summary>
        [SerializeField] private Vector3 isHitedMovingSpace;

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
        /// <summary>ワープ可否状態</summary>
        private BoolReactiveProperty _isWarp = new BoolReactiveProperty();

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool ManualStart()
        {
            try
            {
                _isWarp.Value = false;
                _isWarp.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                        if (x)
                        {
                            // T.B.D ワープ有効の処理を実装
                            Debug.Log("ワープ有効");
                        }
                        else
                        {
                            // T.B.D ワープ無効の処理を実装
                            Debug.Log("ワープ無効");
                        }
                    })
                    .AddTo(_compositeDisposable);
                var parent = transform;
                var warpA = parent.GetChild(0);
                var warpB = parent.GetChild(1);
                // トリガーに触れたオブジェクトを管理
                Dictionary<Transform, bool> onEnteredTransMap = null;
                for (var i = 0; i < parent.childCount; i++)
                {
                    Transform warp = parent.GetChild(i);
                    warp.OnTriggerEnterAsObservable()
                        .Where(x => _isWarp.Value &&
                            // 移動対象に含まれる
                            0 < collierTagNames.Where(q => x.CompareTag(q)).Select(q => q).ToArray().Length &&
                            // トリガーに触れたオブジェクトがnull
                            onEnteredTransMap == null)
                        .Select(x => x.gameObject)
                        .Subscribe(x =>
                        {
                            onEnteredTransMap = new Dictionary<Transform, bool>();
                            if (x.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                                // 空間操作ブロックの場合は全ての情報を格納
                                foreach (Transform c in x.transform.parent)
                                    onEnteredTransMap[c] = true;
                            else
                                onEnteredTransMap[x.transform] = true;
                            if (!WarpCollisionEnvironment(x, warp, warpA, warpB))
                                throw new System.Exception("衝突エンバイロメント制御の失敗");
                        })
                        .AddTo(_compositeDisposable);
                    warp.OnTriggerExitAsObservable()
                        .Where(x => 0 < collierTagNames.Where(q => x.CompareTag(q)).Select(q => q).ToArray().Length &&
                            onEnteredTransMap != null &&
                            onEnteredTransMap.ContainsKey(x.transform))
                        .Subscribe(x =>
                        {
                            // ワープ接触状態の更新
                            if (x.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                                // ボックスのレイ
                                foreach (Transform c in x.transform.parent)
                                    onEnteredTransMap[c.transform] = IsHitBoxCast(c, isExitedMoveCube, LayerMask.GetMask(LayerConst.LAYER_NAME_WARPGATE));
                            else
                                onEnteredTransMap[x.transform] = false;
                            // 関連する全てのオブジェクトが非接触状態なら解放する
                            if (onEnteredTransMap.Count == onEnteredTransMap.Where(q => !q.Value).ToArray().Length)
                            {
                                onEnteredTransMap = null;
                                if (_checkObject != null)
                                    Destroy(_checkObject);
                            }
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
        /// ボックス型のレイ判定
        /// </summary>
        /// <param name="origin">対象オブジェクトの中心</param>
        /// <param name="halfExtents">ボックスレイ（半分のスケール）</param>
        /// <param name="layerMask">レイヤーマスク</param>
        /// <returns>ヒット結果</returns>
        private bool IsHitBoxCast(Transform origin, Vector3 halfExtents, int layerMask)
        {
            try
            {
                return Physics.CheckBox(origin.position, halfExtents, origin.rotation, layerMask);

            }
            catch (System.Exception e)
            {
                throw e;
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
        private bool WarpCollisionEnvironment(GameObject environment, Transform warp, Transform warpA, Transform warpB/*, Vector3 closestPoint*/)
        {
            try
            {
                // ヒットオブジェクトがある場合はセットする
                GameObject hitMoveCube = null;
                GameObject hitPlayerOrEnemy = null;
                if (environment.CompareTag(TagConst.TAG_NAME_PLAYER) || environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    hitMoveCube = LevelDesisionIsObjected.IsOnEnemiesAndInfo(environment.transform.position, isUnderMoveCube.DotPosition[0], isUnderMoveCube.DotPosition[1], isOnUnderDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE));
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    // 距離を延ばす
                    var addDistance = environment.transform.parent.childCount;
                    var hitPlayer = LevelDesisionIsObjected.IsOnEnemiesAndInfo(environment.transform.position, isOnPlayerAndEnemyRay.DotPosition[0], isOnPlayerAndEnemyRay.DotPosition[1], isOnUnderDistance * environment.transform.parent.childCount, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER));
                    var hitEnemy = LevelDesisionIsObjected.IsOnEnemiesAndInfo(environment.transform.position, isOnPlayerAndEnemyRay.DotPosition[0], isOnPlayerAndEnemyRay.DotPosition[1], isOnUnderDistance * addDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                    hitPlayerOrEnemy = hitPlayer != null ? hitPlayer : hitEnemy;
                }
                else
                    throw new System.Exception("非対象オブジェクト");
                // 移動オブジェクトを一時的に静的オブジェクトにする
                if (!ChangeFakeStatic(environment))
                    throw new System.Exception("静的変化の失敗");
                if (hitMoveCube != null)
                    if (!ChangeFakeStatic(hitMoveCube.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? hitMoveCube.transform.GetChild(0).gameObject : hitMoveCube))
                        throw new System.Exception("静的変化の失敗");
                if (hitPlayerOrEnemy != null)
                    if (!ChangeFakeStatic(hitPlayerOrEnemy))
                        throw new System.Exception("静的変化の失敗");
                // 移動処理
                if (environment.CompareTag(TagConst.TAG_NAME_PLAYER) || environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    if (hitMoveCube != null)
                    {
                        if (!WarpCollisionEnvironmentSameTime(environment, warp, warpA, warpB, hitMoveCube))
                            throw new System.Exception("同時ワープの失敗");
                    }
                    else
                    {
                        // 一度触れたワープポイントの中心に移動させる
                        environment.transform.position = warp.Equals(warpA) ? warpA.position : warpB.position;
                        // 移動先のワープへ配置
                        environment.transform.position = warp.Equals(warpA) ? warpB.position : warpA.position;
                    }
                }
                else if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    if (hitPlayerOrEnemy != null)
                    {
                        if (!WarpCollisionEnvironmentSameTime(environment, warp, warpA, warpB, hitPlayerOrEnemy))
                            throw new System.Exception("同時ワープの失敗");
                    }
                    else
                    {
                        // 最終配置のポジションを算出
                        var lastPosition = warp.Equals(warpA) ?
                            GetCalcWarpPosition(environment.transform.localPosition, warpB.position, true) :
                            GetCalcWarpPosition(environment.transform.localPosition, warpA.position, true);
                        _checkObject = InstanceCheckCube(environment.transform.parent, lastPosition, warp, warpA, warpB);
                    }
                }
                // 移動オブジェクトを元に戻す
                if (!ChangeDynamic(environment))
                    throw new System.Exception("動的変化の失敗");
                if (hitMoveCube != null)
                    if (!ChangeDynamic(hitMoveCube.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? hitMoveCube.transform.GetChild(0).gameObject : hitMoveCube))
                        throw new System.Exception("静的変化の失敗");
                if (hitPlayerOrEnemy != null)
                    if (!ChangeDynamic(hitPlayerOrEnemy))
                        throw new System.Exception("静的変化の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 衝突した対象オブジェクトをワープさせる処理からの呼び出し
        /// 同時ワープを実現する
        /// </summary>
        /// <param name="environment">エンバイロメント</param>
        /// <param name="warp">接触ワープ</param>
        /// <param name="warpA">ワープA（比較用）</param>
        /// <param name="warpB">ワープB（比較用）</param>
        /// <param name="hitObject">ヒットしたオブジェクト</param>
        /// <returns></returns>
        private bool WarpCollisionEnvironmentSameTime(GameObject environment, Transform warp, Transform warpA, Transform warpB, GameObject hitObject)
        {
            try
            {
                // 一つにまとめるオブジェクトを仮生成
                var obj = GameObject.Find(MOVE_PARENT_NAME) != null ? GameObject.Find(MOVE_PARENT_NAME) : new GameObject(MOVE_PARENT_NAME);
                var stage = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current).gameObject;
                obj.transform.position = environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? environment.transform.parent.position : environment.transform.position/*environment.transform.position + (environment.transform.position - hitObject.transform.position)*/;
                obj.transform.parent = stage.transform;
                // 親を一時格納
                var defParent = environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? environment.transform.parent.parent : environment.transform.parent;
                var defOnParent = hitObject.transform.parent;
                // 一時的なグループ化
                if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                    environment.transform.parent.parent = obj.transform;
                else
                    environment.transform.parent = obj.transform;
                hitObject.transform.parent = obj.transform;

                // 最終配置のポジションを算出
                var lastPosition = warp.Equals(warpA) ?
                    GetCalcWarpPosition(environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? environment.transform.localPosition : Vector3.zero, /*obj.transform.position,*/ warpB.position, true) :
                    GetCalcWarpPosition(environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? environment.transform.localPosition : Vector3.zero, /*obj.transform.position,*/ warpA.position, true);
                _checkObject = InstanceCheckCube(obj.transform, lastPosition, warp, warpA, warpB, hitObject);

                // グループを元に戻す
                if (environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                    environment.transform.parent.parent = defParent;
                else
                    environment.transform.parent = defParent;
                hitObject.transform.parent = defOnParent;

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
            return InstanceCheckCube(originParent, lastPosition, warp, warpA, warpB, null);
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
        /// <param name="onOrUnderObject">上または下に存在するオブジェクト</param>
        /// <returns>当たり判定用プレハブのクローン</returns>
        private GameObject InstanceCheckCube(Transform originParent, Vector3 lastPosition, Transform warp, Transform warpA, Transform warpB, GameObject onOrUnderObject)
        {
            try
            {
                var moveCubeGroup = originParent.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? originParent : GetFindChildGameObjectWithTag(originParent, TagConst.TAG_NAME_MOVECUBEGROUP);
                if (moveCubeGroup.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                {
                    var group = Instantiate(checkCbSmallGroupPrefab, lastPosition, Quaternion.identity, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current));
                    foreach (Transform child in moveCubeGroup)
                    {
                        Instantiate(checkCbSmallPrefab, lastPosition + child.localPosition, Quaternion.identity, group.transform);
                    }
                    if (group.transform.childCount < 1)
                        throw new System.Exception("子オブジェクトがありません");
                    if (onOrUnderObject != null)
                    {
                        if (onOrUnderObject.CompareTag(TagConst.TAG_NAME_PLAYER) || onOrUnderObject.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                        {
                            // プレイヤーは空間操作ブロック上にいるのでプレイヤー位置にチェックブロックを追加
                            Instantiate(checkCbSmallPrefab, group.transform.GetChild(0).position + Vector3.up, Quaternion.identity, group.transform);
                        }
                        else if (onOrUnderObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                        {
                            // 空間操作ブロックはプレイヤーの下にいるのでプレイヤー位置にチェックブロックを追加
                            Instantiate(checkCbSmallPrefab, group.transform.GetChild(0).position + Vector3.up, Quaternion.identity, group.transform);
                            group.transform.position += Vector3.down;
                        }
                        else
                            throw new System.Exception("非対象オブジェクト");
                    }
                    var isCollision = false;
                    foreach (Transform child in group.transform)
                    {
                        isCollision = IsHitBoxCast(child, isHitedMovingSpace, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)) ||
                            IsHitBoxCast(child, isHitedMovingSpace, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                            IsHitBoxCast(child, isHitedMovingSpace, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) ||
                            IsHitBoxCast(child, isHitedMovingSpace, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES))
                        ;
                        if (isCollision)
                            // 一度でもTrueが返却された場合は処理を抜ける
                            break;
                    }
                    if (!isCollision)
                    {
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
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// 親オブジェクト内にある子オブジェクトを探す
        /// タグ名から一つのオブジェクトを取得
        /// </summary>
        /// <param name="parent">子を持つオブジェクト</param>
        /// <param name="tagName">タグ名</param>
        /// <returns>タグ名にヒットする子オブジェクト</returns>
        private Transform GetFindChildGameObjectWithTag(Transform parent, string tagName)
        {
            var childList = new List<Transform>();
            foreach (Transform child in parent)
                childList.Add(child);
            if (1 < childList.Where(q => q.CompareTag(tagName)).Select(q => q).ToArray().Length)
                throw new System.Exception("同じタグ名のオブジェクトが複数存在します");
            return childList.Where(q => q.CompareTag(tagName)).Select(q => q).ToArray()[0];
        }

        /// <summary>
        /// オブジェクトスケールが１×１でない場合もある
        /// その際は移動値の計算結果を返す
        /// 移動先の座標へローカル座標を考慮した補正を加える
        /// また、空間操作ブロック
        /// </summary>
        /// <param name="localPosition">対象オブジェクトのローカル座標</param>
        /// <param name="toPosition">移動先のワールド座標</param>
        /// <param name="isReversed">座標反転（デフォルトは無効）</param>
        /// <returns>移動座標</returns>
        private Vector3 GetCalcWarpPosition(Vector3 localPosition, Vector3 toPosition, bool isReversed)
        {
            return toPosition + new Vector3(0f, localPosition.y * (isReversed ? -1f : 1f), 0f);
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

        /// <summary>
        /// ワープ可否のステータスを切り替える
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool SwitchWarpState()
        {
            try
            {
                if (_isWarp != null)
                    _isWarp.Value = !_isWarp.Value;
                else
                    throw new System.Exception("ワープ可否フラグnull");

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
