using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SwitchOnOFFBlock : MonoBehaviour
{
    /// <summary>
    /// ブロックの有効／無効ステータス（有効ならtrue、無効ならfalse）
    /// </summary>
    [SerializeField] private bool isEnabled;
    /// <summary>
    /// 有効状態のフェード値
    /// </summary>
    [SerializeField] private float enabledEndValue;
    /// <summary>
    /// 無効状態のフェード値
    /// </summary>
    [SerializeField] private float disabledEndValue;
    /// <summary>
    /// フェードアニメーションの遷移時間
    /// </summary>
    [SerializeField] private float doFadeDuration;
    private bool _defaultIsEnabled; //有効／無効ステータスの初期値
    private Renderer _renderer; //オブジェクトのマテリアル等の情報
    private BoxCollider _collider;  //コライダー

    public bool Initialize()
    {
        try
        {
            _defaultIsEnabled = isEnabled;
            _renderer = GetComponent<Renderer>();
            _collider = GetComponent<BoxCollider>();
            return true;
        }
        catch(System.Exception)
        {
            throw;
        }
    }

    public bool Exit()
    {
        try
        {
            isEnabled = _defaultIsEnabled;
            CheckOnOffState();
            return true;
        }
        catch(System.Exception)
        {
            throw;
        }
    }

    public void CheckOnOffState()
    {
        if(isEnabled == true)
        {
            _collider.enabled = true;
            this._renderer.DOFade(endValue: enabledEndValue, duration: doFadeDuration);
        }
        else if(isEnabled == false)
        {
            _collider.enabled = false;
            this._renderer.DOFade(endValue: disabledEndValue, duration: doFadeDuration);
        }
    }

    public bool UpdateOnOffState()
    {
        try
        {
            if(isEnabled == true)
            {
                isEnabled = false;
            }
            else
            {
                isEnabled = true;
            }
            return true;
        }
        catch(System.Exception)
        {
            throw;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
