/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MarkerTracker : Tracker
{
    #region PRIVATE_MEMBER_VARIABLES

    // Dictionary that contains Markers.
    private Dictionary<int, MarkerBehaviour> mMarkerBehaviourDict =
        new Dictionary<int, MarkerBehaviour>();

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // Starts the tracker.
    // The tracker needs to be stopped before Trackables can be modified.
    public override bool Start()
    {
        if (markerTrackerStart() == 0)
        {
            Debug.LogError("Could not start tracker.");
            return false;
        }

        return true;
    }


    // Stops the tracker.
    // The tracker needs to be stopped before Trackables can be modified.
    public override void Stop()
    {
        markerTrackerStop();
    }


    // Creates a marker with the given id, name, and size.
    // Registers the marker at native code.
    // Returns a MarkerBehaviour object to receive updates.
    public MarkerBehaviour CreateMarker(int markerID, String trackableName, float size)
    {
        int trackableID = RegisterMarker(markerID, trackableName, size);

        if (trackableID == -1)
        {
            Debug.LogError("Could not create marker with id " + markerID + ".");
            return null;
        }

        // Alternatively instantiate Trackable Prefabs.
        GameObject markerObject = new GameObject();
        MarkerBehaviour newMB =
            markerObject.AddComponent<MarkerBehaviour>();

        Debug.Log("Creating Marker with values: " +
                  "\n MarkerID:     " + markerID +
                  "\n TrackableID:  " + trackableID +
                  "\n Name:         " + trackableName +
                  "\n Size:         " + size + "x" + size);

        newMB.InitializeID(trackableID);
        newMB.MarkerID = markerID;
        newMB.TrackableName = trackableName;
        newMB.transform.localScale = new Vector3(size, size, size);

        // Add newly created Marker to dictionary.
        mMarkerBehaviourDict[trackableID] = newMB;

        return newMB;
    }


    // Creates a marker with the given id, name, and size.
    // Registers the marker at native code.
    // Returns the unique trackable id.
    public int RegisterMarker(int markerID, String trackableName, float size)
    {
        int result = markerTrackerCreateMarker(markerID, trackableName, size);

        // Tell QCARManager to reinitialize its trackable array.
        QCARManager.Instance.Reinit();

        return result;
    }


    // Destroys the marker associated with the given MarkerBehaviour
    // at native code.
    public bool DestroyMarker(MarkerBehaviour marker, bool destroyGameObject)
    {
        if (markerTrackerDestroyMarker(marker.TrackableID) == 0)
        {
            Debug.LogError("Could not destroy marker with id " + marker.MarkerID + ".");
            return false;
        }

        mMarkerBehaviourDict.Remove(marker.TrackableID);

        if (destroyGameObject)
        {
            GameObject.Destroy(marker.gameObject);
        }

        // Tell QCARManager to reinitialize its trackable array.
        QCARManager.Instance.Reinit();

        return true;
    }


    // Returns the total number of markers registered at native.
    public int GetNumMarkers()
    {
        return markerTrackerGetNumMarkers();
    }


    // Returns the Marker at the given index.
    public MarkerBehaviour GetMarker(int index)
    {
        Dictionary<int, MarkerBehaviour>.Enumerator enumerator =
            mMarkerBehaviourDict.GetEnumerator();

        MarkerBehaviour mb = null;

        for (int i = 0; i <= index; ++i)
        {
            if (!enumerator.MoveNext())
            {
                Debug.LogError("Marker index invalid.");
                mb = null;
                break;
            }

            mb = enumerator.Current.Value;
        }

        return mb;
    }


    // Returns the Marker with the given unique ID.
    // Unique IDs for Markers are created when markers are registered.
    public bool TryGetMarkerByID(int id,
                                 out MarkerBehaviour marker)
    {
        return mMarkerBehaviourDict.TryGetValue(id, out marker);
    }


    public void AddMarkers(MarkerBehaviour[] markerBehaviours)
    {
        foreach (MarkerBehaviour marker in markerBehaviours)
        {
            if (marker.TrackableName == null)
            {
                Debug.LogError("Found Marker without name.");
                continue;
            }

            int id = RegisterMarker(marker.MarkerID, marker.TrackableName, marker.GetSize().x);

            if (id == -1)
            {
                Debug.LogWarning("Marker named " + marker.TrackableName +
                                 " could not be registered, disabling.");
                marker.enabled = false;
            }
            else
            {
                marker.InitializeID(id);
                if (!mMarkerBehaviourDict.ContainsKey(id))
                {
                    mMarkerBehaviourDict[id] = marker;
                    Debug.Log("Found Marker named " + marker.TrackableName +
                                " with id " + marker.TrackableID);
                }
                marker.enabled = true;
            }
        }
    }


    public void DestroyAllMarkers(bool destroyGameObject)
    {
        int numMarkers = GetNumMarkers();

        for (int i = 0; i < numMarkers; i++)
        {
            MarkerBehaviour marker = GetMarker(i);
            if (markerTrackerDestroyMarker(marker.TrackableID) == 0)
            {
                Debug.LogError("Could not destroy marker with id " + marker.MarkerID + ".");
            }

            if (destroyGameObject)
            {
                GameObject.Destroy(marker.gameObject);
            }
        }

        mMarkerBehaviourDict.Clear();

        // Tell QCARManager to reinitialize its trackable array.
        QCARManager.Instance.Reinit();
    }


    public void RemoveDisabledTrackablesFromQueue(ref LinkedList<int> trackableIDs)
    {
        LinkedListNode<int> idNode = trackableIDs.First;

        while (idNode != null)
        {
            LinkedListNode<int> next = idNode.Next;

            MarkerBehaviour markerBehaviour;
            if (TryGetMarkerByID(idNode.Value, out markerBehaviour))
            {
                if (markerBehaviour.enabled == false)
                {
                    trackableIDs.Remove(idNode);
                }
            }
            idNode = next;
        }
    }


    public void UpdateCameraPose(Camera arCamera,
                                 QCARManager.TrackableData[] trackableDataArray,
                                 int originTrackableID)
    {
        // If there is a World Center Trackable use it to position the camera.
        if (originTrackableID >= 0)
        {
            foreach (QCARManager.TrackableData trackableData in trackableDataArray)
            {
                if (trackableData.id == originTrackableID)
                {
                    if (trackableData.status ==
                        TrackableBehaviour.Status.DETECTED
                        || trackableData.status ==
                        TrackableBehaviour.Status.TRACKED)
                    {
                        MarkerBehaviour originTrackable = null;
                        if (TryGetMarkerByID(originTrackableID,
                            out originTrackable))
                        {
                            if (originTrackable.enabled)
                            {
                                PositionCamera(originTrackable, arCamera,
                                               trackableData.pose);
                            }
                        }
                    }
                    break;
                }
            }
        }
    }


    // Method used to update poses of all active Markers
    // in the scene
    public void UpdateTrackablePoses(Camera arCamera,
                            QCARManager.TrackableData[] trackableDataArray,
                            int originTrackableID)
    {
        foreach (QCARManager.TrackableData trackableData in trackableDataArray)
        {
            // For each Trackable data struct from native
            MarkerBehaviour markerBehaviour;
            if (TryGetMarkerByID(trackableData.id, out markerBehaviour))
            {
                // If this is the world center skip it, we never move the
                // world center Trackable in the scene
                if (trackableData.id == originTrackableID)
                {
                    continue;
                }

                if ((trackableData.status ==
                        TrackableBehaviour.Status.DETECTED
                        || trackableData.status ==
                        TrackableBehaviour.Status.TRACKED) &&
                        markerBehaviour.enabled)
                {
                    // The Trackable object is visible and enabled,
                    // move it into position in relation to the camera
                    // (which we moved earlier)
                    PositionTrackable(markerBehaviour, arCamera,
                                      trackableData.pose);
                }
            }
        }

        // Update each Trackable
        // Do this once all Trackables have been moved into place
        foreach (QCARManager.TrackableData trackableData in trackableDataArray)
        {
            MarkerBehaviour markerBehaviour;
            if (TryGetMarkerByID(trackableData.id, out markerBehaviour))
            {
                if (markerBehaviour.enabled)
                {
                    markerBehaviour.OnTrackerUpdate(
                                            trackableData.status);
                }
            }
        }
    }

    #endregion // PUBLIC_METHODS



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerTrackerStart();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void markerTrackerStop();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerTrackerCreateMarker(int id, String trackableName, float size);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerTrackerDestroyMarker(int trackableId);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerTrackerGetNumMarkers();

#else // !UNITY_EDITOR

    private static int markerTrackerStart() { return 0; }

    private static void markerTrackerStop() { }

    private static int markerTrackerCreateMarker(int id, String trackableName, float size) { return 0; }

    private static int markerTrackerDestroyMarker(int trackableIndex) { return 0; }

    private static int markerTrackerGetNumMarkers() { return 0; }

#endif // !UNITY_EDITOR

    #endregion // NATIVE_FUNCTIONS
}