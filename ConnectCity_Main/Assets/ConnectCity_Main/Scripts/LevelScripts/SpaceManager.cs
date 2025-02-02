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

namespace Main.Level
{
    /// <summary>
    /// 空間制御する
    /// </summary>
    public class SpaceManager : MonoBehaviour
    {
        /// <summary>移動速度（初動フェーズ）</summary>
        [SerializeField] private float moveLSpeed = .3f;
        /// <summary>移動速度（移動フェーズ）</summary>
        [SerializeField] private float moveHSpeed = 3.5f;
        /// <summary>長押ししてから移動フェーズへ以降するまでの時間</summary>
        [SerializeField] private float gearChgDelaySec = 2f;
        /// <summary>動くキューブをグループ化するプレハブ</summary>
        [SerializeField] private GameObject moveCubeGroup;
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
        /// <summary>ステージ範囲（UP/DOWN/LEFT/RIGHT）</summary>
        [SerializeField] private float[] stageScaleMaxDistance = new float[4];
        /// <summary>空間操作に必要なRigidBody、Velocity</summary>
        private SpaceDirection2D _spaceDirections = new SpaceDirection2D();
        /// <summary>ブロックの接続状況</summary>
        private List<ConnectDirection2D> _connectDirections = new List<ConnectDirection2D>();
        /// <summary>プールのグループ</summary>
        private PoolGroup _poolGroup;

        private void Start()
        {
            // ブロックの接続状況
            var moveCubes = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBE);
            if (!SetCollsion(moveCubes, GameObject.FindGameObjectWithTag(TagConst.TAG_NAME_LEVELDESIGN).transform))
                throw new System.Exception("オブジェクト取得の失敗");
            // 速度の初期値
            _spaceDirections.MoveSpeed = moveLSpeed;
            // 移動入力をチェック
            var velocitySeted = new BoolReactiveProperty();
            this.UpdateAsObservable()
                .Subscribe(_ => velocitySeted.Value = SetMoveVelocotyLeftAndRight());
            velocitySeted.Where(x => x)
                .Throttle(System.TimeSpan.FromSeconds(gearChgDelaySec))
                .Where(_ => velocitySeted.Value)
                .Subscribe(_ => _spaceDirections.MoveSpeed = moveHSpeed);
            velocitySeted.Where(x => !x)
                .Subscribe(_ => {
                    _spaceDirections.MoveSpeed = moveLSpeed;
                });

