using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitleSelect
{
    /// <summary>
    /// タイトルシーンのゲームマネージャー
    /// </summary>
    public class GameManagerTitle : MonoBehaviour
    {
        /// <summary>BGMのオブジェクト</summary>
        [SerializeField] private GameObject bgmPlay;

        private void Reset()
        {
            if (bgmPlay == null)
                bgmPlay = GameObject.Find("BgmPlay");
        }

        void Start()
        {
            bgmPlay.GetComponent<BgmPlay>().PlayBGM(ClipToPlayBGM.Title_No3);
        }
    }
}
