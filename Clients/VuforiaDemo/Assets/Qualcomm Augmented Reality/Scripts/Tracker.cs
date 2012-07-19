/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

public abstract class Tracker
{
    #region NESTED

    // Enumeration of the different tracker types
    public enum Type
    {
        IMAGE_TRACKER,    // Tracks ImageTargets and MultiTargets
        MARKER_TRACKER    // Tracks Markers
    }

    #endregion // NESTED



    #region PUBLIC_METHODS

    // Starts the Tracker
    public abstract bool Start();

    // Stops the Tracker
    public abstract void Stop();

    #endregion // PUBLIC_METHODS



    #region PROTECTED_METHODS

    // Position the camera relative to a Trackable.
    protected void PositionCamera(TrackableBehaviour trackableBehaviour,
                                  Camera arCamera,
                                  QCARManager.PoseData camToTargetPose)
    {
        arCamera.transform.localPosition =
                trackableBehaviour.transform.rotation *
                Quaternion.AngleAxis(90, Vector3.left) *
                Quaternion.Inverse(camToTargetPose.orientation) *
                (-camToTargetPose.position) +
                trackableBehaviour.transform.position;

        arCamera.transform.rotation =
                trackableBehaviour.transform.rotation *
                Quaternion.AngleAxis(90, Vector3.left) *
                Quaternion.Inverse(camToTargetPose.orientation);
    }    
    
    // Position a Trackable relative to the Camera.
    protected void PositionTrackable(TrackableBehaviour trackableBehaviour,
                                     Camera arCamera,
                                     QCARManager.PoseData camToTargetPose)
    {
        trackableBehaviour.transform.position =
                arCamera.transform.TransformPoint(camToTargetPose.position);

        trackableBehaviour.transform.rotation =
                arCamera.transform.rotation *
                camToTargetPose.orientation *
                Quaternion.AngleAxis(270, Vector3.left);
    }

    #endregion // PROTECTED_METHODS
}