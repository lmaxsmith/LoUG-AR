using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap;

public class Gravity : MonoBehaviour {

    #region Variables
    //public Variabls
    public float gravitationalConstant = 1f;
    public float forceclamp = 100;

    //private variables
    public Rigidbody[] gravityBallz;

    //constants
    
    #endregion

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate () {
        //Gather and size the balls
        gravityBallz = gameObject.GetComponentsInChildren<Rigidbody>();

        //cycle through each ball
        for (int i = 0; i < gravityBallz.Length; i++)
        {

            //cull balls before calculating
            if (Vector3.Distance(gravityBallz[i].transform.position, Camera.main.transform.position) > 100)
            {
                GameObject.Destroy(gravityBallz[i].gameObject);
                
            }

            //adjust ball size according to rigid body mass
            gravityBallz[i].transform.localScale = new Vector3(
                Mathf.Pow(gravityBallz[i].mass, 1f / 3f), Mathf.Pow(gravityBallz[i].mass, 1f / 3f), Mathf.Pow(gravityBallz[i].mass, 1f / 3f));


            ApplyGravity(gravityBallz[i]);
            
        }
	}

    

    void ApplyGravity(Rigidbody gravityBall)
    {
        //cycle through each other ball to calculate pull
        for (int j = 0; j < gravityBallz.Length; j++)
        {

            // Gets a vector that points from the this ball to the other.
            var heading = gravityBall.transform.position - gravityBallz[j].transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            //calculate pull to other ball (in normalized direction) based on mass and distance)
            Vector3 relativePull = Vector3.zero;
            relativePull.x = Mathf.Clamp(direction.x * gravityBall.mass * gravityBallz[j].mass * gravitationalConstant / distance / distance, -forceclamp, forceclamp);
            relativePull.y = Mathf.Clamp(direction.y * gravityBall.mass * gravityBallz[j].mass * gravitationalConstant / distance / distance, -forceclamp, forceclamp);
            relativePull.z = Mathf.Clamp(direction.z * gravityBall.mass * gravityBallz[j].mass * gravitationalConstant / distance / distance, -forceclamp, forceclamp);
            gravityBallz[j].AddForce(relativePull,ForceMode.Force);
            
        }
    }

    
}
