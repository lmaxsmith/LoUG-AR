using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class BallControl : MonoBehaviour {

    #region Variables
    //public 
    public GameObject ballPrefab;

    //private 
    Gravity gravityGroup;
    public GameObject selectedBall;
    public GameObject grabbedBall;
    Vector3 grabbedBallVelocity;
    Rigidbody thisRB;
    FixedJoint thisJoint;


    //constants
    public float grabStrength = 20;
    #endregion

    //temp
    public Vector3 relativePull = Vector3.zero;


    // Use this for initialization
    void Start () {
        gravityGroup = FindObjectOfType<Gravity>();
        thisRB = GetComponent<Rigidbody>();
        thisJoint = GetComponent<FixedJoint>();


        //subscribe methods for controller
        MLInput.OnTriggerDown += StartClick;
        MLInput.OnTriggerUp += StopClick;
        
    }
    //unsubscribe from events
    private void OnDestroy()
    {
        MLInput.OnTriggerDown -= StartClick;
        MLInput.OnTriggerUp -= StopClick;
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
    public void StartClick(byte controllerId, float value)
    {
        StartClick();
    }
    public void StartClick()
    {
        if (selectedBall)
        {
            grabbedBall = selectedBall;
            //StartCoroutine("MoveBall");
            thisJoint.connectedBody = grabbedBall.GetComponent<Rigidbody>();
        }
        else
        {
            NewBall();
        }

    }

    //what to do when trigger is released
    public void StopClick(byte controllerId, float value)
    {
        StopClick();
    }
    public void StopClick()
    {
        //shut it down
        grabbedBall = null;
        thisJoint = null;
        StopAllCoroutines();

    }


    void NewBall()
    {
        GameObject newBall = Instantiate(ballPrefab,gravityGroup.transform);
        newBall.transform.position = transform.position;
        newBall.GetComponent<Rigidbody>().mass = .0001f;
        grabbedBall = newBall;
        thisJoint.connectedBody = grabbedBall.GetComponent<Rigidbody>();

        StartCoroutine("InflateBall");
        //StartCoroutine("MoveBall");
    }

    IEnumerator MoveBall()
    {
        while (true)
        {

            //grabbedBall.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;

            // Gets a vector that points from the this ball to the other.
            var heading = transform.position - grabbedBall.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            //calculate pull to other ball (in normalized direction) based on mass and distance)
            
            relativePull.x = Mathf.Clamp(direction.x * grabStrength * gravityGroup.gravitationalConstant * distance * distance, -gravityGroup.forceclamp, gravityGroup.forceclamp);
            relativePull.y = Mathf.Clamp(direction.y * grabStrength * gravityGroup.gravitationalConstant * distance * distance, -gravityGroup.forceclamp, gravityGroup.forceclamp);
            relativePull.z = Mathf.Clamp(direction.z * grabStrength * gravityGroup.gravitationalConstant * distance * distance, -gravityGroup.forceclamp, gravityGroup.forceclamp);
            grabbedBall.GetComponent<Rigidbody>().AddForce(relativePull);

            //grabbedBall.GetComponent<Rigidbody>().MovePosition(transform.position);
            //grabbedBallVelocity = grabbedBall.GetComponent<Rigidbody>().velocity;

            yield return null; 
        }
    }

    IEnumerator InflateBall()
    {
        while (true)
        {
            grabbedBall.GetComponent<Rigidbody>().mass += .0001f;
            yield return null;
        }
    }

}
