using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    //Easy acces to Controller
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private GameObject collidingObject;
    private GameObject objectInHand;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    private void SetCollidingObject(Collider col)
    {
        // Checks if there is an object being held and that the object has a rigidbody
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // set this object as collidingObject
        collidingObject = col.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // When the trigger is pressed down and an object is in range grab it
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // When the trigger is released and there is an object attached to the controller release it
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }

    // When the controller enters an object set this object to grab
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // When a player hold the controller in the object for a longer time this can fail or get buggy without this
    public void OnTriggerStay(Collider other)
    {
       SetCollidingObject(other);
    }

    // When you exit the trigger set the colliding
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        // Set the colliding object to the controller
        objectInHand = collidingObject;
        collidingObject = null;
        // Create a joint that connects the controller to the object
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // Add a joint that doesn't breake easily
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        // check for a fixed joint
        if (GetComponent<FixedJoint>())
        {
            // Remove the connection en verwijder de joint
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // Add the speed an rotation of the controller to the object
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // Remove the refenrence to the attached object
        objectInHand = null;
    }

    public void ReleaseObject(GameObject deleted)
    {
        if (deleted = objectInHand)
            ReleaseObject();
    }
}
