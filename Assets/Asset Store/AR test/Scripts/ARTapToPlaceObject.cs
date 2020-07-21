using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] Transform objectToPlace;
    [SerializeField] Transform placementIndicator;
    [SerializeField] Camera arCam;

    ARSessionOrigin arOrigin;
    ARRaycastManager arRaycaster;
    Pose placementPose;
    bool placementPosIsValid = false;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycaster = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        UpdatePlacementPos();
        UpdatePlacementIndicator();

        if (placementPosIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation, transform);
    }

    private void UpdatePlacementIndicator()
    {
        placementIndicator.gameObject.SetActive(placementPosIsValid);
        if (placementPosIsValid)
        {
            placementIndicator.SetPositionAndRotation(placementPose.position, placementIndicator.rotation);
        }
    }

    private void UpdatePlacementPos()
    {
        Vector2 screenCenter = arCam.ViewportToScreenPoint(Vector2.one/2f);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        arRaycaster.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPosIsValid = hits.Count > 0;
        if (placementPosIsValid)
        {
            placementPose = hits[0].pose;
            Vector3 camForward = arCam.transform.forward;
            Vector3 camBearing = new Vector3(camForward.x, 0f, camForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(camBearing);
        }
    }
}
