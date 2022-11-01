using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;
using Main.Common.LevelDesign;
using Main.Common;
using Main.UI;
using System.Threading.Tasks;
using Main.Audio;
using System.Linq;
using Main.InputSystem;
using DG.Tweening;

namespace Main.Level
{
    /// <summary>
    /// 空間制御する
    /// </summary>
    public class SpaceOwner : MonoBehaviour
    {
        /// <summary>移動速度（初動フェーズ）</summary>
        [SerializeField] private float moveLSpeed = .3f;
        /// <summary>移動速度（移動フェーズ）</summary>
        [SerializeField] private float moveHSpeed = 3.5f;
        /// <summary>長押ししてから移動フェーズへ以降するまでの時間</summary>
        [SerializeField] private float gearChgDelaySec = 2f;
        /// <summary>動くキューブをグループ化するプレハブ</summary>
        [SerializeField] private GameObject moveCubeGroupPrefab;
        /// <summary>空間操作ブロックのプレハブ</summary>
        [SerializeField] private GameObject moveCubePrefab;
        /// <summary>接続演出のプレハブ（リング）</summary>
        [SerializeField] private GameObject[] connectedRingPrefabs;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, -.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirection = Vector3.up;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistance = .9f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffsetLeft = new Vector3(.1f, 0f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirectionLeft = Vector3.left;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistanceLeft = .8f;
        /// <summary>接地判定用のレイ　当たり判定の最大距離（プレス用）</summary>
        [SerializeField] private float rayMaxDistanceLeftLong = 1.6f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffsetRight = new Vector3(-.1f, 0f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirectionRight = Vector3.right;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        [SerializeField] private float rayMaxDistanceRight = .8f;
        /// <summary>接地判定用のレイ　当たり判定の最大距離（プレス用）</summary>
        [SerializeField] private float rayMaxDistanceRightLong = 1.6f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffsetUp = new Vector3(0f, -.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirectionUp = Vector3.up;
        /// <summary>接地判定用のレイ　当たり判定の最大距離（プレス用）</summary>
        [SerializeField] private float rayMaxDistanceUpLong = 1.6f;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        [SerializeField] private Vector3 rayOriginOffsetDown = new Vector3(0f, .1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        [SerializeField] private Vector3 rayDirectionDown = Vector3.down;
        /// <summary>接地判定用のレイ　当たり判定の最大距離（プレス用）</summary>
        [SerializeField] private float rayMaxDistanceDownLong = 1.6f;
        /// <summary>Axis入力の斜めの甘さ（高い程メリハリのある入力が必要）</summary>
        [SerializeField, Range(0f, 1f)] private float deadMagnitude = 0.9f;
        /// <summary>フレームチェック遅延時間（ミリ秒）</summary>
        [SerializeField] private int checkDelayTimeMs = 10;
        /// <summary>MoveCubeの初期状態</summary>
        private ObjectsOffset[] _cubeOffsets;
        /// <summary>MoveCubeの初期状態</summary>
        public ObjectsOffset[] CubeOffsets => _cubeOffsets;
        /// <summary>空間操作に必要なRigidBody、Velocity</summary>
        private SpaceDirection2D _spaceDirections = new SpaceDirection2D();
        /// <summary>空間操作に必要なRigidBody、Velocity</summary>
        public SpaceDirection2D SpaceDirections => _spaceDirections;
        /// <summary>ブロックの接続状況</summary>
        private List<ConnectDirection2D> _connectDirections = new List<ConnectDirection2D>();
        /// <summary>プールのグループ</summary>
        private PoolGroup _poolGroup;
        /// <summary>MoveCubes</summary>
        private GameObject[] _moveCubes;
        /// <summary>入力禁止</summary>
        public bool InputBan
        {
            set => _inputBan = value;
            get => _inputBan;
        }
        /// <summary>入力禁止</summary>
        private bool _inputBan = false;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        /// <summary>コネクト回数のチェック</summary>
        private IntReactiveProperty _connectSuccess = new IntReactiveProperty();

        /// <summary>
        /// 疑似スタート
        /// </summary>
        public bool Initialize()
        {
            try
            {
                _connectDirections = new List<ConnectDirection2D>();
                _spaceDirections = new SpaceDirection2D();
                // ブロックの接続状況
                _moveCubes = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBE);
                _moveCubes = _moveCubes.Where(q => !q.GetComponent<MoveCbSmall>().Instanced).Select(q => q).ToArray();
                if (_moveCubes.Length == 0)
                    Debug.LogError("MobeCubeの取得失敗");
                // 既にグループ化されている場合は初期値を更新しない
                var group = _moveCubes.Where(x => x.transform.parent.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                    .Select(x => x);
                if (group.ToList().Count == 0)
                    _cubeOffsets = LevelDesisionIsObjected.SaveObjectOffset(_moveCubes);
                if (_cubeOffsets == null)
                    Debug.LogError("オブジェクト初期状態の保存の失敗");
                _connectSuccess.Value = 0;
                // コネクト回数のチェック
                _connectSuccess.ObserveEveryValueChanged(x => x.Value)
                    .Do(x =>
                    {
                        if (!_inputBan && 0 < x && !GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().UpdateCountDown(x, GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().ClearConnectedCounter))
                            Debug.LogError("カウントダウン更新処理の失敗");
                    })
                    .Where(x => GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().ClearConnectedCounter == x)
                    .Subscribe(_ =>
                    {
                        if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().OpenDoor())
                            Debug.LogError("ゴール演出の失敗");
                    })
                    .AddTo(_compositeDisposable);
                // 特定の空間操作ブロックはデフォルトで接続状態にする
                _connectDirections.ObserveEveryValueChanged(x => x.Count)
                    .Where(x => x < 1 && _moveCubes != null && 0 < _moveCubes.Length)
                    .Subscribe(x => AsyncCheckConnectDirection2DListDelayTime(_connectDirections, _moveCubes))
                    .AddTo(_compositeDisposable);
                _moveCubes = SetCollsion(_moveCubes, _connectSuccess);
                if (_moveCubes == null)
                    throw new System.Exception("オブジェクト取得の失敗");
                // 速度の初期値
                _spaceDirections.MoveSpeed = moveLSpeed;
                // 移動入力をチェック
                var velocitySeted = new BoolReactiveProperty();
                this.UpdateAsObservable()
                    .Where(_ => !_inputBan)
                    .Subscribe(_ => velocitySeted.Value = SetMoveVelocotyLeftAndRight())
                    .AddTo(_compositeDisposable);
                velocitySeted.Where(x => x)
                    .Throttle(System.TimeSpan.FromSeconds(gearChgDelaySec))
                    .Where(_ => velocitySeted.Value)
                    .Subscribe(_ =>
                    {
                        _spaceDirections.MoveSpeed = moveHSpeed;
                        GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SetPlayerControllerInputBan(true);
                    })
                    .AddTo(_compositeDisposable);
                velocitySeted.Where(x => !x)
                    .Subscribe(_ =>
                    {
                        _spaceDirections.MoveSpeed = moveLSpeed;
                        GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SetPlayerControllerInputBan(false);
                    })
                    .AddTo(_compositeDisposable);

                // 空間内のブロック座標をチェック
                this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        if (!CheckPositionAndSetMoveCubesComponents(_moveCubes))
                            Debug.LogError("制御対象RigidBody格納の失敗");
                        if (!PlayEffectMove(_spaceDirections.RbsRightSpace))
                            Debug.LogError("移動エフェクト制御の失敗");
                        if (!PlayEffectMove(_spaceDirections.RbsLeftSpace))
                            Debug.LogError("移動エフェクト制御の失敗");
                    })
                    .AddTo(_compositeDisposable);
                // 不要なグループ削除
                var moveCubeGroups = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBEGROUP);
                foreach (var obj in moveCubeGroups)
                    obj.ObserveEveryValueChanged(x => x.transform.childCount < 1)
                        .Where(x => x)
                        .Subscribe(_ => Destroy(obj));
                // SEの制御（左空間）
                var leftMoveSE = false;
                var leftVelocityMagnitude = new FloatReactiveProperty();
                leftVelocityMagnitude.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                    // 0より大きくなった時に一度だけ
                    if (!leftMoveSE && CheckMovementLeftOrRightSpace(_inputBan, x, _spaceDirections.RbsLeftSpace))
                        {
                            leftMoveSE = true;
                            GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(ClipToPlay.se_block_float);
                        }
                        else if (0 == x && leftMoveSE)
                            leftMoveSE = false;
                    })
                    .AddTo(_compositeDisposable);
                // SEの制御（右空間）
                var rightMoveSE = false;
                var rightVelocityMagnitude = new FloatReactiveProperty();
                rightVelocityMagnitude.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                    // 0より大きくなった時に一度だけ
                    if (!rightMoveSE && CheckMovementLeftOrRightSpace(_inputBan, x, _spaceDirections.RbsRightSpace))
                        {
                            rightMoveSE = true;
                            GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(ClipToPlay.se_block_float);
                        }
                        else if (0 == x && rightMoveSE)
                            rightMoveSE = false;
                    })
                    .AddTo(_compositeDisposable);

                // 1フレーム前のVelocity値が同じなら移動制御を行わない（Vector3.zeroを繰り返さない）
                var beforeSpaceDirections = new SpaceDirection2D();
                // 左空間の制御
                this.FixedUpdateAsObservable()
                    .Do(_ => leftVelocityMagnitude.Value = _spaceDirections.MoveVelocityLeftSpace.magnitude)
                    .Subscribe(_ =>
                    {
                        if (0f < beforeSpaceDirections.MoveVelocityLeftSpace.magnitude)
                        {
                            if (!CheckMovementLeftOrRightSpace(_inputBan, _spaceDirections.MoveVelocityLeftSpace.magnitude, _spaceDirections.RbsLeftSpace))
                                _spaceDirections.MoveVelocityLeftSpace = Vector3.zero;
                            if (_spaceDirections.RbsLeftSpace != null && !MoveMoveCube(_spaceDirections.RbsLeftSpace, _spaceDirections.MoveVelocityLeftSpace, _spaceDirections.MoveSpeed))
                                Debug.LogWarning("左空間：MoveCubeの制御を破棄");
                        }
                        beforeSpaceDirections.MoveVelocityLeftSpace = _spaceDirections.MoveVelocityLeftSpace;
                    })
                    .AddTo(_compositeDisposable);
                // 右空間の制御
                this.FixedUpdateAsObservable()
                    .Do(_ => rightVelocityMagnitude.Value = _spaceDirections.MoveVelocityRightSpace.magnitude)
                    .Subscribe(_ =>
                    {
                        if (0f < beforeSpaceDirections.MoveVelocityRightSpace.magnitude)
                        {
                            if (!CheckMovementLeftOrRightSpace(_inputBan, _spaceDirections.MoveVelocityRightSpace.magnitude, _spaceDirections.RbsRightSpace))
                                _spaceDirections.MoveVelocityRightSpace = Vector3.zero;
                            if (_spaceDirections.RbsRightSpace != null && !MoveMoveCube(_spaceDirections.RbsRightSpace, _spaceDirections.MoveVelocityRightSpace, _spaceDirections.MoveSpeed))
                                Debug.LogWarning("右空間：MoveCubeの制御を破棄");
                        }
                        beforeSpaceDirections.MoveVelocityRightSpace = _spaceDirections.MoveVelocityRightSpace;
                    })
                    .AddTo(_compositeDisposable);
                if (!InitializePool())
                    Debug.LogError("プール作成の失敗");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 非同期処理
        /// Updateの１フレーム送り後にコネクトセットリストの内容が空かチェック
        /// セルフコネクト呼び出し判定
        /// </summary>
        /// <param name="connectDirections">ブロックの接続状況リスト</param>
        /// <param name="moveCubes">空間操作ブロック</param>
        private async void AsyncCheckConnectDirection2DListDelayTime(List<ConnectDirection2D> connectDirections, GameObject[] moveCubes)
        {
            await Task.Delay(checkDelayTimeMs);
            if (connectDirections.Count < 1)
                if (!SelfConnectCubes(connectDirections, moveCubes))
                    Debug.LogError("セルフコネクト処理の失敗");
        }

        /// <summary>
        /// セルフコネクト処理
        /// </summary>
        /// <param name="connectDirections">ブロックの接続状況リスト</param>
        /// <param name="moveCubes">空間操作ブロック</param>
        /// <returns>成功／失敗</returns>
        private bool SelfConnectCubes(List<ConnectDirection2D> connectDirections, GameObject[] moveCubes)
        {
            try
            {
                // コネクト事前登録済みのオブジェクト同士を取得
                foreach (var originCube in moveCubes.Where(q => q.GetComponent<MoveCbSmall>().ConnectedLockTo != null && 0 < q.GetComponent<MoveCbSmall>().ConnectedLockTo.Length)
                    .Select(q => q))
                {
                    var pair = new ConnectDirection2D();
                    pair.OriginMoveCube = originCube;
                    foreach (var meetCube in originCube.GetComponent<MoveCbSmall>().ConnectedLockTo)
                    {
                        if (!originCube.transform.parent.Equals(meetCube.transform.parent))
                        {
                            pair.MeetMoveCube = meetCube;
                            pair = GetConnectDirection2DAddDirection(pair);
                            if (1 == SetGroupMattingMoveCube(pair))
                            {
                                var p = new ConnectDirection2D();
                                p.OriginMoveCube = pair.MeetMoveCube;
                                p.MeetMoveCube = pair.OriginMoveCube;
                                p = GetConnectDirection2DAddDirection(p);
                                if (2 == SetGroupMattingMoveCube(p))
                                    ConnectMoveCube();
                                _connectDirections = new List<ConnectDirection2D>();
                            }
                        }
                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// 新たに空間操作ブロックを生成
        /// </summary>
        /// <param name="target">生成する座標</param>
        /// <returns>成功／失敗</returns>
        public bool CreateNewMoveCube(Vector3 target)
        {
            try
            {
                var level = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>();
                var scene = GameManager.Instance.SceneOwner.GetComponent<SceneOwner>();
                var group = Instantiate(moveCubeGroupPrefab, target, Quaternion.identity, level.LevelDesign.transform.GetChild(scene.SceneIdCrumb.Current));
                var moveCube = Instantiate(moveCubePrefab, target, Quaternion.identity, group.transform);
                moveCube.GetComponent<MoveCbSmall>().SetInstanced(true);
                // 空間操作ブロックリストを更新
                GameObject[] newMoveCubes = { moveCube };
                var tmpMoveCubes = SetCollsion(newMoveCubes, _connectSuccess);
                if (tmpMoveCubes == null || (tmpMoveCubes != null && tmpMoveCubes.Length < 1))
                    throw new System.Exception("生成後の空間操作ブロック設定の失敗");
                if (_moveCubes == null || (_moveCubes != null && _moveCubes.Length < 1))
                    throw new System.Exception("空間操作ブロックの内容がありません");
                var mergeMoveCubeList = _moveCubes.ToList();
                foreach (var t in tmpMoveCubes)
                    mergeMoveCubeList.Add(t);
                _moveCubes = mergeMoveCubeList.ToArray();

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// ステージ読み込み後の後から生成された空間操作ブロックを全て削除する
        /// ※条件付きブロックにて使用する
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DestroyNewMoveCube()
        {
            try
            {
                if (_moveCubes == null || (_moveCubes != null && _moveCubes.Length < 1))
                    throw new System.Exception("空間操作ブロックの内容がありません");
                foreach (var cube in _moveCubes.Where(q => q.GetComponent<MoveCbSmall>().Instanced).Select(q => q))
                    Destroy(cube);
                _moveCubes = _moveCubes.Where(q => q != null).Select(q => q).ToArray();

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 空間操作が可能かをチェックする
        /// </summary>
        /// <param name="inputBan">操作可否</param>
        /// <param name="velocitySpaceMagnitude">空間操作のVelocity</param>
        /// <param name="rigidbodiesSpace">左／右空間のRigidbodyの配列</param>
        /// <returns>移動可</returns>
        private bool CheckMovementLeftOrRightSpace(bool inputBan, float velocitySpaceMagnitude, Rigidbody[] rigidbodiesSpace)
        {
            return !inputBan && deadMagnitude < velocitySpaceMagnitude && rigidbodiesSpace != null && 0 < rigidbodiesSpace.Length;
        }

        /// <summary>
        /// 管理系の処理を破棄
        /// </summary>
        public void DisposeAllFromSceneOwner()
        {
            _compositeDisposable.Clear();
        }

        /// <summary>
        /// 衝突判定をセット
        /// ブロックオブジェクトが見つからない場合はnullを返却
        /// </summary>
        /// <param name="moveCubes">空間操作ブロックオブジェクト</param>
        /// <param name="level">レベルデザイン</param>
        /// <param name="count">コネクト成功回数</param>
        /// <returns>セット処理完了／引数情報の一部にnull</returns>
        private GameObject[] SetCollsion(GameObject[] moveCubes, IntReactiveProperty count)
        {
            // 取得したペアがない場合は空オブジェクトを返却
            if (moveCubes == null || moveCubes.Length < -1) return null;
            var level = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current);

            var isDead = false;
            foreach (var obj in moveCubes)
            {
                // 空間内にあるMoveCubeを個々に親オブジェクトをセット
                // コネクト処理にて親オブジェクトが必要となるため
                var group = Instantiate(moveCubeGroupPrefab, obj.transform.position, Quaternion.identity);
                group.transform.parent = level;
                obj.transform.parent = group.transform;
                // 空間操作ブロックの速度ストッパー（弾き飛ばされないようにする）
                group.GetComponent<Rigidbody>().ObserveEveryValueChanged(x => x.velocity)
                    .Subscribe(x =>
                    {
                        if (8f < x.magnitude)
                        {
                            if (_spaceDirections.RbsLeftSpace != null)
                                foreach (var r in _spaceDirections.RbsLeftSpace)
                                    r.isKinematic = true;
                            if (_spaceDirections.RbsRightSpace != null)
                                foreach (var r in _spaceDirections.RbsRightSpace)
                                    r.isKinematic = true;
                        }
                    });
                group.GetComponent<Rigidbody>().ObserveEveryValueChanged(x => x.isKinematic)
                    .Where(x => x)
                    .Subscribe(_ => DOVirtual.DelayedCall(.5f, () => group.GetComponent<Rigidbody>().isKinematic = false));

                var rOrgOffAry = LevelDesisionIsObjected.GetThreePointHorizontal(rayOriginOffset, .5f);
                // プレイヤーの接地判定
                obj.UpdateAsObservable()
                    .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rOrgOffAry[0], rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)) ||
                        LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rOrgOffAry[1], rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)) ||
                        LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rOrgOffAry[2], rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
                    .Select(_ => GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MoveCharactorPlayer(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                    .Where(x => !x)
                    .Subscribe(_ => Debug.LogError("プレイヤー操作指令の失敗"))
                    .AddTo(_compositeDisposable);
                // プレイヤーを押せるように入れたものだが逆に引くことも可能となってしまっているためそれを治す
                obj.UpdateAsObservable()
                    .Where(_ => (LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffsetLeft, rayDirectionLeft, rayMaxDistanceLeft, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)) ||
                        LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffsetRight, rayDirectionRight, rayMaxDistanceRight, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER))) &&
                        (CheckPushingActor(_spaceDirections.RbsLeftSpace, _spaceDirections.MoveVelocityLeftSpace, obj, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().Player.transform.position) ||
                        CheckPushingActor(_spaceDirections.RbsRightSpace, _spaceDirections.MoveVelocityRightSpace, obj, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().Player.transform.position)))
                    .Subscribe(_ =>
                    {
                        if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MoveCharactorPlayer(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                            Debug.LogError("プレイヤー操作指令の失敗");
                    })
                    .AddTo(_compositeDisposable);
                // 敵ギミックの接地判定
                RaycastHit[] hits = new RaycastHit[1];
                obj.UpdateAsObservable()
                    .Select(_ => obj)
                    .Where(x => LevelDesisionIsObjected.IsOnPlayeredAndInfo(x.transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES)))
                    .Subscribe(x =>
                    {
                        var top = LevelDesisionIsObjected.IsOnEnemiesAndInfo(x.transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                        if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MoveCharactorRobotEnemy(x.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime, top))
                            Debug.LogError("敵ギミック操作指令の失敗");
                    })
                    .AddTo(_compositeDisposable);
                obj.UpdateAsObservable()
                    .Select(_ => obj)
                    .Where(x => LevelDesisionIsObjected.IsOnPlayeredAndInfo(x.transform.position, rayOriginOffsetLeft, rayDirectionLeft, rayMaxDistanceLeft, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES)))
                    .Subscribe(x =>
                    {
                        var left = LevelDesisionIsObjected.IsOnEnemiesAndInfo(x.transform.position, rayOriginOffsetLeft, rayDirectionLeft, rayMaxDistanceLeft, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                        if (left != null &&
                            (CheckPushingActor(_spaceDirections.RbsLeftSpace, _spaceDirections.MoveVelocityLeftSpace, x, left.transform.position) ||
                            CheckPushingActor(_spaceDirections.RbsRightSpace, _spaceDirections.MoveVelocityRightSpace, x, left.transform.position)))
                            if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MoveCharactorRobotEnemy(x.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime, left))
                                Debug.LogError("敵ギミック操作指令の失敗");
                    })
                    .AddTo(_compositeDisposable);
                obj.UpdateAsObservable()
                    .Select(_ => obj)
                    .Where(x => LevelDesisionIsObjected.IsOnPlayeredAndInfo(x.transform.position, rayOriginOffsetRight, rayDirectionRight, rayMaxDistanceRight, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES)))
                    .Subscribe(x =>
                    {
                        var right = LevelDesisionIsObjected.IsOnEnemiesAndInfo(x.transform.position, rayOriginOffsetRight, rayDirectionRight, rayMaxDistanceRight, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                        if (right != null &&
                            (CheckPushingActor(_spaceDirections.RbsLeftSpace, _spaceDirections.MoveVelocityLeftSpace, x, right.transform.position) ||
                            CheckPushingActor(_spaceDirections.RbsRightSpace, _spaceDirections.MoveVelocityRightSpace, x, right.transform.position)))
                            if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MoveCharactorRobotEnemy(x.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime, right))
                                Debug.LogError("敵ギミック操作指令の失敗");
                    })
                    .AddTo(_compositeDisposable);
                obj.transform.parent.OnCollisionStayAsObservable()
                    .Where(x => x.gameObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                    .Select(x => GetMatchingMoveCubes(obj, x.gameObject, x.contacts[0].point))
                    .Where(x => 2 == SetGroupMattingMoveCube(x))
                    .Select(_ => ConnectMoveCube())
                    .Subscribe(x =>
                    {
                        if (!x)
                            Debug.LogWarning("MoveCubeのコネクト処理失敗");
                        else
                        {
                            if (!_inputBan)
                            {
                                count.Value++;
                                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SwitchWarpState())
                                    Debug.LogError("ワープ可否フラグ切り替え処理の呼び出し失敗");
                                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().UpdateCountDownConditionalBlock())
                                    Debug.LogError("条件付きブロックのカウントダウン処理の呼び出し失敗");
                                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().UpdateOnOffState())
                                    Debug.LogError("ON/OFFブロックのステータス変更処理の呼び出し失敗");
                            }
                        }
                        _connectDirections = new List<ConnectDirection2D>();
                        // 特定の空間操作ブロックはデフォルトで接続状態にする
                        _connectDirections.ObserveEveryValueChanged(x => x.Count)
                            .Where(x => x < 1 && _moveCubes != null && 0 < _moveCubes.Length)
                            .Subscribe(x => AsyncCheckConnectDirection2DListDelayTime(_connectDirections, _moveCubes))
                            .AddTo(_compositeDisposable);
                    })
                    .AddTo(_compositeDisposable);
                // プレイヤーとの衝突
                obj.transform.parent.OnCollisionEnterAsObservable()
                    .Where(x => x.gameObject.CompareTag(TagConst.TAG_NAME_PLAYER) &&
                    _spaceDirections.MoveSpeed == moveHSpeed &&
                    CheckDirectionMoveCubeToPlayer(obj, LayerConst.LAYER_NAME_PLAYER, CollisionDirectionVelocity(x.relativeVelocity)) &&
                    !isDead)
                    .Subscribe(async _ =>
                    {
                        isDead = true;
                        if (!GameManager.Instance.TutorialOwner.GetComponent<TutorialOwner>().CloseEventsAll())
                            Debug.LogError("チュートリアルのUIイベントリセット処理の失敗");
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().SetShortcuGuideScreenInputBan(true);
                        await GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().DestroyPlayer();
                        GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SetSceneIdUndo();
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().EnableDrawLoadNowFadeOutTrigger();
                    })
                    .AddTo(_compositeDisposable);
                // 敵との衝突
                obj.transform.parent.OnCollisionEnterAsObservable()
                    .Where(x => x.gameObject.CompareTag(TagConst.TAG_NAME_ROBOT_EMEMY) &&
                    _spaceDirections.MoveSpeed == moveHSpeed &&
                    CheckDirectionMoveCubeToPlayer(obj, LayerConst.LAYER_NAME_ROBOTENEMIES, CollisionDirectionVelocity(x.relativeVelocity)))
                    .Subscribe(x =>
                    {
                        var target = CheckDirectionMoveCubeToEmemies(obj, LayerConst.LAYER_NAME_ROBOTENEMIES, CollisionDirectionVelocity(x.relativeVelocity));
                        if (target != null)
                            GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().DestroyRobotEnemies(target);
                    })
                    .AddTo(_compositeDisposable);
            }

            return moveCubes;
        }

        /// <summary>
        /// 衝突方向からどの向きかを判断する
        /// </summary>
        /// <param name="relativeVelocity">衝突時の相対速度</param>
        /// <returns>角度</returns>
        private DirectionAndError CollisionDirectionVelocity(Vector3 relativeVelocity)
        {
            var reverse = relativeVelocity * -1f;
            if (0 < Mathf.Abs(reverse.magnitude))
                if (Mathf.Abs(reverse.x) < Mathf.Abs(reverse.y))
                    if (0 < reverse.y)
                        return DirectionAndError.UP;
                    else
                        return DirectionAndError.DOWN;
                else
                    if (0 < reverse.x)
                    return DirectionAndError.RIGHT;
                else
                    return DirectionAndError.LEFT;
            else
                return DirectionAndError.ERROR;
        }

        /// <summary>
        /// ある動的オブジェクトを横から押す動きを許可するかの判定
        /// </summary>
        /// <param name="rbsLeftOrRightSpace">左／右空間のRigidbody</param>
        /// <param name="moveVelocityLeftOrRightSpace">左／右空間のVelocity</param>
        /// <param name="targetCube">対象の空間操作ブロック</param>
        /// <param name="targetActor">対象の動的オブジェクト位置</param>
        /// <returns>移動可／移動不可</returns>
        private bool CheckPushingActor(Rigidbody[] rbsLeftOrRightSpace, Vector3 moveVelocityLeftOrRightSpace, GameObject targetCube, Vector3 targetActor)
        {
            // 対象のブロックが[左／右]空間に存在するか
            // 親を見るのではなく、子を取り出して参照
            if (rbsLeftOrRightSpace == null)
                return false;

            var space = rbsLeftOrRightSpace.Where(x => x.transform.gameObject.Equals(targetCube.transform.parent.gameObject));
            var find = false;
            foreach (var rb in space)
                find = true;
            if (find)
            {
                // [左]プレイヤー：[右]ブロック　かつ　左方向の移動（プッシュ）
                // [左]ブロック：[右]プレイヤー　かつ　右方向の移動（プッシュ）
                if ((targetActor.x < targetCube.transform.position.x && moveVelocityLeftOrRightSpace.x < 0f) ||
                    (targetCube.transform.position.x < targetActor.x && 0f < moveVelocityLeftOrRightSpace.x))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 衝突したブロック同士がどの方向から衝突したかをセットする
        /// </summary>
        /// <param name="originObject">衝突を受けたオブジェクト</param>
        /// <param name="meetParentObject">衝突したオブジェクト</param>
        /// <param name="contactsPoint">衝突位置の座標</param>
        /// <returns>MoveCubeのペアと角度／空（親オブジェクト内の子オブジェクトが存在しない）</returns>
        private ConnectDirection2D GetMatchingMoveCubes(GameObject originObject, GameObject meetParentObject, Vector3 contactsPoint)
        {
            var pair = GetMinDistanceTheMoveCube(originObject, meetParentObject);
            // 取得したペアがない場合は空オブジェクトを返却
            if (pair.MeetMoveCube == null || pair.MeetMoveCube == null) return new ConnectDirection2D();
            pair.ContactsPoint = contactsPoint;

            return GetConnectDirection2DAddDirection(pair);
        }

        /// <summary>
        /// ペアに対して角度を付与して返却する
        /// </summary>
        /// <param name="pair">ペア（角度の情報が無い状態）</param>
        /// <returns>ペア（角度付き）</returns>
        private ConnectDirection2D GetConnectDirection2DAddDirection(ConnectDirection2D pair)
        {
            Direction result;

            // X座標とY座標を比較して距離がある＝対象軸上にオブジェクトがある
            var colliedDistanceX = Vector2.Distance(new Vector2(pair.OriginMoveCube.transform.position.x, 0f), new Vector2(pair.MeetMoveCube.transform.position.x, 0f));
            var colliedDistanceY = Vector2.Distance(new Vector2(0f, pair.OriginMoveCube.transform.position.y), new Vector2(0f, pair.MeetMoveCube.transform.position.y));
            if (colliedDistanceY < colliedDistanceX)
                // 衝突対象は右または左にある
                result = pair.OriginMoveCube.transform.position.x < pair.MeetMoveCube.transform.position.x ? Direction.RIGHT : Direction.LEFT;
            else
                // 衝突対象は上または下にある
                result = pair.OriginMoveCube.transform.position.y < pair.MeetMoveCube.transform.position.y ? Direction.UP : Direction.DOWN;

            switch (result)
            {
                case Direction.UP:
                    pair.OriginRayDire = Direction.UP;
                    break;
                case Direction.DOWN:
                    pair.OriginRayDire = Direction.DOWN;
                    break;
                case Direction.LEFT:
                    pair.OriginRayDire = Direction.LEFT;
                    break;
                case Direction.RIGHT:
                    pair.OriginRayDire = Direction.RIGHT;
                    break;
                default:
                    throw new System.Exception("角度判定の失敗");
            }

            return pair;
        }

        /// <summary>
        /// 一つのオブジェクトと別の親オブジェクトを比較して最短距離に存在するオブジェクトのペアを取得
        /// </summary>
        /// <param name="origin">ある一つの子オブジェクト</param>
        /// <param name="parentObject">別の親オブジェクト</param>
        /// <returns>MoveCubeのペア／空（親オブジェクト内の子オブジェクトが存在しない）</returns>
        private ConnectDirection2D GetMinDistanceTheMoveCube(GameObject origin, GameObject parentObject)
        {
            // originとmeetの親内にある子同士を比較
            var distancePairList = new List<ConnectDirection2D>();
            for (var i = 0; i < origin.transform.parent.childCount; i++)
            {
                // originとmeetの子オブジェクトのペアを作成
                var workPair = new ConnectDirection2D();
                var orgChild = origin.transform.parent.GetChild(i);
                workPair.OriginMoveCube = orgChild.gameObject;
                for (var j = 0; j < parentObject.transform.childCount; j++)
                {
                    // 距離が近いものから昇順に組み合わせをセットしていく
                    var meetChild = parentObject.transform.GetChild(j);
                    workPair.MeetMoveCube = meetChild.gameObject;
                    workPair.Distance = Vector2.Distance(new Vector2(orgChild.transform.position.x, orgChild.transform.position.y), new Vector2(meetChild.position.x, meetChild.position.y));
                    distancePairList.Add(workPair);
                }
            }
            // 取得したペアがない場合は空オブジェクトを返却
            if (distancePairList.Count < 1)
                return new ConnectDirection2D();

            // 一回目と二回目の情報を保持する
            // 一回目はOriginの昇順、二回目はMeetの昇順
            if (-1 < _connectDirections.Count && _connectDirections.Count < 1)
            {
                IOrderedEnumerable<ConnectDirection2D> sortList = distancePairList.OrderBy(rec => rec.Distance)
                    .ThenBy(rec => rec.OriginMoveCube.name);
                foreach (var sort in sortList)
                    return sort;
            }
            else if (0 < _connectDirections.Count && _connectDirections.Count < 2)
            {
                IOrderedEnumerable<ConnectDirection2D> sortList = distancePairList.OrderBy(rec => rec.Distance)
                    .ThenBy(rec => rec.MeetMoveCube.name);
                foreach (var sort in sortList)
                    return sort;
            }

            return new ConnectDirection2D();
        }

        /// <summary>
        /// ペアをリストへセットする
        /// 親オブジェクトが空になったり、リスト内に同じオブジェクト情報がある場合は「0」を返却
        /// </summary>
        /// <param name="connectDirection">MoveCubeのペアと角度／空（親オブジェクト内の子オブジェクトが存在しない）</param>
        /// <returns>リスト内のオブジェクト数</returns>
        private int SetGroupMattingMoveCube(ConnectDirection2D connectDirection)
        {
            if (connectDirection.OriginMoveCube == null || connectDirection.MeetMoveCube == null)
            {
                // オブジェクトが空＝子無しの親
                // リストを初期化して0を返却
                _connectDirections = new List<ConnectDirection2D>();
                return 0;
            }
            foreach (var conn in _connectDirections)
            {
                if (conn.OriginMoveCube.Equals(connectDirection.OriginMoveCube))
                {
                    _connectDirections = new List<ConnectDirection2D>();
                    return 0;
                }
            }
            _connectDirections.Add(connectDirection);
            return _connectDirections.Count;
        }

        /// <summary>
        /// マッチング済みのペア同士を接続する
        /// </summary>
        /// <returns>接続完了／マッチング失敗</returns>
        private bool ConnectMoveCube()
        {
            // お互いが向かい合っている状態
            if (_connectDirections[0].OriginMoveCube.Equals(_connectDirections[1].MeetMoveCube) &&
                _connectDirections[1].OriginMoveCube.Equals(_connectDirections[0].MeetMoveCube) &&
                _connectDirections[0].OriginRayDire.Equals(ReverseDirection(_connectDirections[1].OriginRayDire)))
            {
                // グループ化してお互いをコネクトする
                var orgTran = _connectDirections[0].OriginMoveCube.transform;
                var metTran = _connectDirections[1].OriginMoveCube.transform;

                // 位置の補正
                switch (_connectDirections[0].OriginRayDire)
                {
                    case Direction.UP:
                        var uMove = (orgTran.position + Vector3.up) - metTran.position;
                        if (CheckMeetDistance(metTran, rayOriginOffsetUp, Vector3.up, uMove.magnitude, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)))
                            return false;
                        metTran.parent.position += uMove;
                        break;
                    case Direction.DOWN:
                        var dMove = (orgTran.position + Vector3.down) - metTran.position;
                        if (CheckMeetDistance(metTran, rayOriginOffsetDown, Vector3.down, dMove.magnitude, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)))
                            return false;
                        metTran.parent.position += dMove;
                        break;
                    case Direction.LEFT:
                        var lMove = (orgTran.position + Vector3.left) - metTran.position;
                        if (CheckMeetDistance(metTran, rayOriginOffsetLeft, Vector3.left, lMove.magnitude, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)))
                            return false;
                        metTran.parent.position += lMove;
                        break;
                    case Direction.RIGHT:
                        var rMove = (orgTran.position + Vector3.right) - metTran.position;
                        if (CheckMeetDistance(metTran, rayOriginOffsetUp, Vector3.right, rMove.magnitude, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)))
                            return false;
                        metTran.parent.position += rMove;
                        break;
                }

                if (!_inputBan)
                {
                    if (!PlayConnectParticle(new Vector3(_connectDirections[0].ContactsPoint.x, _connectDirections[0].ContactsPoint.y, _connectDirections[0].ContactsPoint.z - 1f), orgTran.parent.childCount + metTran.parent.childCount))
                        Debug.Log("パーティクル生成の失敗");
                    GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(ClipToPlay.se_conect_No1);
                }

                // お互いの親にぶら下がっている子オブジェクトの数を比較
                // 多い方の親の傘下に入る
                if (orgTran.parent.childCount < metTran.parent.childCount)
                {
                    var children = new List<Transform>();
                    for (var i = 0; i < orgTran.parent.childCount; i++)
                        children.Add(orgTran.parent.GetChild(i));
                    foreach (var child in children)
                        child.parent = metTran.parent;
                }
                else
                {
                    var children = new List<Transform>();
                    // 片方のグループに集める
                    for (var i = 0; i < metTran.parent.childCount; i++)
                        children.Add(metTran.parent.GetChild(i));
                    foreach (var child in children)
                        child.parent = orgTran.parent;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// コネクトによって座標移動を行う際、事前に移動距離を測る
        /// 静的オブジェクトにMoveCubeが詰まったりしないようにする
        /// </summary>
        /// <param name="meet">衝突ブロック（移動対象）</param>
        /// <param name="offset">始点</param>
        /// <param name="direction">終点</param>
        /// <param name="distance">距離</param>
        /// <param name="layerMask">マスク情報</param>
        /// <returns>移動先に静的オブジェクトがあるか</returns>
        private bool CheckMeetDistance(Transform meet, Vector3 offset, Vector3 direction, float distance, int layerMask)
        {
            foreach (Transform child in meet.parent)
                if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(child.position, offset, direction, distance, layerMask))
                    return true;
            return false;
        }

        /// <summary>
        /// 角度を逆転させる
        /// 上なら下、左なら右
        /// </summary>
        /// <param name="direction">角度・正</param>
        /// <returns>角度・逆</returns>
        private Direction ReverseDirection(Direction direction)
        {
            if (direction.Equals(Direction.UP))
                return Direction.DOWN;
            if (direction.Equals(Direction.DOWN))
                return Direction.UP;
            if (direction.Equals(Direction.LEFT))
                return Direction.RIGHT;
            if (direction.Equals(Direction.RIGHT))
                return Direction.LEFT;
            throw new System.Exception("角度計算の失敗");
        }

        /// <summary>
        /// 接続演出のパーティクルを生成
        /// </summary>
        /// <param name="point">発生位置</param>
        /// <param name="count">コネクションされたオブジェクト数</param>
        /// <returns>成功／失敗</returns>
        private bool PlayConnectParticle(Vector3 point, int count)
        {
            if (3 < count)
            {
                // パーティクルレベル3
                return InstanceConnectedRingsPool(connectedRingPrefabs[(int)ParticleSystemPoolIdx.THIRD], point, Quaternion.identity, _poolGroup.Pools[(int)ParticleSystemPoolIdx.THIRD]);
            }
            else if (2 < count)
            {
                // パーティクルレベル2
                return InstanceConnectedRingsPool(connectedRingPrefabs[(int)ParticleSystemPoolIdx.SECOND], point, Quaternion.identity, _poolGroup.Pools[(int)ParticleSystemPoolIdx.SECOND]);
            }
            else
            {
                // パーティクルレベル1
                return InstanceConnectedRingsPool(connectedRingPrefabs[(int)ParticleSystemPoolIdx.FIRST], point, Quaternion.identity, _poolGroup.Pools[(int)ParticleSystemPoolIdx.FIRST]);
            }
        }

        /// <summary>
        /// プレハブを元にオブジェクトを生成してプールする
        /// ※プール内に存在するならそのオブジェクトを再利用する
        /// </summary>
        /// <param name="connectedRing">プレハブ</param>
        /// <param name="point">生成する位置</param>
        /// <param name="quaternion">クォータニオン</param>
        /// <param name="pool">プール対象のオブジェクトのTransform</param>
        /// <returns>成功／失敗</returns>
        private bool InstanceConnectedRingsPool(GameObject connectedRing, Vector3 point, Quaternion quaternion, Transform pool)
        {
            foreach (Transform t in pool)
            {
                t.SetPositionAndRotation(point, quaternion);
                t.GetComponent<ParticleSystem>().Play();
                return true;
            }
            Instantiate(connectedRing, point, quaternion, pool).GetComponent<ParticleSystem>().Play();
            return true;
        }

        /// <summary>
        /// MoveCubeと壁の間にプレイヤーが挟まれた状態かをチェック
        /// </summary>
        /// <param name="moveCube">対象のMoveCube</param>
        /// <param name="layerName">レイヤー名</param>
        /// <param name="direction">角度</param>
        /// <returns>挟まっている／離れている</returns>
        private bool CheckDirectionMoveCubeToPlayer(GameObject moveCube, string layerName, DirectionAndError direction)
        {
            RaycastHit[] hits = new RaycastHit[1];
            return CheckDirectionMoveCubeToPlayer(moveCube, layerName, out hits, direction);
        }

        /// <summary>
        /// MoveCubeと壁の間にプレイヤーが挟まれた状態かをチェック
        /// </summary>
        /// <param name="moveCube">対象のMoveCube</param>
        /// <param name="layerName">レイヤー名</param>
        /// <param name="hits">ヒットオブジェクト（参照用）</param>
        /// <param name="direction">角度</param>
        /// <returns>挟まっている／離れている</returns>
        private bool CheckDirectionMoveCubeToPlayer(GameObject moveCube, string layerName, out RaycastHit[] hits, DirectionAndError direction)
        {
            var rOrgOffL = rayOriginOffsetLeft;
            var rOrgDireL = rayDirectionLeft;
            var rMaxDisL = rayMaxDistanceLeftLong;
            var rOrgOffR = rayOriginOffsetRight;
            var rOrgDireR = rayDirectionRight;
            var rMaxDisR = rayMaxDistanceRightLong;
            var rOrgOffUpAry = LevelDesisionIsObjected.GetThreePointHorizontal(rayOriginOffsetUp, .5f);
            var rOrgDireUp = rayDirectionUp;
            var rMaxDisUp = rayMaxDistanceUpLong;
            var rOrgOffDownAry = LevelDesisionIsObjected.GetThreePointHorizontal(rayOriginOffsetDown, .5f);
            var rOrgDireDown = rayDirectionDown;
            var rMaxDisDown = rayMaxDistanceDownLong;
            hits = new RaycastHit[1];

            foreach (Transform moveCubeChild in moveCube.transform.parent)
            {
                // MoveCubeから出した光線がプレイヤーとFreezeオブジェクトにヒット
                // MoveCubeから出した光線がプレイヤーと他のMoveCubeにヒット
                // 左
                if (direction.Equals(DirectionAndError.LEFT) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(layerName)))
                {
                    if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE), out hits) ||
                        LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE), out hits))
                    {
                        return true;
                    }
                }
                // 右
                if (direction.Equals(DirectionAndError.RIGHT) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(layerName)))
                {
                    if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE), out hits) ||
                        LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE), out hits))
                    {
                        return true;
                    }
                }
                for (var i = 0; i < 3; i++)
                {
                    // 上
                    if (direction.Equals(DirectionAndError.UP) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(layerName)))
                    {
                        if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE), out hits) ||
                            LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE), out hits))
                        {
                            return true;
                        }
                    }
                    // 下
                    if (direction.Equals(DirectionAndError.DOWN) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(layerName)))
                    {
                        if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE), out hits) ||
                            LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE), out hits))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// MoveCubeと壁の間にプレイヤーが挟まれた状態かをチェック
        /// </summary>
        /// <param name="moveCube">対象のMoveCube</param>
        /// <param name="layerName">レイヤー名</param>
        /// <returns>挟まっている／離れている</returns>
        private GameObject CheckDirectionMoveCubeToEmemies(GameObject moveCube, string layerName, DirectionAndError direction)
        {
            RaycastHit[] hits = new RaycastHit[1];
            return CheckDirectionMoveCubeToEmemies(moveCube, layerName, out hits, direction);
        }

        /// <summary>
        /// MoveCubeと壁の間にプレイヤーが挟まれた状態かをチェック
        /// </summary>
        /// <param name="moveCube">対象のMoveCube</param>
        /// <param name="layerName">レイヤー名</param>
        /// <param name="hits">ヒットオブジェクト（参照用）</param>
        /// <returns>挟まっている／離れている</returns>
        private GameObject CheckDirectionMoveCubeToEmemies(GameObject moveCube, string layerName, out RaycastHit[] hits, DirectionAndError direction)
        {
            var rOrgOffL = rayOriginOffsetLeft;
            var rOrgDireL = rayDirectionLeft;
            var rMaxDisL = rayMaxDistanceLeftLong;
            var rOrgOffR = rayOriginOffsetRight;
            var rOrgDireR = rayDirectionRight;
            var rMaxDisR = rayMaxDistanceRightLong;
            var rOrgOffUpAry = LevelDesisionIsObjected.GetThreePointHorizontal(rayOriginOffsetUp, .5f);
            var rOrgDireUp = rayDirectionUp;
            var rMaxDisUp = rayMaxDistanceUpLong;
            var rOrgOffDownAry = LevelDesisionIsObjected.GetThreePointHorizontal(rayOriginOffsetDown, .5f);
            var rOrgDireDown = rayDirectionDown;
            var rMaxDisDown = rayMaxDistanceDownLong;
            hits = new RaycastHit[1];

            foreach (Transform moveCubeChild in moveCube.transform.parent)
            {
                // MoveCubeから出した光線がプレイヤーとFreezeオブジェクトにヒット
                // MoveCubeから出した光線がプレイヤーと他のMoveCubeにヒット
                // 左
                if (direction.Equals(DirectionAndError.LEFT) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(layerName)))
                {
                    if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES), out hits))
                    {
                        return LevelDesisionIsObjected.IsOnEnemiesAndInfo(moveCubeChild.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                    }
                }
                // 右
                if (direction.Equals(DirectionAndError.RIGHT) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(layerName)))
                {
                    if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES), out hits))
                    {
                        return LevelDesisionIsObjected.IsOnEnemiesAndInfo(moveCubeChild.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                    }
                }
                for (var i = 0; i < 3; i++)
                {
                    // 上
                    if (direction.Equals(DirectionAndError.UP) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(layerName)))
                    {
                        if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES), out hits))
                        {
                            return LevelDesisionIsObjected.IsOnEnemiesAndInfo(moveCubeChild.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                        }
                    }
                    // 下
                    if (direction.Equals(DirectionAndError.DOWN) && LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(layerName)))
                    {
                        if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCubeChild.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES), out hits))
                        {
                            return LevelDesisionIsObjected.IsOnEnemiesAndInfo(moveCubeChild.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(LayerConst.LAYER_NAME_ROBOTENEMIES));
                        }
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// 操作入力を元に制御情報を更新
        /// </summary>
        /// <returns>処理結果の成功／失敗</returns>
        private bool SetMoveVelocotyLeftAndRight()
        {
            // キーボード
            var lCom = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualLAxcel;
            var hztlLKey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualLMove.x;
            var vtclLkey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualLMove.y;
            var rCom = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualRAxcel;
            var hztlRKey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualRMove.x;
            var vtclRkey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualRMove.y;

            // コントローラー
            var hztlL = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoLMove.x;
            var vtclL = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoLMove.y;
            var hztlR = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoRMove.x;
            var vtclR = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoRMove.y;

            if (lCom || rCom)
            {
                // キーボード操作のインプット
                return SetVelocity(hztlLKey, vtclLkey, hztlRKey, vtclRkey);
            }
            else if (0f < Mathf.Abs(hztlL) || 0f < Mathf.Abs(vtclL) || 0f < Mathf.Abs(hztlR) || 0f < Mathf.Abs(vtclR))
            {
                // コントローラー操作のインプット
                return SetVelocity(hztlL, vtclL, hztlR, vtclR);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 移動先情報をセット
        /// </summary>
        /// <param name="horizontalLeft">左空間のHorizontal</param>
        /// <param name="verticalLeft">左空間のVertical</param>
        /// <param name="horizontalRight">右空間のHorizontal</param>
        /// <param name="verticalRight">右空間のVertical</param>
        /// <returns>成功</returns>
        private bool SetVelocity(float horizontalLeft, float verticalLeft, float horizontalRight, float verticalRight)
        {
            _spaceDirections.MoveVelocityLeftSpace = new Vector3(horizontalLeft, verticalLeft);
            _spaceDirections.MoveVelocityRightSpace = new Vector3(horizontalRight, verticalRight);

            return (0f < _spaceDirections.MoveVelocityLeftSpace.magnitude)
                || (0f < _spaceDirections.MoveVelocityRightSpace.magnitude) ? true : false;
        }

        /// <summary>
        /// 動かすブロックの位置が左空間・右空間かを調べて、各空間操作用のリストへ格納
        /// </summary>
        /// <param name="moveCubes">空間操作ブロックオブジェクト</param>
        /// <returns>処理結果の成功／失敗</returns>
        private bool CheckPositionAndSetMoveCubesComponents(GameObject[] moveCubes)
        {
            if (moveCubes != null && 0 < moveCubes.Length)
            {
                // RigidBodyのリスト
                var rbsLeft = new List<Rigidbody>();
                var rbsRight = new List<Rigidbody>();
                // Animatorのリスト
                var scrsLeft = new List<MoveCbSmall>();
                var scrsRight = new List<MoveCbSmall>();

                foreach (var obj in moveCubes)
                {
                    // グループ化されている場合は親のRigidBodyをセット
                    var rb = obj.transform.parent.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? obj.transform.parent.GetComponent<Rigidbody>() : obj.GetComponent<Rigidbody>();
                    // ワープ移動中はオブジェクトの親子関係が一時的に変更となるため、それを考慮する
                    var currentIdx = GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current;
                    var currentLevel = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(currentIdx);
                    var parentLocal = obj.transform.parent.parent.Equals(currentLevel) ? obj.transform.parent : obj.transform.parent.parent;
                    if (parentLocal.localPosition.x < transform.localPosition.x)
                    {
                        // 左空間
                        rbsLeft.Add(rb);
                        scrsLeft.Add(obj.GetComponent<MoveCbSmall>());
                    }
                    else if (transform.localPosition.x < parentLocal.localPosition.x)
                    {
                        //右空間
                        rbsRight.Add(rb);
                        scrsRight.Add(obj.GetComponent<MoveCbSmall>());
                    }
                    else
                        return false;

                }
                // 動かす対象のRigidBody、Animatorを格納
                if (0 < rbsLeft.Count)
                {
                    _spaceDirections.RbsLeftSpace = rbsLeft.Distinct().ToArray();
                    _spaceDirections.ScrLeftSpace = scrsLeft.ToArray();
                }
                else
                {
                    _spaceDirections.RbsLeftSpace = null;
                    _spaceDirections.ScrLeftSpace = null;
                }
                if (0 < rbsRight.Count)
                {
                    _spaceDirections.RbsRightSpace = rbsRight.Distinct().ToArray();
                    _spaceDirections.ScrRightSpace = scrsRight.ToArray();
                }
                else
                {
                    _spaceDirections.RbsRightSpace = null;
                    _spaceDirections.ScrRightSpace = null;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 操作に合わせてエフェクト再生
        /// </summary>
        /// <param name="rigidbodies">Rigidbodyの配列</param>
        /// <returns>成功／失敗</returns>
        private bool PlayEffectMove(Rigidbody[] rigidbodies)
        {
            try
            {
                if (rigidbodies != null && 0 < rigidbodies.Length)
                {
                    foreach (var group in rigidbodies)
                    {
                        var magn = group.velocity.magnitude;
                        if (0f < magn)
                        {
                            foreach (Transform child in group.transform)
                            {
                                child.GetComponent<MoveCbSmall>().OnAirHoverFromSpaceOwner();
                            }
                        }
                        else
                        {
                            foreach (Transform child in group.transform)
                            {
                                child.GetComponent<MoveCbSmall>().OnFreezeFromSpaceOwner();
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// MoveCubeをRigidBodyから動かす
        /// </summary>
        /// <param name="rigidBodySpace">対象のRigidBody</param>
        /// <param name="moveVelocitySpace">移動ベクトル</param>
        /// <param name="moveSpeed">移動速度（初動／移動）</param>
        /// <returns>移動処理完了／RigidBodyの一部がnull</returns>
        private bool MoveMoveCube(Rigidbody[] rigidBodySpace, Vector3 moveVelocitySpace, float moveSpeed)
        {
            for (var i = 0; i < rigidBodySpace.Length; i++)
            {
                if (moveVelocitySpace.Equals(Vector3.zero))
                {
                    rigidBodySpace[i].velocity = Vector3.zero;
                }
                else
                {
                    if (Mathf.Abs(moveVelocitySpace.x) < Mathf.Abs(moveVelocitySpace.y))
                        rigidBodySpace[i].velocity = new Vector3(0f, rigidBodySpace[i].velocity.y, 0f);
                    else
                        rigidBodySpace[i].velocity = new Vector3(rigidBodySpace[i].velocity.x, 0f, 0f);

                    if (!rigidBodySpace[i].isKinematic)
                        rigidBodySpace[i].AddForce(moveVelocitySpace * moveSpeed * (1 - Time.deltaTime));
                }
            }
            return true;
        }

        /// <summary>
        /// MoveCubeをRigidBodyから動かす
        /// </summary>
        /// <param name="moveVelocitySpace">移動ベクトル</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>移動処理完了／RigidBodyの一部がnull</returns>
        public bool MoveRigidBodyMoveCube(Vector3 moveVelocitySpace, GameObject origin)
        {
            try
            {
                var rigidbody = GetMoveCubeParentRigidbody(origin);
                if (rigidbody != null)
                    rigidbody.velocity = moveVelocitySpace;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// オーナーからRigidbodyのステータスを変更
        /// </summary>
        /// <param name="isKinematic">有効／無効</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeRigidBodyStateMoveCube(bool isKinematic, GameObject origin)
        {
            try
            {
                var rigidbody = GetMoveCubeParentRigidbody(origin);
                if (rigidbody != null)
                    rigidbody.isKinematic = isKinematic;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// オーナーからボックスコライダーのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeBoxColliderStateMoveCube(bool isEnabled, GameObject origin)
        {
            try
            {
                var rigidbody = GetMoveCubeParentRigidbody(origin);
                if (rigidbody != null)
                    foreach (Transform t in rigidbody.transform)
                        if (t.GetComponent<BoxCollider>() != null)
                            t.GetComponent<BoxCollider>().enabled = isEnabled;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 空間操作ブロックグループRigidbodyをリストから取得
        /// </summary>
        /// <param name="origin">対象の空間操作ブロック</param>
        /// <returns>空間操作ブロックRigidbody（単体）</returns>
        private Rigidbody GetMoveCubeParentRigidbody(GameObject origin)
        {
            try
            {
                // ワープ移動中はオブジェクトの親子関係が一時的に変更となるため、それを考慮する
                var currentIdx = GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current;
                var currentLevel = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(currentIdx);
                var parentLocal = origin.transform.parent.parent.Equals(currentLevel) ? origin.transform.parent : origin.transform.parent.parent;

                var direction = LevelDesisionIsObjected.CheckPositionAndGetDirection2D(transform, parentLocal.localPosition);
                if (-1 < direction)
                    if (((Direction2D)direction).Equals(Direction2D.Left))
                    {
                        if (_spaceDirections.RbsLeftSpace == null ||
                            (_spaceDirections.RbsLeftSpace != null && _spaceDirections.RbsLeftSpace.Where(q => q.transform.gameObject.Equals(origin.transform.parent.gameObject))
                            .Select(q => q)
                            .ToArray()
                            .Length < 1))
                        {
                            // 空間操作Rigidbodyリストの同期
                            if (!CheckPositionAndSetMoveCubesComponents(_moveCubes))
                                Debug.LogError("制御対象RigidBody格納の失敗");
                        }
                        return _spaceDirections.RbsLeftSpace.Where(q => q.transform.gameObject.Equals(origin.transform.parent.gameObject))
                            .Select(q => q)
                            .ToArray()[0];
                    }
                    else if (((Direction2D)direction).Equals(Direction2D.Right))
                    {
                        if (_spaceDirections.RbsRightSpace == null ||
                            (_spaceDirections.RbsRightSpace != null && _spaceDirections.RbsRightSpace.Where(q => q.transform.gameObject.Equals(origin.transform.parent.gameObject))
                            .Select(q => q)
                            .ToArray()
                            .Length < 1))
                        {
                            // 空間操作Rigidbodyリストの同期
                            if (!CheckPositionAndSetMoveCubesComponents(_moveCubes))
                                Debug.LogError("制御対象RigidBody格納の失敗");
                        }
                        return _spaceDirections.RbsRightSpace.Where(q => q.transform.gameObject.Equals(origin.transform.parent.gameObject))
                            .Select(q => q)
                            .ToArray()[0];
                    }
                return null;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// プールの初期設定
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool InitializePool()
        {
            _poolGroup = new PoolGroup();
            _poolGroup.Pools = new Transform[connectedRingPrefabs.Length];
            _poolGroup.Pools[(int)ParticleSystemPoolIdx.FIRST] = GameObject.Find(connectedRingPrefabs[(int)ParticleSystemPoolIdx.FIRST].name + "Pool") != null ? GameObject.Find(connectedRingPrefabs[(int)ParticleSystemPoolIdx.FIRST].name + "Pool").transform : new GameObject(connectedRingPrefabs[(int)ParticleSystemPoolIdx.FIRST].name + "Pool").transform;
            _poolGroup.Pools[(int)ParticleSystemPoolIdx.SECOND] = GameObject.Find(connectedRingPrefabs[(int)ParticleSystemPoolIdx.SECOND].name + "Pool") != null ? GameObject.Find(connectedRingPrefabs[(int)ParticleSystemPoolIdx.SECOND].name + "Pool").transform : new GameObject(connectedRingPrefabs[(int)ParticleSystemPoolIdx.SECOND].name + "Pool").transform;
            _poolGroup.Pools[(int)ParticleSystemPoolIdx.THIRD] = GameObject.Find(connectedRingPrefabs[(int)ParticleSystemPoolIdx.THIRD].name + "Pool") != null ? GameObject.Find(connectedRingPrefabs[(int)ParticleSystemPoolIdx.THIRD].name + "Pool").transform : new GameObject(connectedRingPrefabs[(int)ParticleSystemPoolIdx.THIRD].name + "Pool").transform;
            return true;
        }
    }

    /// <summary>
    /// 空間操作に必要なRigidBody、Velocity
    /// </summary>
    public struct SpaceDirection2D
    {
        /// <summary>左側のRigidBody</summary>
        public Rigidbody[] RbsLeftSpace { get; set; }
        /// <summary>右側のRigidBody</summary>
        public Rigidbody[] RbsRightSpace { get; set; }
        /// <summary>左側のVelocity</summary>
        public Vector3 MoveVelocityLeftSpace { get; set; }
        /// <summary>右側のVelocity</summary>
        public Vector3 MoveVelocityRightSpace { get; set; }
        /// <summary>移動速度</summary>
        public float MoveSpeed { get; set; }
        /// <summary>左空間のスクリプト</summary>
        public MoveCbSmall[] ScrLeftSpace { get; set; }
        /// <summary>右空間のスクリプト</summary>
        public MoveCbSmall[] ScrRightSpace { get; set; }
    }

    /// <summary>
    /// ブロックの接続状態
    /// </summary>
    public struct ConnectDirection2D
    {
        /// <summary>オリジナルのブロック</summary>
        public GameObject OriginMoveCube { get; set; }
        /// <summary>衝突されたブロック</summary>
        public GameObject MeetMoveCube { get; set; }
        /// <summary>オリジナルのブロックのレイ</summary>
        public Direction OriginRayDire { get; set; }
        /// <summary>接続位置</summary>
        public Vector3 ContactsPoint { get; set; }
        /// <summary>距離</summary>
        public float Distance { get; set; }
    }

    /// <summary>
    /// 角度
    /// </summary>
    public enum Direction
    {
        /// <summary>上</summary>
        UP
        /// <summary>下</summary>
        , DOWN
        /// <summary>左</summary>
        , LEFT
        /// <summary>右</summary>
        , RIGHT
    }

    /// <summary>
    /// 角度（例外付き）
    /// </summary>
    public enum DirectionAndError
    {
        /// <summary>上</summary>
        UP,
        /// <summary>下</summary>
        DOWN,
        /// <summary>左</summary>
        LEFT,
        /// <summary>右</summary>
        RIGHT,
        /// <summary>エラー</summary>
        ERROR,
    }

    /// <summary>
    /// プールのグループ
    /// </summary>
    public struct PoolGroup
    {
        /// <summary>接続演出のプレハブ（リング）のプール</summary>
        public Transform[] Pools { get; set; }
    }

    /// <summary>
    /// オブジェクトプール
    /// パーティクル用のインデックス
    /// </summary>
    public enum ParticleSystemPoolIdx
    {
        /// <summary>1番目</summary>
        FIRST,
        /// <summary>2番目</summary>
        SECOND,
        /// <summary>3番目</summary>
        THIRD,
    }
}
