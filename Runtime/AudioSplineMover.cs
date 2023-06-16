// This script was created by dpiltz@vfs.com and heavily plagerizes the stuff created by the guy who made these videos - https://www.youtube.com/watch?v=IGmkDpNSpB8&list=TLPQMDIwOTIwMjKWemA7Mug_cw&index=1 https://www.youtube.com/watch?v=PPqtdpE4GM0&t=8s 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSplineMover : MonoBehaviour
{
    //reference to the audio spline
    public AudioSpline audioSpline;
    //reference to the object this object should follow
    public Transform followObj;


    private Transform thisTransform;

    private void Start()
    {
        thisTransform = transform;
    }

    private void Update()
    {
        //find the point on the spline that is closest to follow obj (most likely this will be the player) and move this object to that point
        thisTransform.position = audioSpline.WhereOnSpline(followObj.position);
    }
}
