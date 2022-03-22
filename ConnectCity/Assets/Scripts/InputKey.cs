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

    public enum Title_Status
    {
        Push_Button,
        Start_End,
        End_Confirm
    }

    Title_Status status;
    // Start is called before the first frame update
    void Start()
    {
        title_image = GameObject.Find("Title_Logo_Image");
        push_image = GameObject.Find("PushGameStart_Logo_Image");
        gamestart_image = GameObject.Find("GameStart_Logo_Image");
        gamefinish_image = GameObject.Find("GameFinish_Logo_Image");
        pause_pencil_image = GameObject.Find("Pause_Pencil_Logo_Image");
        gamefinish_check_image = GameObject.Find("GameFinish_Check_Logo_Image");
        yes_image = GameObject.Find("Yes_Logo_Image");
        no_image = GameObject.Find("No_Logo_Image");
        status = Title_Status.Push_Button;
        push_image.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch(push_image)
        {
            case Title_Status.Push_Button:
                if (Input.GetKeyDown(KeyCode.Space))
                { }
        }
    }
}
