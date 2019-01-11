using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;


public class HandTrackingBallControl : MagicLeap.HandTrackingVisualizer {

    public BallControl ballControl;

    Vector3 pointerTip;
    Vector3 thumbTip;
    MLHandKeyPose previousPose;
    public bool currentlyClicked;

    // Use this for initialization
    void Start () {
        base.Start();


        ballControl = FindObjectOfType<BallControl>();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
        if (Hand.Index.KeyPoints.Count > 0)
        {
            pointerTip = _indexFinger[0].position;
        }
        if (Hand.Index.KeyPoints.Count > 0)
        {
            thumbTip = _thumb[0].position;
        }

        ballControl.transform.position = new Vector3(
            (pointerTip.x + thumbTip.x) / 2,
            (pointerTip.y + thumbTip.y) / 2,
            (pointerTip.z + thumbTip.z) / 2);


        if (Hand.KeyPose == MLHandKeyPose.Pinch && previousPose != MLHandKeyPose.Pinch)
        {
            Debug.Log("start click");
            ballControl.StartClick();
            currentlyClicked = true;
        }
        if (Hand.KeyPose != MLHandKeyPose.Pinch && previousPose == MLHandKeyPose.Pinch)
        {
            Debug.Log("stop click");
            ballControl.StopClick();
            currentlyClicked = false;
        }

        previousPose = Hand.KeyPose;
    }
}
