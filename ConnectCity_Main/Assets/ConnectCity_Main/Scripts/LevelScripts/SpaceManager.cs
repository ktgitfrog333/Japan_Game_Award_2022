using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;
using UniRx;
using UniRx.Triggers;
using Common.LevelDesign;

/// <summary>
/// 空間制御する
/// </summary>
public class SpaceManager : MonoBehaviour
{
    /// <summary>移動速度</summary>
    [SerializeField] private float moveSpeed = 3.5f;
    /// <summary>動くキューブをグループ化するプレハブ</summary>
    [SerializeField] private GameObject moveCubeGroup;

    /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
    private static readonly Vector3 ISPLAYERED_RAY_ORIGIN_OFFSET = new Vector3(0f, -0.1f);
    /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
    private static readonly Vector3 ISPLAYERED_RAY_DIRECTION = Vector3.up;
    /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
    private static readonly float ISPLAYERED_RAY_MAX_DISTANCE = 0.9f;
    /// <summary>空間操作に必要なRigidBody、Velocity</summary>
    private SpaceDirection2D _spaceDirections = new SpaceDirection2D();
    /// <summary>ブロックの接続状況</summary>
    private List<ConnectDirection2D> _connectDirections = new List<ConnectDirection2D>();

    private void Start()
    {
        var moveCubes = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBE);

