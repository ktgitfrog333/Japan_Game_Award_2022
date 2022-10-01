using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitleSelect
{
    /// <summary>
    /// セレクトシーンのゲームマネージャー
    /// </summary>
    public class GameManagerSelect : MonoBehaviour
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
            bgmPlay.GetComponent<BgmPlay>().PlayBGM(ClipToPlayBGM.Title_No1);
        }
    }
}
