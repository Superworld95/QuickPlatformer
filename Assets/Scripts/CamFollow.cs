using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    /*
    Title: Unity camera follows player script
    Author: User on stackoverflow.com
    Date: 2021
    Code Version: 2021.3.0
    Availability: https://stackoverflow.com/questions/65816546/unity-camera-follows-player-script
    */
    public Transform playerToFollow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerToFollow.transform.position + new Vector3(0, 2, -16);
    }
}
