using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class BallControl : MonoBehaviour {

    #region Variabls
    //public 
    public GameObject ballPrefab;

    //private 
    Gravity gravityGroup;
    public GameObject selectedBall;
    public GameObject grabbedBall;
    //constants
    const float grabStrength = 2;
    #endregion

    // Use this for initialization
    void Start () {
        gravityGroup = FindObjectOfType<Gravity>();

        //subscribe methods for controller
        MLInput.OnTriggerDown += HandleTriggerDown;
        MLInput.OnTriggerUp += HandleTriggerUp;
    }
    //unsubscribe from events
    private void OnDestroy()
    {
        MLInput.OnTriggerDown -= HandleTriggerDown;
        MLInput.OnTriggerUp -= HandleTriggerUp;
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == gravityGroup.gameObject.layer)
        {
            selectedBall = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        selectedBall = null;
    }

    //what to do when trigger is pressed
    void HandleTriggerDown(byte controllerId, float value)
    {
        if (selectedBall)
        {
            grabbedBall = selectedBall;
            StartCoroutine("MoveBall");
        }
        else
        {
            NewBall();
        }
    }

    //what to do when trigger is released
    void HandleTriggerUp(byte controllerId, float value)
    {
        grabbedBall = null;
        StopAllCoroutines();
    }


    void NewBall()
    {
        GameObject newBall = Instantiate(ballPrefab,gravityGroup.transform);
        newBall.transform.position = transform.position;
        newBall.GetComponent<Rigidbody>().mass = .01f;
        grabbedBall = newBall;
        StartCoroutine("InflateBall");
        StartCoroutine("MoveBall");
    }

    IEnumerator MoveBall()
    {
        while (true)
        {
            // Gets a vector that points from the this ball to the other.
            var heading = transform.position - grabbedBall.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            //calculate pull to other ball (in normalized direction) based on mass and distance)
            Vector3 relativePull = Vector3.zero;
            relativePull.x = Mathf.Clamp(direction.x * grabbedBall.GetComponent<Rigidbody>().mass * grabStrength * gravityGroup.gravitationalConstant * distance * distance, -gravityGroup.forceclamp, gravityGroup.forceclamp);
            relativePull.y = Mathf.Clamp(direction.y * grabbedBall.GetComponent<Rigidbody>().mass * grabStrength * gravityGroup.gravitationalConstant * distance * distance, -gravityGroup.forceclamp, gravityGroup.forceclamp);
            relativePull.z = Mathf.Clamp(direction.z * grabbedBall.GetComponent<Rigidbody>().mass * grabStrength * gravityGroup.gravitationalConstant * distance * distance, -gravityGroup.forceclamp, gravityGroup.forceclamp);
            grabbedBall.GetComponent<Rigidbody>().AddForce(relativePull);

            yield return null; 
        }
    }

    IEnumerator InflateBall()
    {
        while (true)
        {
            grabbedBall.GetComponent<Rigidbody>().mass += .01f;
            yield return null;
        }
    }

}
