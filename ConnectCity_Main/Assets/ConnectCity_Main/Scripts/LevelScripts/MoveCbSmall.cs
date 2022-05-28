using Main.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Level
{
    /// <summary>
    /// MoveCubeオブジェクト
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class MoveCbSmall : MonoBehaviour
    {
        /// <summary>MoveCubeのアニメータ</summary>
        [SerializeField] private Animator _animator;
        /// <summary>パーティクルシステムの配列</summary>
        [SerializeField] private ParticleSystem[] _particleSystems;

        private void Reset()
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();
            if (_particleSystems.Length <= 0f)
            {
                var particleList = new List<ParticleSystem>();
                for (var i = 0; i < transform.childCount; i++)
                {
                    particleList.Add(transform.GetChild(i).GetComponent<ParticleSystem>());
                }
                if (particleList.Count < 1) Debug.LogError("パーティクルのセットが失敗");
                _particleSystems = particleList.ToArray();
            }
        }

        /// <summary>
        /// MoveCubeのアニメーションを再生
        /// </summary>
        /// <param name="moveSpeed">移動先</param>
        /// <returns>成功／失敗</returns>
        public bool PlayMoveCbAnimation(float moveSpeed)
        {
            _animator.SetFloat(MoveCbSmallAnimator.PARAMETERS_MOVESPEED, moveSpeed);
            return true;
        }

        /// <summary>
        /// 移動中に光をまとわせる
        /// 移動アニメーションで発火
        /// ※デッドロジックだが今Animatorを消すと影響範囲が広いため、消さずに残す
        /// </summary>
        public void OnAirHover()
        {
            // 移動SE
            //SfxPlay.Instance.PlaySFX(ClipToPlay.se_block_float);
            // 移動中にパーティクルで枠をつける
            //if (!_particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.activeSelf)
            //    _particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.SetActive(true);
            //_particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].Play();
        }

        /// <summary>
        /// 移動中に光をまとわせる
        /// 空間操作からの呼び出し
        /// </summary>
        public void OnAirHoverFromSpaceManager()
        {
            // 移動中にパーティクルで枠をつける
            if (!_particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.activeSelf)
                _particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.SetActive(true);
            if (!_particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].isPlaying)
                _particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].Play();
        }

        /// <summary>
        /// エフェクトの停止
        /// オブジェクト硬直アニメーションで発火
        /// ※デッドロジックだが今Animatorを消すと影響範囲が広いため、消さずに残す
        /// </summary>
        public void OnFreeze()
        {
            // T.B.D 止まった時にエフェクトを止める
            //if (_particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.activeSelf)
            //    _particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.SetActive(false);
        }

        /// <summary>
        /// エフェクトの停止
        /// 空間操作からの呼び出し
        /// </summary>
        public void OnFreezeFromSpaceManager()
        {
            // 止まった時にエフェクトを止める
            if (_particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].isPlaying)
                _particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].Stop();
            if (_particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.activeSelf)
                _particleSystems[(int)MoveCbSmallEffectIdx.MoveDust].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// MoveCubeのパラメータ
    /// </summary>
    public class MoveCbSmallAnimator
    {
        /// <summary>
        /// 移動速度のパラメータ
        /// </summary>
        public static readonly string PARAMETERS_MOVESPEED = "MoveSpeed";
    }
    /// <summary>
    /// MoveCubeのエフェクトで使用するエフェクト配列のインデックスを管理
    /// </summary>
    public enum MoveCbSmallEffectIdx
    {
        /// <summary>移動エフェクト</summary>
        MoveDust
    }
}
