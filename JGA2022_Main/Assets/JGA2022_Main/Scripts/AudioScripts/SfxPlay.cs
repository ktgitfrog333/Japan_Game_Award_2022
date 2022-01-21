using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
/// <summary>
/// 効果音を再生するクラス
/// </summary>
public class SfxPlay : MonoBehaviour
{
    /// <summary>クラス自身</summary>
    private static SfxPlay instance;
    /// <summary>シングルトンのインスタンス</summary>
    public static SfxPlay Instance
    {
        get { return instance; }
    }

    /// <summary>オーディオソース</summary>
    [SerializeField] private AudioSource audioSource;
    /// <summary>効果音のクリップ</summary>
    [SerializeField] private AudioClip[] clip;

    private void Reset()
    {
        Initialize();
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

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    private void Initialize()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// 指定されたSEを再生する
    /// </summary>
    /// <param name="clipToPlay">SE</param>
    public void PlaySFX(ClipToPlay clipToPlay)
    {
        try
        {
            if ((int)clipToPlay <= (clip.Length - 1))
            {
                audioSource.clip = clip[(int)clipToPlay];

                // SEを再生
                audioSource.Play();
            }
        }
        catch (Exception e)
        {
            Debug.Log("対象のファイルが見つかりません:[" + clipToPlay + "]");
            Debug.Log(e);
        }
    }
}

/// <summary>
/// オーディオクリップリストのインデックス
/// </summary>
public enum ClipToPlay
{
    /// <summary>メニューを開く</summary>
    se_menu = 0,
    /// <summary>メニューを閉じる</summary>
    se_close = 1,
    /// <summary>項目の決定</summary>
    se_decided = 2,
    /// <summary>ゲームクリア</summary>
    me_game_clear = 3,
    /// <summary>ステージセレクト</summary>
    se_select = 4
}
