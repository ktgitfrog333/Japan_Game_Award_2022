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
using DG.Tweening;

namespace Gimmick
{
    /// <summary>
    /// ワープゲート
    /// ペアによって管理する
    /// ワープAからはワープBへ移動する（逆パターンも含む）
    /// </summary>
    public class WarpGatesPair : MonoBehaviour, IOwner
    {
        /// <summary>ワープ初期の有効／無効ステータス（有効ならtrue、無効ならfalse）</summary>
        [SerializeField] private bool isEnabled = false;
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
        /// <summary>ワープ有効な場合のスケール</summary>
        [SerializeField] private Vector3 enableScale = Vector3.one;
        /// <summary>ワープ無効な場合のスケール</summary>
        [SerializeField] private Vector3 disableScale = Vector3.one * .25f;
        /// <summary>ワープスケール変更アニメーションの時間</summary>
        [SerializeField] private float doScaleDuration = .1f;
        /// <summary>吸引時の回転</summary>
        [SerializeField] private Vector3 suctionRotate = new Vector3(0f, 0f, 360f);
        /// <summary>吸引時のスケール</summary>
        [SerializeField] private Vector3 suctionScale = Vector3.zero;
        /// <summary>吸引時アニメーション時間</summary>
        [SerializeField] private float suctionDuration = .5f;

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
        /// <summary>吸い込みTweenアニメーション</summary>
        private Sequence[] _sequenceSuction = new Sequence[2];
        /// <summary>入力禁止</summary>
        public bool _inputBan;

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool ManualStart()
        {
            try
            {
                _inputBan = false;
                var parent = transform;
                var warpA = parent.GetChild(0);
                var warpB = parent.GetChild(1);
                _isWarp.Value = isEnabled;
                _isWarp.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                        if (x)
                        {
                            // ワープ有効
                            warpA.DOScale(enableScale, doScaleDuration)
                                .SetLink(gameObject);
                            warpB.DOScale(enableScale, doScaleDuration)
                                .SetLink(gameObject);
                        }
                        else
                        {
                            // ワープ無効
                            warpA.DOScale(disableScale, doScaleDuration)
                                .SetLink(gameObject);
                            warpB.DOScale(disableScale, doScaleDuration)
                                .SetLink(gameObject);
                        }
                    })
                    .AddTo(_compositeDisposable);
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
                            onEnteredTransMap == null &&
                            // 操作禁止の状態でない
                            !_inputBan)
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
                            var charactors = new WarpCharactors();
                            charactors.Environment = x;
                            if (!WarpCollisionEnvironment(warp, warpA, warpB, suctionRotate, suctionScale, suctionDuration, charactors))
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
        /// 吸い込まれるTweenアニメーションを実行済み状態でKill
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool KillCompleteTweeenSuction()
        {
            try
            {
                if (_sequenceSuction != null && 0 < _sequenceSuction.Length)
                {
                    for (var i = 0; i < _sequenceSuction.Length; i++)
                    {
                        if (_sequenceSuction[i] != null)
                        {
                            switch (i)
                            {
                                case 0:
                                    _sequenceSuction[i].PlayBackwards();
                                    break;
                                case 1:
                                    _sequenceSuction[i].Kill(true);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
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
        /// <param name="warp">接触ワープ</param>
        /// <param name="warpA">ワープA（比較用）</param>
        /// <param name="warpB">ワープB（比較用）</param>
        /// <param name="suctionRotate">吸引対象オブジェクトのクォータニオン角</param>
        /// <param name="suctionScale">吸引対象オブジェクトのスケール</param>
        /// <param name="suctionDuration">吸引アニメーション時間</param>
        /// <param name="charactors">移動対象キャラクター</param>
        /// <returns>成功／失敗</returns>
        private bool WarpCollisionEnvironment(Transform warp, Transform warpA, Transform warpB, Vector3 suctionRotate, Vector3 suctionScale, float suctionDuration, WarpCharactors charactors)
        {
            try
            {
                // ヒットオブジェクトがある場合はセットする
                if (charactors.Environment.CompareTag(TagConst.TAG_NAME_PLAYER) || charactors.Environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    charactors.HitMoveCube = LevelDesisionIsObjected.IsOnEnemiesAndInfo(charactors.Environment.transform.position, isUnderMoveCube.DotPosition[0], isUnderMoveCube.DotPosition[1], isOnUnderDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE));
                }
                else if (charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    // 距離を延ばす
                    var hitPlayer = GetHitPlayerOrEnemyInMoveCubeGroup(charactors, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER));
                    var hitEnemy = GetHitPlayerOrEnemyInMoveCubeGroup(charactors, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                    charactors.HitPlayerOrEnemy = hitPlayer != null ? hitPlayer : hitEnemy;
                }
                else
                    throw new System.Exception("非対象オブジェクト");
                // 移動オブジェクトを一時的に静的オブジェクトにする
                if (!ChangeFakeStatic(charactors.Environment))
                    throw new System.Exception("静的変化の失敗");
                if (charactors.HitMoveCube != null)
                    if (!ChangeFakeStatic(charactors.HitMoveCube.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? charactors.HitMoveCube.transform.GetChild(0).gameObject : charactors.HitMoveCube))
                        throw new System.Exception("静的変化の失敗");
                if (charactors.HitPlayerOrEnemy != null)
                    if (!ChangeFakeStatic(charactors.HitPlayerOrEnemy))
                        throw new System.Exception("静的変化の失敗");
                // 移動処理
                if (charactors.Environment.CompareTag(TagConst.TAG_NAME_PLAYER) || charactors.Environment.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY))
                {
                    if (charactors.HitMoveCube != null)
                    {
                        if (!WarpCollisionEnvironmentSameTime(warp, warpA, warpB, charactors.HitMoveCube, suctionRotate, suctionScale, suctionDuration, charactors))
                            throw new System.Exception("同時ワープの失敗");
                    }
                    else
                    {
                        var defaultScale = charactors.Environment.transform.localScale;
                        // ワープへ吸い込まれる
                        var isCompetedSuction = new BoolReactiveProperty();
                        _sequenceSuction[0] = PlayAndGetSuction(charactors.Environment.transform, warp.Equals(warpA) ? warpA.position : warpB.position, suctionRotate, suctionScale, suctionDuration, isCompetedSuction);
                        if (_sequenceSuction[0] == null)
                            throw new System.Exception("吸引アニメーション管理の失敗");
                        var isCompletedRelease = new BoolReactiveProperty();
                        isCompetedSuction.ObserveEveryValueChanged(x => x.Value)
                            .Where(x => x && !_inputBan)
                            .Subscribe(_ =>
                            {
                                // 移動先のワープへ配置
                                charactors.Environment.transform.position = warp.Equals(warpA) ? warpB.position : warpA.position;
                                _sequenceSuction[1] = PlayAndGetSuction(charactors.Environment.transform, charactors.Environment.transform.position, suctionRotate, defaultScale, suctionDuration, isCompletedRelease);
                                if (_sequenceSuction[1] == null)
                                    throw new System.Exception("吸引アニメーション管理の失敗");
                            })
                            .AddTo(_compositeDisposable);
                        // 放出後の後処理
                        isCompletedRelease.ObserveEveryValueChanged(x => x.Value)
                            .Where(x => x)
                            .Subscribe(_ =>
                            {
                                if (!AllChangeDynamic(charactors))
                                    throw new System.Exception("いずれかの動的変化の失敗");
                            })
                            .AddTo(_compositeDisposable);
                    }
                }
                else if (charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                {
                    if (charactors.HitPlayerOrEnemy != null)
                    {
                        if (!WarpCollisionEnvironmentSameTime(warp, warpA, warpB, charactors.HitPlayerOrEnemy, suctionRotate, suctionScale, suctionDuration, charactors))
                            throw new System.Exception("同時ワープの失敗");
                    }
                    else
                    {
                        // 最終配置のポジションを算出
                        var lastPosition = warp.Equals(warpA) ?
                            GetCalcWarpPosition(charactors.Environment.transform.localPosition, warpB.position) :
                            GetCalcWarpPosition(charactors.Environment.transform.localPosition, warpA.position);
                        _checkObject = InstanceCheckCube(charactors.Environment.transform.parent, lastPosition, warp, warpA, warpB, suctionRotate, suctionScale, suctionDuration, charactors);
                    }
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
        /// 空間操作ブロックグループ内のブロックの上に乗っているオブジェクトを返却
        /// </summary>
        /// <param name="charactors">移動対象キャラクター</param>
        /// <param name="layerMask">マスク情報</param>
        /// <returns>プレイヤー／敵（どちらでもないならnull）</returns>
        private GameObject GetHitPlayerOrEnemyInMoveCubeGroup(WarpCharactors charactors, int layerMask)
        {
            var hitList = new List<GameObject>();
            foreach (Transform c in charactors.Environment.transform.parent)
            {
                var g = LevelDesisionIsObjected.IsOnEnemiesAndInfoThreePointHorizontal(c.transform.position, isOnPlayerAndEnemyRay.DotPosition[0], isOnPlayerAndEnemyRay.DotPosition[1], isOnUnderDistance, layerMask, .5f);
                if (g != null)
                    hitList.Add(g);
            }
            return 0 < hitList.Count ? hitList[0] : null;
        }

        /// <summary>
        /// 動的変化の一括処理
        /// </summary>
        /// <param name="charactors">移動対象キャラクター</param>
        /// <returns>成功／失敗</returns>
        private bool AllChangeDynamic(WarpCharactors charactors)
        {
            try
            {
                return AllChangeDynamic(charactors, null);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 動的変化の一括処理
        /// </summary>
        /// <param name="charactors">移動対象キャラクター</param>
        /// <param name="isCompletedCheck">移動可否チェック完了フラグ</param>
        /// <returns>成功／失敗</returns>
        private bool AllChangeDynamic(WarpCharactors charactors, BoolReactiveProperty isCompletedCheck)
        {
            try
            {
                // 移動オブジェクトを元に戻す
                if (!ChangeDynamic(charactors.Environment))
                    throw new System.Exception("動的変化の失敗");
                if (charactors.HitMoveCube != null)
                    if (!ChangeDynamic(charactors.HitMoveCube.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? charactors.HitMoveCube.transform.GetChild(0).gameObject : charactors.HitMoveCube))
                        throw new System.Exception("動的変化の失敗");
                if (charactors.HitPlayerOrEnemy != null)
                    if (!ChangeDynamic(charactors.HitPlayerOrEnemy))
                        throw new System.Exception("動的変化の失敗");
                if (isCompletedCheck != null)
                    isCompletedCheck.Value = true;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 吸引／放出アニメーションを管理
        /// ・fromワープへの移動アニメーション
        /// ・回転アニメーション
        /// ・縮小アニメーション
        /// </summary>
        /// <param name="environment">エンバイロメント</param>
        /// <param name="endPosition">移動先の座標</param>
        /// <param name="endRotate">回転する角度</param>
        /// <param name="endScale">変化スケール</param>
        /// <param name="duration">アニメーション時間</param>
        /// <param name="isCompeted">アニメーション終了のフラグ</param>
        /// <returns>Tween情報</returns>
        private Sequence PlayAndGetSuction(Transform environment, Vector3 endPosition, Vector3 endRotate, Vector3 endScale, float duration, BoolReactiveProperty isCompeted)
        {
            try
            {
                var seq = DOTween.Sequence();
                // 一度触れたワープポイントの中心に移動させる
                seq.Append(environment.transform.DOMove(endPosition, duration))
                    .Join(environment.transform.DORotate(endRotate, duration, RotateMode.WorldAxisAdd))
                    .Join(environment.transform.DOScale(endScale, duration))
                    .OnComplete(() => isCompeted.Value = true)
                    .SetLink(gameObject);

                return seq;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// 衝突した対象オブジェクトをワープさせる処理からの呼び出し
        /// 同時ワープを実現する
        /// </summary>
        /// <param name="warp">接触ワープ</param>
        /// <param name="warpA">ワープA（比較用）</param>
        /// <param name="warpB">ワープB（比較用）</param>
        /// <param name="hitObject">ヒットしたオブジェクト</param>
        /// <param name="suctionRotate">吸引対象オブジェクトのクォータニオン角</param>
        /// <param name="suctionScale">吸引対象オブジェクトのスケール</param>
        /// <param name="suctionDuration">吸引アニメーション時間</param>
        /// <param name="charactors">移動対象キャラクター</param>
        /// <returns>成功／失敗</returns>
        private bool WarpCollisionEnvironmentSameTime(Transform warp, Transform warpA, Transform warpB, GameObject hitObject, Vector3 suctionRotate, Vector3 suctionScale, float suctionDuration, WarpCharactors charactors)
        {
            try
            {
                // 一つにまとめるオブジェクトを仮生成
                var obj = GameObject.Find(MOVE_PARENT_NAME) != null ? GameObject.Find(MOVE_PARENT_NAME) : new GameObject(MOVE_PARENT_NAME);
                var stage = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current).gameObject;
                obj.transform.position = charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? charactors.Environment.transform.parent.position : charactors.Environment.transform.position;
                obj.transform.parent = stage.transform;
                // 親を一時格納
                charactors.DefaultParent = charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? charactors.Environment.transform.parent.parent : charactors.Environment.transform.parent;
                charactors.DefaultHitParent = hitObject.transform.parent;
                // 一時的なグループ化
                if (charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                    charactors.Environment.transform.parent.parent = obj.transform;
                else
                    charactors.Environment.transform.parent = obj.transform;
                hitObject.transform.parent = obj.transform;

                // 最終配置のポジションを算出
                var lastPosition = warp.Equals(warpA) ?
                    GetCalcWarpPosition(charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? charactors.Environment.transform.localPosition : Vector3.zero, warpB.position) :
                    GetCalcWarpPosition(charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE) ? charactors.Environment.transform.localPosition : Vector3.zero, warpA.position);
                // 移動チェックブロック生成、移動処理
                var isCompletedCheck = new BoolReactiveProperty();
                _checkObject = InstanceCheckCube(obj.transform, lastPosition, warp, warpA, warpB, hitObject, suctionRotate, suctionScale, suctionDuration, charactors, isCompletedCheck);
                isCompletedCheck.ObserveEveryValueChanged(x => x.Value)
                    .Where(x => x)
                    .Subscribe(_ =>
                    {
                        if (!ResetGroup(charactors, hitObject))
                            throw new System.Exception("グループ化解除の失敗");
                    })
                    .AddTo(_compositeDisposable);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// グループ化した親関係を元に戻す
        /// ※EnvironmentとHitオブジェクトを同時に移動させる事象で使用される
        /// </summary>
        /// <param name="charactors">移動対象キャラクター</param>
        /// <param name="hitObject">ヒットしたオブジェクト</param>
        /// <returns>成功／失敗</returns>
        private bool ResetGroup(WarpCharactors charactors, GameObject hitObject)
        {
            try
            {
                // グループを元に戻す
                if (charactors.Environment.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                    charactors.Environment.transform.parent.parent = charactors.DefaultParent;
                else
                    charactors.Environment.transform.parent = charactors.DefaultParent;
                hitObject.transform.parent = charactors.DefaultHitParent;

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
        private GameObject InstanceCheckCube(Transform originParent, Vector3 lastPosition, Transform warp, Transform warpA, Transform warpB, Vector3 suctionRotate, Vector3 suctionScale, float suctionDuration, WarpCharactors charactors)
        {
            return InstanceCheckCube(originParent, lastPosition, warp, warpA, warpB, null, suctionRotate, suctionScale, suctionDuration, charactors, null);
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
        private GameObject InstanceCheckCube(Transform originParent, Vector3 lastPosition, Transform warp, Transform warpA, Transform warpB, GameObject onOrUnderObject, Vector3 suctionRotate, Vector3 suctionScale, float suctionDuration, WarpCharactors charactors, BoolReactiveProperty isCompletedCheck)
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
                            Instantiate(checkCbSmallPrefab, GetMinDistance(moveCubeGroup, onOrUnderObject).Target.localPosition + lastPosition + Vector3.up, Quaternion.identity, group.transform);
                        }
                        else if (onOrUnderObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                        {
                            // 空間操作ブロックはプレイヤーの下にいるのでプレイヤー位置にチェックブロックを追加
                            Instantiate(checkCbSmallPrefab, lastPosition + Vector3.up * 2, Quaternion.identity, group.transform);
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

                        var defaultScale = originParent.localScale;
                        // ワープへ吸い込まれる
                        var isCompetedSuction = new BoolReactiveProperty();
                        // 一度、触れたワープポイントの中心に移動させる
                        _sequenceSuction[0] = PlayAndGetSuction(originParent, warp.Equals(warpA) ? warpA.position : warpB.position, suctionRotate, suctionScale, suctionDuration, isCompetedSuction);
                        if (_sequenceSuction[0] == null)
                            throw new System.Exception("吸引アニメーション管理の失敗");
                        var isCompletedRelease = new BoolReactiveProperty();
                        isCompetedSuction.ObserveEveryValueChanged(x => x.Value)
                            .Where(x => x && !_inputBan)
                            .Subscribe(_ =>
                            {
                                // 一度、移動先のワープポイントの中心へ配置
                                originParent.position = warp.Equals(warpA) ? warpB.position : warpA.position;
                                // 最終配置のポジションへ調整
                                _sequenceSuction[1] = PlayAndGetSuction(originParent, lastPosition, suctionRotate, defaultScale, suctionDuration, isCompletedRelease);
                                if (_sequenceSuction[1] == null)
                                    throw new System.Exception("吸引アニメーション管理の失敗");
                            })
                            .AddTo(_compositeDisposable);
                        // 放出後の後処理
                        isCompletedRelease.ObserveEveryValueChanged(x => x.Value)
                            .Where(x => x)
                            .Subscribe(_ =>
                            {
                                if (!AllChangeDynamic(charactors, isCompletedCheck))
                                    throw new System.Exception("いずれかの動的変化の失敗");
                            })
                            .AddTo(_compositeDisposable);
                    }
                    else
                    {
                        if (originParent.name.Equals(MOVE_PARENT_NAME))
                            if (!ResetGroup(charactors, onOrUnderObject))
                                throw new System.Exception("グループ化解除の失敗");
                        foreach (Transform child in group.transform)
                            child.GetComponent<LineRenderer>().enabled = true;
                        if (!AllChangeDynamic(charactors))
                            throw new System.Exception("いずれかの動的変化の失敗");
                    }

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
        /// プレイヤー／敵と空間操作ブロックを比較
        /// 最短距離の組み合わせを返却
        /// </summary>
        /// <param name="moveCubeGroup">空間操作ブロックグループ</param>
        /// <param name="onOrUnderObject">上または下に存在するオブジェクト</param>
        /// <returns>オブジェクト距離情報</returns>
        private PairDistance GetMinDistance(Transform moveCubeGroup, GameObject onOrUnderObject)
        {
            try
            {
                var pairDistanceList = new List<PairDistance>();
                foreach (Transform child in moveCubeGroup)
                {
                    var pair = new PairDistance();
                    var d = Vector3.Distance(child.position, onOrUnderObject.transform.position);
                    pair.Target = child;
                    pair.Distance = d;
                    pairDistanceList.Add(pair);
                }
                IOrderedEnumerable<PairDistance> pairSortList = pairDistanceList.OrderBy(rec => rec.Distance)
                    .ThenBy(rec => rec.Target.name);
                foreach (var p in pairSortList)
                    return p;

                throw new System.Exception("ペアが存在しません");
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new PairDistance();
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
        /// <returns>移動座標</returns>
        private Vector3 GetCalcWarpPosition(Vector3 localPosition, Vector3 toPosition)
        {
            return toPosition + new Vector3(localPosition.x * -1f, localPosition.y * -1f, 0f);
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
                    owner.SetPlayerControllerInputBan(true);
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
                    owner.SetPlayerControllerInputBan(false);
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
                if (_checkObject != null)
                    Destroy(_checkObject);
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

        /// <summary>
        /// WarpGatesPairの操作禁止フラグをセット
        /// </summary>
        /// <param name="flag">有効／無効</param>
        /// <returns></returns>
        public bool SetWarpGatesPairInputBan(bool flag)
        {
            try
            {
                _inputBan = true;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 移動対象となるキャラクター
        /// </summary>
        public struct WarpCharactors
        {
            /// <summary>
            /// プレイヤー／敵／空間操作ブロック
            /// </summary>
            public GameObject Environment { get; set; }
            /// <summary>
            /// ヒットしたオブジェクト格納用
            /// 空間操作ブロック（グループ）
            /// </summary>
            public GameObject HitMoveCube { get; set; }
            /// <summary>
            /// ヒットしたオブジェクト格納用
            /// プレイヤー／敵
            /// </summary>
            public GameObject HitPlayerOrEnemy { get; set; }
            /// <summary>
            /// デフォルトの親
            /// </summary>
            public Transform DefaultParent { get; set; }
            /// <summary>
            /// ヒットオブジェクトのデフォルトの親
            /// </summary>
            public Transform DefaultHitParent { get; set; }
        }

        /// <summary>
        /// オブジェクトの距離
        /// </summary>
        public struct PairDistance
        {
            /// <summary>対象オブジェクト</summary>
            public Transform Target { get; set; }
            /// <summary>距離</summary>
            public float Distance { get; set; }
        }
    }
}
