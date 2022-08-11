using Main.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Main.Common.LevelDesign;
using Main.Common.Const;
using System.Linq;

namespace Main.Level
{
    /// <summary>
    /// チュートリアルのエンバイロメント
    /// </summary>
    public class TutorialEnvironment : MonoBehaviour, IOwner, ITutorialOwnerEnvironment
    {
        /// <summary>
        /// ゴーストのMoveCubeのプレハブ
        /// </summary>
        private GameObject[] _ghostMoveCbSmalls;
        /// <summary>ゴーストのMoveCube移動速度</summary>
        [SerializeField] float localPathDuration = 4f;
        /// <summary>実行中のSequenceを途中で止めるため</summary>
        private Sequence _runningSeqence;
        /// <summary>現在の方向</summary>
        private Vector3[] _currentDirection;
        /// <summary>現在の方向</summary>
        public Vector3[] CurrentDirection => _currentDirection;
        /// <summary>空間操作の基準値</summary>
        private Transform _spaceOwnerTransform;
        /// <summary>MoveCubeの初期状態</summary>
        private ObjectsOffset[] _cubeOffsets;
        /// <summary>MoveCubeの初期状態</summary>
        public ObjectsOffset[] CubeOffsets => _cubeOffsets;
        /// <summary>
        /// ゴーストのMoveCubeのローカルPath
        /// ステージ１       [0]
        /// ステージ２前半   [1]～[3]
        /// ステージ２後半   [4]～[6]
        /// ステージ３       []～[]
        /// </summary>
        [SerializeField] private LocalPathPositionsMap[] positionMap;

        /// <summary>
        /// ローカルPathを二次元配列にするためのシリアライズ
        /// </summary>
        [System.Serializable]
        public class LocalPathPositionsMap
        {
            /// <summary>ゴーストキューブのローカルPath</summary>
            [SerializeField] private Vector3[] localPathPositions;
            /// <summary>ゴーストキューブのローカルPath</summary>
            public Vector3[] LocalPathPositions => localPathPositions;
        }

