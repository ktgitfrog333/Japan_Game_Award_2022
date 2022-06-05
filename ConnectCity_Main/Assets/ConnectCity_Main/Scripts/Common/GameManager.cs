using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common
{
    /// <summary>
    /// ゲームマネージャー
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>シーンのオーナー</summary>
        [SerializeField] private GameObject sceneOwner;
        /// <summary>シーンのオーナー</summary>
        public GameObject SceneOwner => sceneOwner;
        /// <summary>オーディオのオーナー</summary>
        [SerializeField] private GameObject audioOwner;
        /// <summary>オーディオのオーナー</summary>
        public GameObject AudioOwner => audioOwner;
        /// <summary>UIのオーナー</summary>
        [SerializeField] private GameObject uIOwner;
        /// <summary>UIのオーナー</summary>
        public GameObject UIOwner => uIOwner;
        /// <summary>レベルデザインのオーナー</summary>
        [SerializeField] private GameObject levelOwner;
        /// <summary>レベルデザインのオーナー</summary>
        public GameObject LevelOwner => levelOwner;

        /// <summary>クラス自身</summary>
        private static GameManager instance;
        /// <summary>シングルトンのインスタンス</summary>
        public static GameManager Instance => instance;

        private void Awake()
        {
            instance = this;
        }

        private void Reset()
        {
            if (sceneOwner == null)
                sceneOwner = GameObject.Find("SceneOwner");
            if (audioOwner == null)
                audioOwner = GameObject.Find("AudioOwner");
            if (uIOwner == null)
                uIOwner = GameObject.Find("UIOwner");
            if (levelOwner == null)
                levelOwner = GameObject.Find("LevelOwner");
        }
    }
}