        // ブロックの接続状況
        if (!SetCollsion(moveCubes, GameObject.FindGameObjectWithTag(TagConst.TAG_NAME_LEVELDESIGN).transform))
            throw new System.Exception("オブジェクト取得の失敗");
        // 移動入力をチェック
        this.UpdateAsObservable()
            .Subscribe(_ => SetMoveVelocotyLeftAndRight());
        // 空間内のブロック座標をチェック
        this.UpdateAsObservable()
            .Select(_ => CheckPositionAndSetMoveCubesRigidbodies(moveCubes))
            .Where(x => !x)
            .Subscribe(_ => throw new System.Exception("制御対象RigidBody格納の失敗"));
        var moveCubeGroups = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBEGROUP);
        // 不要なグループ削除
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                foreach (var obj in moveCubeGroups)
                    if (obj != null && obj.transform.childCount < 1) Destroy(obj);
            });
        // 左空間の制御
        this.FixedUpdateAsObservable()
            .Where(_ => 0f < _spaceDirections.MoveVelocityLeftSpace.magnitude && _spaceDirections.RbsLeftSpace != null && 0 < _spaceDirections.RbsLeftSpace.Length)
            .Select(_ => MoveMoveCube(_spaceDirections.RbsLeftSpace, _spaceDirections.MoveVelocityLeftSpace))
            .Where(x => !x)
            .Subscribe(_ => Debug.Log("左空間：MoveCubeの制御を破棄"));
        // 右空間の制御
        this.FixedUpdateAsObservable()
            .Where(_ => 0f < _spaceDirections.MoveVelocityRightSpace.magnitude && _spaceDirections.RbsRightSpace != null && 0 < _spaceDirections.RbsRightSpace.Length)
            .Select(_ => MoveMoveCube(_spaceDirections.RbsRightSpace, _spaceDirections.MoveVelocityRightSpace))
            .Where(x => !x)
            .Subscribe(_ => Debug.Log("右空間：MoveCubeの制御を破棄"));

        //// デバッグ用
        //this.UpdateAsObservable()
        //    .Subscribe(_ =>
        //    {
        //        var game = new List<GameObject>();
        //        foreach(var g in _connectDirections)
        //        {
        //            game.Add(g.UpConnectedBlock);
        //            game.Add(g.DownConnectedBlock);
        //            game.Add(g.LeftConnectedBlock);
        //            game.Add(g.RightConnectedBlock);
        //        }
        //        gameObjects1 = game.ToArray();
        //    });
    }

    /// <summary>
    /// MoveCubeをRigidBodyから動かす
    /// </summary>
    /// <param name="rigidBodySpace">対象のRigidBody</param>
    /// <param name="moveVelocitySpace">移動ベクトル</param>
    /// <returns>移動処理完了／RigidBodyの一部がnull</returns>
    private bool MoveMoveCube(Rigidbody[] rigidBodySpace, Vector3 moveVelocitySpace)
    {
        foreach (var rb in rigidBodySpace)
            if (rb == null) return false;
        foreach (var rb in rigidBodySpace)
            rb.AddForce(moveVelocitySpace + moveVelocitySpace * Time.deltaTime);
        return true;
    }

    [SerializeField] private GameObject[] gameObjects1;

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

            obj.UpdateAsObservable()
                .Where(_ => LevelDesisionIsObjected.IsOnPlayeredAndInfo(obj.transform.position, ISPLAYERED_RAY_ORIGIN_OFFSET, ISPLAYERED_RAY_DIRECTION, ISPLAYERED_RAY_MAX_DISTANCE, LayerMask.GetMask(LayerConst.LAYER_NAME_PLAYER)))
                .Select(_ => GameManager.Instance.MoveCharactorFromSpaceManager(obj.GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero) * Time.deltaTime))
                .Where(x => x!)
                .Subscribe(_ => Debug.Log("プレイヤー操作指令の失敗"));
            obj.transform.parent.OnCollisionEnterAsObservable()
                .Where(x => x.gameObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
                .Select(x => GetMatchingMoveCubes(obj, x.gameObject))
                .Where(x => 2 == SetGroupMattingMoveCube(x))
                .Select(_ => ConnectMoveCube())
                .Subscribe(x =>
                {
                    if (!x) Debug.Log("MoveCubeのコネクト処理失敗");
                    _connectDirections = new List<ConnectDirection2D>();
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
    private ConnectDirection2D GetMatchingMoveCubes(GameObject originObject, GameObject meetParentObject)
    {
        var pair = GetMinDistanceTheMoveCube(originObject, meetParentObject);
        // 取得したペアがない場合は空オブジェクトを返却
        if (pair.MeetMoveCube == null || pair.MeetMoveCube == null) return new ConnectDirection2D();
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
    /// 操作入力を元に制御情報を更新
    /// </summary>
    /// <returns>処理結果の成功／失敗</returns>
    private bool SetMoveVelocotyLeftAndRight()
    {
        var hztlL = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_LS);
        var vtclL = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_LS);
        var hztlR = Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_RS);
        var vtclR = Input.GetAxis(InputConst.INPUT_CONST_VERTICAL_RS);

        if (0f < Mathf.Abs(hztlL) || 0f < Mathf.Abs(vtclL) || 0f < Mathf.Abs(hztlR) || 0f < Mathf.Abs(vtclR))
        {
            _spaceDirections.MoveVelocityLeftSpace = new Vector3(hztlL, vtclL) * moveSpeed;
            _spaceDirections.MoveVelocityRightSpace = new Vector3(hztlR, vtclR) * moveSpeed;

            return (0f < _spaceDirections.MoveVelocityLeftSpace.magnitude)
                || (0f < _spaceDirections.MoveVelocityRightSpace.magnitude) ? true : false;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 動かすブロックの位置が左空間・右空間かを調べて、各空間操作用のリストへ格納
    /// </summary>
    /// <returns>処理結果の成功／失敗</returns>
    private bool CheckPositionAndSetMoveCubesRigidbodies(GameObject[] gameObjects)
    {
        if (0 < gameObjects.Length)
        {
            var rbsLeft = new List<Rigidbody>();
            var rbsRight = new List<Rigidbody>();
            foreach (var obj in gameObjects)
            {
                // グループ化されている場合は親のRigidBodyをセット
                var rb = obj.transform.parent.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) ? obj.transform.parent.GetComponent<Rigidbody>() : obj.GetComponent<Rigidbody>();
                if (obj.transform.position.x < 0f)
                    // 左空間
                    rbsLeft.Add(rb);
                else if (0f < obj.transform.position.x)
                    // 右空間
                    rbsRight.Add(rb);
                else
                    return false;
            }
            // 動かす対象のRigidBodyを格納
            if (0 < rbsLeft.Count)
                _spaceDirections.RbsLeftSpace = rbsLeft.ToArray();
            else
                _spaceDirections.RbsLeftSpace = null;
            if (0 < rbsRight.Count)
                _spaceDirections.RbsRightSpace = rbsRight.ToArray();
            else
                _spaceDirections.RbsRightSpace = null;
            return true;
        }
        else
        {
            return false;
        }
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
}
/// <summary>
/// ブロックの接続状態
/// </summary>
public struct ConnectDirection2D
{
    /// <summary>オリジナルのブロック</summary>
    public GameObject OriginMoveCube { get; set; }
    public GameObject MeetMoveCube { get; set; }
    public Direction OriginRayDire { get; set; }
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