        public bool Initialize()
        {
            try
            {
                _currentDirection = new Vector3[2];
                _spaceOwnerTransform = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.transform;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool ManualStart()
        {
            try
            {
                _ghostMoveCbSmalls = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_MOVECUBEGHOST);
                _cubeOffsets = LevelDesisionIsObjected.SaveObjectOffset(_ghostMoveCbSmalls);
                for (var i = 0; i < _ghostMoveCbSmalls.Length; i++)
                    _ghostMoveCbSmalls[i].transform.GetComponent<Renderer>().material.DOFade(0f, .1f);

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
                if (_runningSeqence != null)
                    _runningSeqence.Kill();
                _currentDirection = new Vector3[2];
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool PlayEnvironment(EnvironmentIndex index, float durationTime)
        {
            try
            {
                switch (index)
                {
                    case EnvironmentIndex.SpaceController:
                        _ghostMoveCbSmalls[0].transform.GetComponent<Renderer>().material.DOFade(.75f, durationTime);
                        _runningSeqence = GetSequenceMoveGhostCube(new GameObject[1] { _ghostMoveCbSmalls[0] }
                            , new LocalPathPositionsMap[1] { positionMap[0] }
                            , _ghostMoveCbSmalls[0].transform
                            , positionMap[0].LocalPathPositions
                            , localPathDuration);

                        break;
                    case EnvironmentIndex.SpaceConnect:
                        for (var i = 0; i < 3; i++)
                            _ghostMoveCbSmalls[i].transform.GetComponent<Renderer>().material.DOFade(.75f, durationTime);
                        _runningSeqence = GetSequenceMoveGhostCube(new GameObject[3] { _ghostMoveCbSmalls[0], _ghostMoveCbSmalls[1], _ghostMoveCbSmalls[2] }
                            , new LocalPathPositionsMap[3] { positionMap[1], positionMap[2], positionMap[3] }
                            , _ghostMoveCbSmalls[0].transform
                            , positionMap[1].LocalPathPositions
                            , localPathDuration);

                        break;
                    case EnvironmentIndex.SpaceDirection:
                        for (var i = 3; i < 6; i++)
                            _ghostMoveCbSmalls[i].transform.GetComponent<Renderer>().material.DOFade(.75f, durationTime);
                        _runningSeqence = GetSequenceMoveGhostCube(new GameObject[3] { _ghostMoveCbSmalls[3], _ghostMoveCbSmalls[4], _ghostMoveCbSmalls[5] }
                            , new LocalPathPositionsMap[3] { positionMap[4], positionMap[5], positionMap[6] }
                            , _ghostMoveCbSmalls[3].transform
                            , positionMap[4].LocalPathPositions
                            , localPathDuration);

                        break;
                    case EnvironmentIndex.SpaceRecycle:
                        break;
                    default:
                        throw new System.Exception("チュートリアルエンバイロメント呼び出しの例外");
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
        /// ゴーストキューブのDOTweenシーケンスを取得
        /// </summary>
        /// <param name="ghostMoveCubes">対象のゴースト</param>
        /// <param name="localPathPositions">移動パス</param>
        /// <param name="watchingTarget">基準ゴースト</param>
        /// <param name="watchingPathPositions">基準移動パス</param>
        /// <param name="moveDuration">移動速度</param>
        /// <returns>ゴーストキューブのDOTweenシーケンス</returns>
        private Sequence GetSequenceMoveGhostCube(GameObject[] ghostMoveCubes, LocalPathPositionsMap[] localPathPositions, Transform watchingTarget, Vector3[] watchingPathPositions, float moveDuration)
        {
            // 操作反映先のUI（左右）を指定する
            var dire = new IntReactiveProperty(-1);
            dire.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        case (int)Direction2D.Left:
                            _currentDirection[(int)Direction2D.Right] = Vector3.zero;
                            break;
                        case (int)Direction2D.Right:
                            _currentDirection[(int)Direction2D.Left] = Vector3.zero;
                            break;
                        default:
                            // -1の場合
                            _currentDirection[(int)Direction2D.Left] = Vector3.zero;
                            _currentDirection[(int)Direction2D.Right] = Vector3.zero;
                            break;
                    }
                });

            // ゴーストが存在する空間情報を管理
            var cnt = new IntReactiveProperty(0);
            cnt.ObserveEveryValueChanged(x => x.Value)
                .Where(x => 0 < x)
                .Subscribe(x =>
                {
                    if (x < localPathPositions[0].LocalPathPositions.Length)
                        if (-1 < dire.Value)
                            _currentDirection[dire.Value] = UpdateFixedValue(watchingPathPositions[x] - watchingPathPositions[x - 1]);
                });

            // シーケンス作成と初期処理
            var seqence = DOTween.Sequence();
            seqence.SetLink(gameObject)
                .SetLoops(-1, LoopType.Restart)
                .OnUpdate(() => dire.Value = CheckPositionAndGetDirection2D(watchingTarget.localPosition))
                ;

            // 移動パスを元にゴーストを自動操作
            for (var i = 0; i < localPathPositions[0].LocalPathPositions.Length; i++)
            {
                if (i == 0)
                {
                    seqence.AppendCallback(() =>
                    {
                        _currentDirection[(int)Direction2D.Left] = Vector3.zero;
                        _currentDirection[(int)Direction2D.Right] = Vector3.zero;
                        cnt.Value = 0;
                    });
                    for (var j = 0; j < ghostMoveCubes.Length; j++)
                        seqence = ConnectDOLocalMoveSequenceAppendOrJoin(seqence, j, ghostMoveCubes[j].transform, localPathPositions[j].LocalPathPositions[i], .1f);
                }
                else
                {
                    seqence.AppendCallback(() => cnt.Value++);
                    for (var j = 0; j < ghostMoveCubes.Length; j++)
                        seqence = ConnectDOLocalMoveSequenceAppendOrJoin(seqence, j, ghostMoveCubes[j].transform, localPathPositions[j].LocalPathPositions[i], moveDuration);
                }
            }
            return seqence;
        }

        /// <summary>
        /// DOLocalMoveのシーケンス追加
        /// 複数パスを同時に動かす場合は、最初のみAppendとしてそれ以降は並列（Join）に動かす
        /// </summary>
        /// <param name="sequence">シーケンス</param>
        /// <param name="counter">配列の何番目か</param>
        /// <param name="ghostMoveCubeTransform">ゴースト</param>
        /// <param name="pathPosition">移動パス</param>
        /// <param name="duration">移動速度</param>
        /// <returns>追加されたDOLocalMoveのシーケンス</returns>
        private Sequence ConnectDOLocalMoveSequenceAppendOrJoin(Sequence sequence, int counter, Transform ghostMoveCubeTransform, Vector3 pathPosition, float duration)
        {
            if (counter == 0)
                return sequence.Append(ghostMoveCubeTransform.DOLocalMove(pathPosition, duration));
            else
                return sequence.Join(ghostMoveCubeTransform.DOLocalMove(pathPosition, duration));
        }

        /// <summary>
        /// 対象の位置が左空間 or 右空間に存在するかをチェック
        /// </summary>
        /// <param name="targetPosition">対象のポジション</param>
        /// <returns>左右の方向（0なら-1）</returns>
        private int CheckPositionAndGetDirection2D(Vector3 targetPosition)
        {
            if (targetPosition.x < _spaceOwnerTransform.localPosition.x)
            {
                return (int)Direction2D.Left;
            }
            else if (_spaceOwnerTransform.localPosition.x < targetPosition.x)
            {
                return (int)Direction2D.Right;
            }
            return -1;
        }

        /// <summary>
        /// ベクターの各値が一定の範囲（1.0f）を超えたら1.0fへ固定
        /// </summary>
        /// <param name="value">ベクター情報</param>
        /// <returns>更新後のベクター情報</returns>
        private Vector3 UpdateFixedValue(Vector3 value)
        {
            var x = 1.0f < Mathf.Abs(value.x) ? 1.0f * (0f < value.x ? 1f : -1f) : value.x;
            var y = 1.0f < Mathf.Abs(value.y) ? 1.0f * (0f < value.y ? 1f : -1f) : value.y;
            var z = 1.0f < Mathf.Abs(value.z) ? 1.0f * (0f < value.z ? 1f : -1f) : value.z;
            return new Vector3(x, y, z);
        }

        public bool StopEnvironment(EnvironmentIndex index, float durationTime)
        {
            try
            {
                if (_runningSeqence != null)
                    _runningSeqence.Kill();

                switch (index)
                {
                    case EnvironmentIndex.SpaceController:
                        _currentDirection = new Vector3[2];
                        _ghostMoveCbSmalls[0].transform.GetComponent<Renderer>().material.DOFade(0f, durationTime);

                        break;
                    case EnvironmentIndex.SpaceConnect:
                        _currentDirection = new Vector3[2];
                        for (var i = 0; i < 3; i++)
                            _ghostMoveCbSmalls[i].transform.GetComponent<Renderer>().material.DOFade(0f, durationTime);

                        break;
                    case EnvironmentIndex.SpaceDirection:
                        _currentDirection = new Vector3[2];
                        for (var i = 3; i < 6; i++)
                            _ghostMoveCbSmalls[i].transform.GetComponent<Renderer>().material.DOFade(0f, durationTime);

                        break;
                    case EnvironmentIndex.SpaceRecycle:
                        break;
                    default:
                        throw new System.Exception("チュートリアルエンバイロメント呼び出しの例外");
                }

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
