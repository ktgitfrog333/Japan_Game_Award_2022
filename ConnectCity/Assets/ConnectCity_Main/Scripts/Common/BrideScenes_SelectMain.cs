using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Common
{
    /// <summary>
    /// シーン間のデータ管理（Select_SceneとMain_Scene）
    /// </summary>
    public class BrideScenes_SelectMain : MonoBehaviour
    {
        private static BrideScenes_SelectMain instance;
        public static BrideScenes_SelectMain Instance { get { return instance; } }

        private void Awake()
        {
            if (null != instance)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            instance = this;
        }
        /// <summary>読み込ませるシーン</summary>
        private string _loadSceneName;
        /// <summary>読み込ませるステージID</summary>
        private int _loadSceneId;
        public int LoadSceneId
        {
            get
            {
                return _loadSceneId;
            }
            set
            {
                _loadSceneId = value;
            }
        }

        public int MyProperty { get; set; }

        /// <summary>
        /// メインシーンを次のシーンへセット
        /// Select_Sceneから呼び出される想定の処理
        /// </summary>
        /// <param name="sceneId"></param>
        public void SetMainSceneNameIdFromSelect_Scene(int sceneId)
        {
            _loadSceneName = "Main_Scene";
            _loadSceneId = sceneId;
        }

        /// <summary>
        /// シーンロード開始
        /// </summary>
        public void PlayLoadScene()
        {
            SceneManager.sceneLoaded += LoadedGameScene;
            SceneManager.LoadScene(_loadSceneName);
        }

        /// <summary>
        /// 次のシーンにあるオブジェクトへ値を渡す
        /// </summary>
        /// <param name="next"></param>
        /// <param name="mode"></param>
        private void LoadedGameScene(Scene next, LoadSceneMode mode)
        {
            //SceneInfoManager.Instance.UpdateScenesMap(_loadSceneId);
            // シーン移動の度に実行されないように消す
            SceneManager.sceneLoaded -= LoadedGameScene;
        }
    }
}
