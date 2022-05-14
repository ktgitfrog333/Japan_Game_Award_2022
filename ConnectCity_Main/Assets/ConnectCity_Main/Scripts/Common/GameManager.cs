using Main.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Main.Common.Const.TagConst;
using System.Threading.Tasks;
using Main.Level;
using Main.Common.LevelDesign;
using System.Linq;
using Gimmick;

namespace Main.Common
{
    /// <summary>
    /// ゲームオブジェクト間の関数実行の指令はこのオブジェクトを仲介する
    /// T.B.D SFXやPauseScreenなどシングルトンにしてしまっている物はここに集約させる？
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>クラス自身</summary>
        private static GameManager instance;
        /// <summary>シングルトンのインスタンス</summary>
        public static GameManager Instance
        {
            get { return instance; }
        }
        private void Awake()
        {
            // シングルトンのため複数生成禁止
            if (null != instance)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        /// <summary>プレイヤーのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] player;
        /// <summary>プレイヤーのゲームオブジェクト</summary>
        public GameObject Player => player[SceneInfoManager.Instance.SceneIdCrumb.Current];
        /// <summary>プレイヤーの初期状態</summary>
        private ObjectsOffset[] _playerOffsets;
        /// <summary>プレイヤーの初期状態</summary>
        public ObjectsOffset[] PlayerOffsets => _playerOffsets;
        /// <summary>空間操作</summary>
        [SerializeField] private GameObject spaceManager;
        /// <summary>空間操作</summary>
        public GameObject SpaceManager => spaceManager;
        /// <summary>カメラ</summary>
        [SerializeField] private GameObject mainCamera;
        /// <summary>カメラ</summary>
        public GameObject MainCamera { get { return mainCamera; } }
        /// <summary>ゴールポイントのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] goalPoint;
        /// <summary>ゴールポイントのゲームオブジェクト</summary>
        public GameObject GoalPoint => goalPoint[SceneInfoManager.Instance.SceneIdCrumb.Current];
        /// <summary>T.B.D 重力操作ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] gravityControllers;
        /// <summary>敵ギミックのオーナー</summary>
        [SerializeField] private GameObject robotEnemiesOwner;
        /// <summary>敵ギミックのオーナー</summary>
        public GameObject RobotEnemiesOwner => robotEnemiesOwner;
        /// <summary>ぼろいブロック・天井のオーナー</summary>
        [SerializeField] private GameObject breakBlookOwner;
        /// <summary>ぼろいブロック・天井のオーナー</summary>
        public GameObject BreakBlookOwner => breakBlookOwner;

        /// <summary>T.B.D レーザー砲ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] turretEnemies;

        private void Reset()
        {
            if (player == null || player.Length == 0)
                player = LevelDesisionIsObjected.GetGameObjectsInLevelDesign("LevelDesign", "SceneInfoManager", TAG_NAME_PLAYER, true);
            if (spaceManager == null)
                spaceManager = GameObject.FindGameObjectWithTag(TAG_NAME_SPACEMANAGER);
            if (mainCamera == null)
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (goalPoint == null || goalPoint.Length == 0)
                goalPoint = LevelDesisionIsObjected.GetGameObjectsInLevelDesign("LevelDesign", "SceneInfoManager", TAG_NAME_GOALPOINT, true);
            if (breakBlookOwner == null)
                breakBlookOwner = GameObject.Find("BreakBlookOwner");
            if (robotEnemiesOwner == null)
                robotEnemiesOwner = GameObject.Find("RobotEnemiesOwner");

            //robotEnemies = GameObject.FindGameObjectsWithTag(TAG_NAME_ROBOT_EMEMY);
            // T.B.D レーザー砲ギミックの仮実装
            //if (turretEnemies == null && turretEnemies.length)
            //    turretEnemies = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
        }

        private void Start()
        {
            ManualStart();
        }

        /// <summary>
        /// 疑似スタート
        /// </summary>
        public void ManualStart()
        {
            _playerOffsets = LevelDesisionIsObjected.SaveObjectOffset(Player);
            if (_playerOffsets == null)
                Debug.LogError("オブジェクト初期状態の保存の失敗");
            if (!robotEnemiesOwner.GetComponent<RobotEnemiesOwner>().Initialize())
                Debug.Log("敵起動処理の失敗");
        }

        /// <summary>
        /// ゴールポイント初期処理
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool InitializeGoalPoint()
        {
            return GoalPoint.GetComponent<GoalPoint>().Initialize();
        }

        /// <summary>
        /// カウントダウン表示を更新
        /// SpaceManagerからの呼び出し
        /// </summary>
        /// <param name="count">コネクト回数</param>
        /// <param name="maxCount">クリア条件のコネクト必要回数</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateCountDownFromSpaceManager(int count, int maxCount)
        {
            return GoalPoint.GetComponent<GoalPoint>().UpdateCountDownFromGameManager(count, maxCount);
        }

        /// <summary>
        /// ドアを開く
        /// ゴール演出のイベント
        /// SpaceManagerからの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool OpenDoorFromSpaceManager()
        {
            return GoalPoint.GetComponent<GoalPoint>().OpenDoorFromGameManager();
        }

        /// <summary>
        /// ドアを閉める
        /// ゴール演出のイベント
        /// SpaceManagerからの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool CloseDoorFromSpaceManager()
        {
            return GoalPoint.GetComponent<GoalPoint>().CloseDoorFromGameManager();
        }

        /// <summary>
        /// 疑似スタートを発火させる
        /// SceneInfoManagerからの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool PlayManualStartFromSceneInfoManager()
        {
            ManualStart();
            return true;
        }

        /// <summary>
        /// 空間操作オブジェクトからプレイヤーオブジェクトへ
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorFromSpaceManager(Vector3 moveVelocity)
        {
            return Player.GetComponent<PlayerController>().MoveChatactorFromGameManager(moveVelocity);
        }

        /// <summary>
        /// 空間操作オブジェクトから敵ギミックオブジェクトへ
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveRobotEnemyFromSpaceManager(Vector3 moveVelocity, GameObject hitObject)
        {
            return hitObject.GetComponent<Robot_Enemy>().MoveChatactorFromGameManager(moveVelocity);
        }

        /// <summary>
        /// 重力操作ギミックオブジェクトからプレイヤーオブジェクトへ
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorFromGravityController(Vector3 moveVelocity)
        {
            return Player.GetComponent<PlayerController>().MoveChatactorFromGameManager(moveVelocity);
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// コネクトシステム処理からの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public async Task<bool> DeadPlayerFromSpaceManager()
        {
            return await Player.GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// 敵からの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public async Task<bool> DeadPlayerFromRobotEnemies()
        {
            return await player[SceneInfoManager.Instance.SceneIdCrumb.Current].GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }

        /// <summary>
        /// プレイヤーの操作禁止フラグを切り替え
        /// 空間操作オブジェクトからの呼び出し
        /// </summary>
        /// <param name="banFlag">操作禁止フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool SetBanPlayerFromSpaceManager(bool banFlag)
        {
            try
            {
                Player.GetComponent<PlayerController>().InputBan = banFlag;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// プレイヤーの操作禁止フラグを切り替え
        /// ゴールポイントからの呼び出し
        /// </summary>
        /// <param name="banFlag">操作禁止フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool SetBanPlayerFromGoalPoint(bool banFlag)
        {
            try
            {
                Player.GetComponent<PlayerController>().InputBan = banFlag;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// レーザー砲からの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public async Task<bool> DeadPlayerFromTurretEnemies()
        {
            return await player[SceneInfoManager.Instance.SceneIdCrumb.Current].GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }

        /// <summary>
        /// 敵ギミックを破壊する
        /// コネクトシステム処理からの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DestroyHumanEnemyFromSpaceManager(GameObject hitObject)
        {
            return hitObject.GetComponent<Robot_Enemy>().DeadPlayerFromGameManager();
        }
    }
}
