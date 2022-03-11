using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //[SerializeField] private ClipToPlay index;
    private void OnEnable()
    {
        //SfxPlay.Instance.PlaySFX(index);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        SfxPlay.Instance.PlaySFX(ClipToPlay.me_game_clear);
        UIManager.Instance.OpenClearScreen();
        gameObject.SetActive(false);
    }
}
