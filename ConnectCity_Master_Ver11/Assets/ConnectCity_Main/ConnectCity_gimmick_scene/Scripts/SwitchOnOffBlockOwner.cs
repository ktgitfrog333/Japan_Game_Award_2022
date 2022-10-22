using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Main.Common;
using UniRx;
using UniRx.Triggers;
using Main.Common.Const;

namespace Gimmick
{
    /// <summary>
    /// ON/OFFブロックのオーナー
    /// </summary>
    public class SwitchOnOffBlockOwner : MonoBehaviour, IOwner
    {
        /// <summary>
        /// ゲームオブジェクト「SwitchOnOFFBlock」を複数管理する配列オブジェクト
        /// </summary>
        private GameObject[] _switchOnOFFBlocks;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool ManualStart()
        {
            try
            {
                _switchOnOFFBlocks = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_SWITCHONOFFBLOCK);
                if (_switchOnOFFBlocks != null && 0 < _switchOnOFFBlocks.Length)
                    foreach (var child in _switchOnOFFBlocks)
                        if (!child.GetComponent<SwitchOnOffBlock>().ManualStart())
                            throw new System.Exception("SwitchOnOFFBlockOwner初期処理の失敗");
                this.FixedUpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        if (_switchOnOFFBlocks != null && 0 < _switchOnOFFBlocks.Length)
                            foreach (var child in _switchOnOFFBlocks)
                                child.GetComponent<SwitchOnOffBlock>().CheckOnOffState();
                    })
                    .AddTo(_compositeDisposable);

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
                if (_switchOnOFFBlocks != null && 0 < _switchOnOFFBlocks.Length)
                    foreach (var child in _switchOnOFFBlocks)
                        if (!child.GetComponent<SwitchOnOffBlock>().Exit())
                            throw new System.Exception("SwitchOnOFFBlockOwner終了処理の失敗");
                _compositeDisposable.Clear();

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// ON/OFFブロックのステータス変更
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool UpdateOnOffState()
        {
            try
            {
                if (_switchOnOFFBlocks != null && 0 < _switchOnOFFBlocks.Length)
                    foreach (var child in _switchOnOFFBlocks)
                        if (!child.GetComponent<SwitchOnOffBlock>().UpdateOnOffState())
                            throw new System.Exception("SwitchOnOFFBlockOwnerステータス変更の失敗");

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