            // 空間内のブロック座標をチェック
            this.UpdateAsObservable()
                .Select(_ => CheckPositionAndSetMoveCubesComponents(moveCubes))
                .Where(x => !x)
                .Subscribe(_ => Debug.LogError("制御対象RigidBody格納の失敗"));
            // 不要なグループ削除
            var moveCubeGroups = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBEGROUP);
            foreach (var obj in moveCubeGroups)
                obj.ObserveEveryValueChanged(x => x.transform.childCount < 1)
                    .Where(x => x)
                    .Subscribe(_ => Destroy(obj));
            // 左空間の制御
            this.FixedUpdateAsObservable()
                .Where(_ => 0f < _spaceDirections.MoveVelocityLeftSpace.magnitude && _spaceDirections.RbsLeftSpace != null && 0 < _spaceDirections.RbsLeftSpace.Length)
                .Subscribe(_ =>
                {
                    if (_spaceDirections.MoveVelocityLeftSpace.magnitude < deadMagnitude)
                        _spaceDirections.MoveVelocityLeftSpace = Vector3.zero;
                    if (!MoveMoveCube(_spaceDirections.RbsLeftSpace, _spaceDirections.ScrLeftSpace, _spaceDirections.MoveVelocityLeftSpace, _spaceDirections.MoveSpeed))
                        Debug.Log("左空間：MoveCubeの制御を破棄");
                });
            // 右空間の制御
            this.FixedUpdateAsObservable()
                .Where(_ => 0f < _spaceDirections.MoveVelocityRightSpace.magnitude && _spaceDirections.RbsRightSpace != null && 0 < _spaceDirections.RbsRightSpace.Length)
                .Subscribe(_ =>
                {
                    if (_spaceDirections.MoveVelocityRightSpace.magnitude < deadMagnitude)
                        _spaceDirections.MoveVelocityRightSpace = Vector3.zero;
                    if (!MoveMoveCube(_spaceDirections.RbsRightSpace, _spaceDirections.ScrRightSpace, _spaceDirections.MoveVelocityRightSpace, _spaceDirections.MoveSpeed))
                        Debug.Log("左空間：MoveCubeの制御を破棄");
                });
            if (!InitializePool())
                Debug.LogError("プール作成の失敗");
            //// デバッグ
            //this.UpdateAsObservable()
            //    .Subscribe(_ =>
            //    {
            //        Debug.DrawRay(Vector3.zero, new Vector3(0f, stageScaleMaxDistance[(int)Direction.UP]), Color.green);
            //        Debug.DrawRay(Vector3.zero, new Vector3(0f, stageScaleMaxDistance[(int)Direction.DOWN] * -1f), Color.green);
            //        Debug.DrawRay(Vector3.zero, new Vector3(stageScaleMaxDistance[(int)Direction.LEFT] * -1f, 0f), Color.green);
            //        Debug.DrawRay(Vector3.zero, new Vector3(stageScaleMaxDistance[(int)Direction.RIGHT], 0f), Color.green);
            //    });
        }

        /// <summary>
        /// ステージの空間操作範囲を設定
        /// GameManagerからの呼び出し
        /// </summary>
        /// <param name="vector4">UP/DOWN/LEFT/RIGHT</param>
        /// <returns>成功／失敗</returns>
        public bool SetStageScaleMaxDistanceFromGameManager(Vector4 vector4)
        {
            stageScaleMaxDistance[(int)Direction.UP] = vector4.x;
            stageScaleMaxDistance[(int)Direction.DOWN] = vector4.y;
            stageScaleMaxDistance[(int)Direction.LEFT] = vector4.z;
            stageScaleMaxDistance[(int)Direction.RIGHT] = vector4.w;
            return true;
        }

        /// <summary>
        /// 衝突判定をセット
        /// ブロックオブジェクトが見つからない場合はnullを返却
        /// </summary>
        /// <param name="moveCubes">空間操作ブロックオブジェクト</param>
        /// <param name="level">レベルデザイン</param>
        /// <returns>セット処理完了／引数情報の一部にnull</returns>
        private bool SetCollsion(GameObject[] moveCubes, Transform level)
        {
            // 取得したペアがない場合は空オブジェクトを返却
            if (moveCubes == null || moveCubes.Length < -1 || level == null) return false;

            foreach (var obj in moveCubes)
            {
                // 空間内にあるMoveCubeを個々に親オブジェクトをセット
                // コネクト処理にて親オブジェクトが必要となるため
                var group = Instantiate(moveCubeGroup, obj.transform.position, Quaternion.identity);
                group.transform.parent = level;
                obj.transform.parent = group.transform;
                // プレイヤーの接地判定
                obj.UpdateAsObservable()
                    .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
                    .Select(_ => GameManager.Instance.MoveCharactorFromSpaceManager(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                    .Where(x => !x)
                    .Subscribe(_ => Debug.LogError("プレイヤー操作指令の失敗"));
                obj.UpdateAsObservable()
                    .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffsetLeft, rayDirectionLeft, rayMaxDistanceLeft, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
                    .Select(_ => GameManager.Instance.MoveCharactorFromSpaceManager(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                    .Where(x => !x)
                    .Subscribe(_ => Debug.LogError("プレイヤー操作指令の失敗"));
                obj.UpdateAsObservable()
                    .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffsetRight, rayDirectionRight, rayMaxDistanceRight, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
                    .Select(_ => GameManager.Instance.MoveCharactorFromSpaceManager(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                    .Where(x => !x)
                    .Subscribe(_ => Debug.LogError("プレイヤー操作指令の失敗"));
                // 敵ギミックの接地判定
                obj.UpdateAsObservable()
                    .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffset, rayDirection, rayMaxDistance, LayerMask.GetMask(LayerConst.LAYER_NAME_HUMANENEMIES)))
                    .Select(_ => GameManager.Instance.MoveHumanEnemyFromSpaceManager(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                    .Where(x => !x)
                    .Subscribe(_ => Debug.LogError("敵ギミック操作指令の失敗"));
                obj.UpdateAsObservable()
                    .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffsetLeft, rayDirectionLeft, rayMaxDistanceLeft, LayerMask.GetMask(LayerConst.LAYER_NAME_HUMANENEMIES)))
                    .Select(_ => GameManager.Instance.MoveHumanEnemyFromSpaceManager(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                    .Where(x => !x)
                    .Subscribe(_ => Debug.LogError("敵ギミック操作指令の失敗"));
                obj.UpdateAsObservable()
                    .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, rayOriginOffsetRight, rayDirectionRight, rayMaxDistanceRight, LayerMask.GetMask(LayerConst.LAYER_NAME_HUMANENEMIES)))
                    .Select(_ => GameManager.Instance.MoveHumanEnemyFromSpaceManager(obj.transform.parent.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                    .Where(x => !x)
                    .Subscribe(_ => Debug.LogError("敵ギミック操作指令の失敗"));
                obj.transform.parent.OnCollisionEnterAsObservable()
                    .Where(x => x.gameObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                    .Select(x => GetMatchingMoveCubes(obj, x.gameObject, x.contacts[0].point))
                    .Where(x => 2 == SetGroupMattingMoveCube(x))
                    .Select(_ => ConnectMoveCube())
                    .Subscribe(x =>
                    {
                        if (!x) Debug.Log("MoveCubeのコネクト処理失敗");
                        _connectDirections = new List<ConnectDirection2D>();
                    });
                // 全てのMoveCubeから左右にレイをとばしてプレイヤーとFreezeを貫通したらTrue
                var pressed = new ReactiveProperty<bool>();
                obj.UpdateAsObservable()
                    .Where(_ => _spaceDirections.MoveSpeed == moveHSpeed)
                    .Select(_ => CheckDirectionMoveCubeToPlayer(obj, LayerConst.LAYER_NAME_PLAYER))
                    .Subscribe(x => pressed.Value = x);
                pressed.Where(x => x)
                    .Subscribe(async _ =>
                    {
                        pressed.Value = false;
                        await GameManager.Instance.DeadPlayerFromSpaceManager();
                        SceneInfoManager.Instance.SetSceneIdRedo();
                        UIManager.Instance.EnableDrawLoadNowFadeOutTrigger();
                    });
                // 全てのMoveCubeから左右にレイをとばして敵ギミックとFreezeを貫通したらTrue
                var pressedEnem = new ReactiveProperty<bool>();
                obj.UpdateAsObservable()
                    .Where(_ => _spaceDirections.MoveSpeed == moveHSpeed)
                    .Select(_ => CheckDirectionMoveCubeToPlayer(obj, LayerConst.LAYER_NAME_HUMANENEMIES))
                    .Subscribe(x => pressedEnem.Value = x);
                pressedEnem.Where(x => x)
                    .Subscribe(_ =>
                    {
                        pressedEnem.Value = false;
                        GameManager.Instance.DestroyHumanEnemyFromSpaceManager();
                    });
            }

            return true;
        }

        /// <summary>
        /// 衝突したブロック同士がどの方向から衝突したかをセットする
        /// </summary>
        /// <param name="originObject">衝突を受けたオブジェクト</param>
        /// <param name="meetParentObject">衝突したオブジェクト</param>
        /// <returns>MoveCubeのペアと角度／空（親オブジェクト内の子オブジェクトが存在しない）</returns>
        private ConnectDirection2D GetMatchingMoveCubes(GameObject originObject, GameObject meetParentObject, Vector3 contactsPoint)
        {
            var pair = GetMinDistanceTheMoveCube(originObject, meetParentObject);
            // 取得したペアがない場合は空オブジェクトを返却
            if (pair.MeetMoveCube == null || pair.MeetMoveCube == null) return new ConnectDirection2D();
            pair.ContactsPoint = contactsPoint;
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
            var distancePair = new SortedDictionary<float, ConnectDirection2D>();
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
                    distancePair[Vector2.Distance(new Vector2(orgChild.transform.position.x, orgChild.transform.position.y), new Vector2(meetChild.position.x, meetChild.position.y))] = workPair;
                }
            }
            // 取得したペアがない場合は空オブジェクトを返却
            if (distancePair.Count < 1) return new ConnectDirection2D();

            var pair = new ConnectDirection2D();
            // 最初の値（最小値）のみ取り出す
            foreach (var p in distancePair)
            {
                pair.OriginMoveCube = p.Value.OriginMoveCube;
                pair.MeetMoveCube = p.Value.MeetMoveCube;
                break;
            }

            return pair;
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

                if (!PlayConnectParticle(new Vector3(_connectDirections[0].ContactsPoint.x, _connectDirections[0].ContactsPoint.y, _connectDirections[0].ContactsPoint.z - 1f) , orgTran.parent.childCount + metTran.parent.childCount))
                    Debug.Log("パーティクル生成の失敗");
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_menu);

                // 位置の補正
                switch (_connectDirections[0].OriginRayDire)
                {
                    case Direction.UP:
                        metTran.position = orgTran.position + Vector3.up;
                        break;
                    case Direction.DOWN:
                        metTran.position = orgTran.position + Vector3.down;
                        break;
                    case Direction.LEFT:
                        metTran.position = orgTran.position + Vector3.left;
                        break;
                    case Direction.RIGHT:
                        metTran.position = orgTran.position + Vector3.right;
                        break;
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
                return InstanceConnectedRingsPool(connectedRingPrefabs[(int)ParticleSystemPoolIdx.THIRD], point, Quaternion.identity, _poolGroup.ConnectedRingPools[(int)ParticleSystemPoolIdx.THIRD]);
            }
            else if (2 < count)
            {
                // パーティクルレベル2
                return InstanceConnectedRingsPool(connectedRingPrefabs[(int)ParticleSystemPoolIdx.SECOND], point, Quaternion.identity, _poolGroup.ConnectedRingPools[(int)ParticleSystemPoolIdx.SECOND]);
            }
            else
            {
                // パーティクルレベル1
                return InstanceConnectedRingsPool(connectedRingPrefabs[(int)ParticleSystemPoolIdx.FIRST], point, Quaternion.identity, _poolGroup.ConnectedRingPools[(int)ParticleSystemPoolIdx.FIRST]);
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
        /// <returns>挟まっている／離れている</returns>
        private bool CheckDirectionMoveCubeToPlayer(GameObject moveCube, string layerName)
        {
            var rOrgOffL = rayOriginOffsetLeft;
            var rOrgDireL = rayDirectionLeft;
            var rMaxDisL = rayMaxDistanceLeftLong;
            var rOrgOffR = rayOriginOffsetRight;
            var rOrgDireR = rayDirectionRight;
            var rMaxDisR = rayMaxDistanceRightLong;
            var rOrgOffUpAry = GetThreePointHorizontal(rayOriginOffsetUp, .5f);
            var rOrgDireUp = rayDirectionUp;
            var rMaxDisUp = rayMaxDistanceUpLong;
            var rOrgOffDownAry = GetThreePointHorizontal(rayOriginOffsetDown, .5f);
            var rOrgDireDown = rayDirectionDown;
            var rMaxDisDown = rayMaxDistanceDownLong;

            // MoveCubeから出した光線がプレイヤーとFreezeオブジェクトにヒット
            // MoveCubeから出した光線がプレイヤーと他のMoveCubeにヒット
            // 左
            if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(layerName)))
            {
                if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffL, rOrgDireL, rMaxDisL, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                {
                    return true;
                }
            }
            // 右
            if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(layerName)))
            {
                if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                    LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffR, rOrgDireR, rMaxDisR, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                {
                    return true;
                }
            }
            for (var i = 0; i < 3; i++)
            {
                // 上
                if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(layerName)))
                {
                    if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                        LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffUpAry[i], rOrgDireUp, rMaxDisUp, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                    {
                        return true;
                    }
                }
                // 下
                if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(layerName)))
                {
                    if (LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) ||
                        LevelDesisionIsObjected.IsOnPlayeredAndInfo(moveCube.transform.position, rOrgOffDownAry[i], rOrgDireDown, rMaxDisDown, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// レイの光線を3本たてる
        /// 補正値によって幅を調整する
        /// </summary>
        /// <param name="rayOriginOffset">基準点（中央点）</param>
        /// <param name="range">幅を広げる範囲</param>
        /// <returns>3点ベクター</returns>
        private Vector3[] GetThreePointHorizontal(Vector3 rayOriginOffset, float range)
        {
            var idx = 0;
            var result = new Vector3[3];
            result[idx++] = new Vector3(-1f * range, rayOriginOffset.y);
            result[idx++] = rayOriginOffset;
            result[idx++] = new Vector3(1f * range, rayOriginOffset.y);
            return result;
        }

        /// <summary>
        /// 操作入力を元に制御情報を更新
        /// </summary>
        /// <returns>処理結果の成功／失敗</returns>
        private bool SetMoveVelocotyLeftAndRight()
        {
            // キーボード
            var lCom = Input.GetButton(InputConst.INPUT_CONST_LS_COM);
            var hztlLKey = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_LS_KEYBOD);
            var vtclLkey = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_LS_KEYBOD);
            var rCom = Input.GetButton(InputConst.INPUT_CONST_RS_COM);
            var hztlRKey = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_RS_KEYBOD);
            var vtclRkey = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_RS_KEYBOD);

            // コントローラー
            var hztlL = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_LS);
            var vtclL = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_LS);
            var hztlR = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_RS);
            var vtclR = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_RS);

            if ((lCom && 0f < Mathf.Abs(hztlLKey)) || (lCom && 0f < Mathf.Abs(vtclLkey)) ||
                (rCom && 0f < Mathf.Abs(hztlRKey)) || (rCom && 0f < Mathf.Abs(vtclRkey)))
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
        /// <returns>処理結果の成功／失敗</returns>
        private bool CheckPositionAndSetMoveCubesComponents(GameObject[] gameObjects)
        {
            if (0 < gameObjects.Length)
            {
                // RididBodyのリスト
                var rbsLeft = new List<Rigidbody>();
                var rbsRight = new List<Rigidbody>();
                // Animatorのリスト
                var scrsLeft = new List<MoveCbSmall>();
                var scrsRight = new List<MoveCbSmall>();

                foreach (var obj in gameObjects)
                {
                    // グループ化されている場合は親のRigidBodyをセット
                    var rb = obj.transform.parent.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? obj.transform.parent.GetComponent<Rigidbody>() : obj.GetComponent<Rigidbody>();
                    if (obj.transform.position.x < 0f)
                    {
                        // 左空間
                        rbsLeft.Add(rb);
                        scrsLeft.Add(obj.GetComponent<MoveCbSmall>());
                    }
                    else if (0f < obj.transform.position.x)
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
                    _spaceDirections.RbsLeftSpace = rbsLeft.ToArray();
                    _spaceDirections.ScrLeftSpace = scrsLeft.ToArray();
                }
                else
                {
                    _spaceDirections.RbsLeftSpace = null;
                    _spaceDirections.ScrLeftSpace = null;
                }
                if (0 < rbsRight.Count)
                {
                    _spaceDirections.RbsRightSpace = rbsRight.ToArray();
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
        /// MoveCubeをRigidBodyから動かす
        /// </summary>
        /// <param name="rigidBodySpace">対象のRigidBody</param>
        /// <param name="moveVelocitySpace">移動ベクトル</param>
        /// <param name="moveSpeed">移動速度（初動／移動）</param>
        /// <returns>移動処理完了／RigidBodyの一部がnull</returns>
        private bool MoveMoveCube(Rigidbody[] rigidBodySpace, MoveCbSmall[] objs, Vector3 moveVelocitySpace, float moveSpeed)
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

                    rigidBodySpace[i].AddForce(moveVelocitySpace * moveSpeed * (1 - Time.deltaTime));
                }
                objs[i].PlayMoveCbAnimation(moveSpeed);
            }
            return true;
        }

        /// <summary>
        /// プールの初期設定
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool InitializePool()
        {
            _poolGroup = new PoolGroup();
            _poolGroup.ConnectedRingPools = new Transform[connectedRingPrefabs.Length];
            _poolGroup.ConnectedRingPools[(int)ParticleSystemPoolIdx.FIRST] = new GameObject(connectedRingPrefabs[(int)ParticleSystemPoolIdx.FIRST].name + "Pool").transform;
            _poolGroup.ConnectedRingPools[(int)ParticleSystemPoolIdx.SECOND] = new GameObject(connectedRingPrefabs[(int)ParticleSystemPoolIdx.SECOND].name + "Pool").transform;
            _poolGroup.ConnectedRingPools[(int)ParticleSystemPoolIdx.THIRD] = new GameObject(connectedRingPrefabs[(int)ParticleSystemPoolIdx.THIRD].name + "Pool").transform;
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
    /// プールのグループ
    /// </summary>
    public struct PoolGroup
    {
        /// <summary>接続演出のプレハブ（リング）のプール</summary>
        public Transform[] ConnectedRingPools { get; set; }
    }

    /// <summary>
    /// オブジェクトプール
    /// パーティクル用のインデックス
    /// </summary>
    public enum ParticleSystemPoolIdx
    {
        /// <summary>0番目</summary>
        FIRST
        /// <summary>1番目</summary>
            , SECOND
        /// <summary>2番目</summary>
            , THIRD
    }
}
