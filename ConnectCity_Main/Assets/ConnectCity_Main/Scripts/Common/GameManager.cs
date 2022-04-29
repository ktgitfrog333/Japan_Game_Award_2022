using Main.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Main.Common.Const.TagConst;
using System.Threading.Tasks;
using Main.Level;
using Main.Common.LevelDesign;

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
        /// <summary>T.B.D 重力操作ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] gravityControllers;
        /// <summary>T.B.D 敵ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] humanEnemies;
        /// <summary>T.B.D 敵ギミックの初期状態</summary>
        private ObjectsOffset[] _humanEnemieOffsets;
        /// <summary>T.B.D 敵ギミックの初期状態</summary>
        public ObjectsOffset[] HumanEnemieOffsets => _humanEnemieOffsets;
        /// <summary>T.B.D レーザー砲ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] turretEnemies;
        /// <summary>T.B.D ぼろいブロックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] brokenCubes;
        /// <summary>T.B.D ぼろいブロックの初期状態</summary>
        private ObjectsOffset[] _brokenCubeOffsets;
        /// <summary>T.B.D ぼろいブロックの初期状態</summary>
        public ObjectsOffset[] BrokenCubeOffsets => _brokenCubeOffsets;

        private void Reset()
        {
            if (player == null)
                player = GameObject.FindGameObjectsWithTag(TAG_NAME_PLAYER);
            if (spaceManager == null)
                spaceManager = GameObject.FindGameObjectWithTag(TAG_NAME_SPACEMANAGER);
            if (mainCamera == true)
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            // T.B.D 重力操作ギミックの仮実装
            //if (gravityControllers == null && gravityControllers.length)
            //    gravityControllers = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
            // T.B.D 敵ギミックの仮実装
            //if (humanEnemies == null && humanEnemies.length)
            //    humanEnemies = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
            // T.B.D レーザー砲ギミックの仮実装
            //if (turretEnemies == null && turretEnemies.length)
            //    turretEnemies = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
            // T.B.D ぼろいブロックの仮実装
            //if (brokenCubes == null || (brokenCubes != null && 0 == brokenCubes.Length))
            //    brokenCubes = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
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
            // T.B.D 敵ギミックの仮実装
            //_humanEnemieOffsets = LevelDesisionIsObjected.SaveObjectOffset(humanEnemies);
            //if (_humanEnemieOffsets == null)
            //    Debug.LogError("敵ギミック初期状態の保存の失敗");
            // T.B.D ぼろいブロックの仮実装
            //_brokenCubeOffsets = LevelDesisionIsObjected.SaveObjectOffset(brokenCubes);
            //if (_brokenCubeOffsets == null)
            //    Debug.LogError("ぼろいブロック初期状態の保存の失敗");
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
        public bool MoveHumanEnemyFromSpaceManager(Vector3 moveVelocity)
        {
            // T.B.D 敵オブジェクトを動かす処理を入れる
            return true;
        }

        /// <summary>
        /// T.B.D 重力操作ギミックの仮実装(GameManager.Instance.MoveCharactorFromGravityController(3.5fなど);)
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
        public async Task<bool> DeadPlayerFromHumanEnemies()
        {
            return await player[SceneInfoManager.Instance.SceneIdCrumb.Current].GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }

        /// <summary>
        /// プレイヤーの操作禁止フラグを切り替え
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
        public bool DestroyHumanEnemyFromSpaceManager()
        {
            // T.B.D 敵ギミック破壊処理を呼ぶ
            return true;
        }
    }
}
