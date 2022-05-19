using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    //LaserPointer objects
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    //Teleport Objects
    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    private bool shouldTeleport;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start ()
    {
        // Make an instance of the prefab
        laser = Instantiate(laserPrefab);
        // Saves it's transform
        laserTransform = laser.transform;

        // Make an instance of the prefab
        reticle = Instantiate(teleportReticlePrefab);
        // Saves it's transform
        teleportReticleTransform = reticle.transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // When the touchpad is held down
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            // Shoot a ray, on hit store the point and show the laser
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);

                // Show the teleport circle
                reticle.SetActive(true);
                // Set the circle to to right position
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                // Set ShouldTeleport true bc a location has been found
                shouldTeleport = true;
            }
        }
        else // if ht player released the touchpad hide the laser
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        //When the touchpad is released and teleport is active, teleport the player
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            Teleport();
        }

    }

    private void ShowLaser(RaycastHit hit)
    {
        // Set the laser active
        laser.SetActive(true);
        // Positions the laser between the controller and the point where the raycast hits
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // rotates the laser so it looks at the raycast hit
        laserTransform.LookAt(hitPoint);
        // Scale the laser so it fits between the two positions
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private void Teleport()
    {
        // set the flag to false when teleporting is in progress
        shouldTeleport = false;
        // hide the circle
        reticle.SetActive(false);
        // Calculate the difference between the cameraRig and the head
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // Set the y to 0 bc the calculation doesn't consider the vertical pos of the player's head
        difference.y = 0;
        // teleport the player to the position
        cameraRigTransform.position = hitPoint + difference;
    }
}
