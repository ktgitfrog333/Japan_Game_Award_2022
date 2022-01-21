using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン管理クラス
/// </summary>
public class SceneInfoManager : MonoBehaviour
{
    private static SceneInfoManager instance;
    public static SceneInfoManager Instance { get { return instance; } }
    /// <summary>シーンマップ</summary>
    private Dictionary<string, string> _mapSceneName = new Dictionary<string, string>();
    /// <summary>シーン名一覧</summary>
    private string[] _sceneNameAry = {
        "Demo_Scene"
            ,"Demo2_Scene"
    };
    /// <summary>読み込ませるシーン</summary>
    private string _loadSceneName;

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

    private void Start()
    {
        // 遷移前のシーン情報がある場合は過去シーンとしてセット
        if (0 < _mapSceneName.Count && _mapSceneName.ContainsKey("Current"))
        {
            _mapSceneName["Old"] = _mapSceneName["Current"];
        }
        // 現在のシーン情報をセット
        _mapSceneName["Current"] = SceneManager.GetActiveScene().name;
        // 次のシーン情報をシーン一覧から検索してセット
        for (var i = 0; i < _sceneNameAry.Length; i++)
        {
            if (_sceneNameAry[i].Equals(_mapSceneName["Current"]) && (i + 1) < _sceneNameAry.Length)
            {
                // 次のシーンが存在する場合はセット
                _mapSceneName["Next"] = _sceneNameAry[i + 1];
            }
            else
            {
                /// 最終ステージシーンならひとまず現在のシーンをセット
                _mapSceneName["Next"] = _mapSceneName["Current"];
            }
        }
    }

    /// <summary>
    /// 現在のシーンを次のシーンへセット
    /// </summary>
    public void LoadSceneNameRedo()
    {
        _loadSceneName = _mapSceneName["Current"];
    }
    /// <summary>
    /// 次のシーンを次のシーンへセット
    /// </summary>
    public void LoadSceneNameNext()
    {
        _loadSceneName = _mapSceneName["Next"];
    }
    /// <summary>
    /// セレクトシーンを次のシーンへセット
    /// </summary>
    public void LoadSceneNameSelect()
    {
        _loadSceneName = "Select_Scene";
    }

    /// <summary>
    /// シーンロード開始
    /// </summary>
    public void PlayLoadScene()
    {
        SceneManager.LoadScene(_loadSceneName);
    }
}
