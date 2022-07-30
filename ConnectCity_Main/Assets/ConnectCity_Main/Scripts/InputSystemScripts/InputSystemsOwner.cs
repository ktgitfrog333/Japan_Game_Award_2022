using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UnityEngine.InputSystem;

namespace Main.InputSystem
{
    /// <summary>
    /// InputSystemのオーナー
    /// </summary>
    public class InputSystemsOwner : MonoBehaviour, IGameManager
    {
        /// <summary>プレイヤー用のインプットイベント</summary>
        [SerializeField] private InputPlayer inputPlayer;
        /// <summary>プレイヤー用のインプットイベント</summary>
        public InputPlayer InputPlayer => inputPlayer;
        /// <summary>UI用のインプットイベント</summary>
        [SerializeField] private InputUI inputUI;
        /// <summary>UI用のインプットイベント</summary>
        public InputUI InputUI => inputUI;
        /// <summary>Space用のインプットイベント</summary>
        [SerializeField] private InputSpace inputSpace;
        /// <summary>Space用のインプットイベント</summary>
        public InputSpace InputSpace => inputSpace;

        private void Reset()
        {
            inputPlayer = transform.GetChild(0).GetComponent<InputPlayer>();
            inputUI = transform.GetChild(1).GetComponent<InputUI>();
            inputSpace = transform.GetChild(2).GetComponent<InputSpace>();
        }

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}
