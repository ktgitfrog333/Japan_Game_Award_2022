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

namespace Main.Level
{
    /// <summary>
    /// チュートリアルのエンバイロメント
    /// </summary>
    public class TutorialEnvironment : MonoBehaviour, IOwner, ITutorialOwnerEnvironment
    {
        /// <summary>
        /// ゴーストのMoveCubeのプレハブ
        /// ステージ１   [0]
        /// ステージ２   [1]～[6]
        /// ステージ３   [7]～[12]
        /// </summary>
        private GameObject[] _ghostMoveCbSmalls;
        /// <summary>
        /// ゴーストのMoveCubeのローカルPath
        /// ステージ１   [0]～[2]
        /// ステージ２   []～[]
        /// ステージ３   []～[]
        /// </summary>
        //[SerializeField] private Vector3[] localPathPositions;
        /// <summary>ゴーストのMoveCube移動速度</summary>
        [SerializeField] float localPathDuration = 4f;
        /// <summary>実行中のDOPathを途中で止めるため</summary>
        private Tweener _runningDOPath;
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
        /// ステージ１   [0]～[2]
        /// ステージ２   []～[]
        /// ステージ３   []～[]
        /// </summary>
        [SerializeField] private LocalPathPositionsMap[] positionMap;

        /// <summary>
        /// ローカルPathを二次元配列にするためのシリアライズ
        /// </summary>
        [System.Serializable]
        public class LocalPathPositionsMap
        {
            /// <summary>ゴーストのMoveCubeのローカルPath</summary>
            [SerializeField] private Vector3[] localPathPositions;
            /// <summary>ゴーストのMoveCubeのローカルPath</summary>
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
                _ghostMoveCbSmalls[0].transform.GetComponent<Renderer>().material.DOFade(.75f, .1f);

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
                _runningDOPath.Kill();
                _currentDirection = new Vector3[2];
                _ghostMoveCbSmalls[0].transform.GetComponent<Renderer>().material.DOFade(1f, .1f);
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
                        Vector3[] path = positionMap[0].LocalPathPositions;
                        var ghostTran = _ghostMoveCbSmalls[0].transform;

                        _runningDOPath = ghostTran.DOLocalPath(path, localPathDuration)
                            .SetLink(gameObject)
                            .SetLoops(-1, LoopType.Restart)
                            .OnWaypointChange(x =>
                            {
                                if (x == 0)
                                {
                                    // 始めのポイント
                                    var firstPos = ghostTran.localPosition;
                                    var firstdire = CheckPositionAndGetDirection2D(firstPos);
                                    if (-1 < firstdire)
                                        _currentDirection[firstdire] = UpdateFixedValue(path[x] - firstPos);
                                }
                                else if (x < path.Length)
                                {
                                    // 途中の経由ポイント
                                    var dire = CheckPositionAndGetDirection2D(ghostTran.localPosition);
                                    if (-1 < dire)
                                        _currentDirection[dire] = UpdateFixedValue(path[x] - path[x - 1]);
                                }
                                else
                                {
                                    // 最後まで到達
                                }
                            });
                        break;
                    case EnvironmentIndex.SpaceConnect:
                        break;
                    case EnvironmentIndex.SpaceDirection:
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
                _runningDOPath.Kill();

                switch (index)
                {
                    case EnvironmentIndex.SpaceController:
                        _ghostMoveCbSmalls[0].transform.GetComponent<Renderer>().material.DOFade(0f, durationTime);

                        break;
                    case EnvironmentIndex.SpaceConnect:
                        break;
                    case EnvironmentIndex.SpaceDirection:
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
