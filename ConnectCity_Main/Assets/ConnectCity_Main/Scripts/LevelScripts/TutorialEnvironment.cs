using Main.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;

namespace Main.Level
{
    /// <summary>
    /// チュートリアルのエンバイロメント
    /// </summary>
    public class TutorialEnvironment : MonoBehaviour, IGameManager, ITutorialOwnerEnvironment
    {
        public bool Initialize()
        {
            try
            {
                // T.B.D エンバイロメントを実装する
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
                // T.B.D エンバイロメントを実装する
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool PlayEnvironment(EnvironmentIndex index)
        {
            try
            {
                // T.B.D エンバイロメントを実装する
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool StopEnvironment(EnvironmentIndex index)
        {
            try
            {
                // T.B.D エンバイロメントを実装する
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
