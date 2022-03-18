using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input : MonoBehaviour
{
    public GameObject title_image;
    public GameObject push_image;
    public GameObject gamestart_image;
    public GameObject gamefinish_image;
    public GameObject pause_pencil_image;
    public GameObject gamefinish_check_image;
    public GameObject yes_image;
    public GameObject no_image;
    // Start is called before the first frame update
    void Start()
    {
        title_image = GameObject.Find("Title_Logo_Image");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
