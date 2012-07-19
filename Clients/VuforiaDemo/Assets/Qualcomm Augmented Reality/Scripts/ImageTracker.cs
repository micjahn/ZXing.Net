/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ImageTracker : Tracker
{
    #region PRIVATE_MEMBER_VARIABLES

    private DataSet mActiveDataSet = null;
    private List<DataSet> mDataSets = new List<DataSet>();

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // Starts the tracker.
    // The tracker must have loaded a dataset before it can start.
    // The tracker needs to be stopped before Trackables can be modified.
    public override bool Start()
    {
        if (imageTrackerStart() == 0)
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
        imageTrackerStop();
    }


    // Creates a new empty dataset.
    public DataSet CreateDataSet()
    {
        IntPtr dataSetPtr = imageTrackerCreateDataSet();
        if (dataSetPtr == IntPtr.Zero)
        {
            Debug.LogError("Could not create dataset.");
            return null;
        }

        DataSet dataSet = new DataSet(dataSetPtr);
        mDataSets.Add(dataSet);

        return dataSet;
    }


    // Destroy the given dataset.
    // Returns false if the given dataset is active.
    public bool DestroyDataSet(DataSet dataSet, bool destroyTrackables)
    {
        if (dataSet == null)
        {
            Debug.LogError("Dataset is null.");
            return false;
        }

        if (destroyTrackables)
        {
            DestroyTrackables(dataSet);
        }

        if (imageTrackerDestroyDataSet(dataSet.DataSetPtr) == 0)
        {
            Debug.LogError("Could not destroy dataset.");
            return false;
        }

        mDataSets.Remove(dataSet);

        return true;
    }


    // Activates the given dataset.
    // If another dataset is currently active, then this call fails as the
    // other dataset needs to be explicitly deactivated first.
    // Datasets can only be activated when the tracker is not working.
    public bool ActivateDataSet(DataSet dataSet)
    {
        if (dataSet == null)
        {
            Debug.LogError("Dataset is null.");
            return false;
        }

        if (imageTrackerActivateDataSet(dataSet.DataSetPtr) == 0)
        {
            Debug.LogError("Could not activate dataset.");
            return false;
        }

        QCARManager.Instance.Reinit();

        // Activate all Trackables.
        int numTrackables = dataSet.GetNumTrackables();
        for (int i = 0; i < numTrackables; ++i)
        {
            TrackableBehaviour trackable = dataSet.GetTrackable(i);
            if (trackable != null)
            {
                trackable.enabled = true;
            }
        }

        mActiveDataSet = dataSet;
        return true;
    }


    // Deactivates the given dataset.
    // This can only be done when the tracker is not running.
    public bool DeactivateDataSet(DataSet dataSet)
    {
        if (dataSet == null)
        {
            Debug.LogError("Dataset is null.");
            return false;
        }

        if (imageTrackerDeactivateDataSet(dataSet.DataSetPtr) == 0)
        {
            Debug.LogError("Could not deactivate dataset.");
            return false;
        }

        // Deactivate all Trackables.
        int numTrackables = dataSet.GetNumTrackables();
        for (int i = 0; i < numTrackables; ++i)
        {
            TrackableBehaviour trackable = dataSet.GetTrackable(i);
            if (trackable != null)
            {
                trackable.enabled = false;
            }
        }

        mActiveDataSet = null;
        return true;
    }


    // Returns the currently active dataset.
    // Returns null if no dataset has been activated.
    public DataSet GetActiveDataSet()
    {
        return mActiveDataSet;
    }


    // Returns the dataset at the given index.
    public DataSet GetDataSet(int index)
    {
        if (index < 0 || index >= mDataSets.Count)
        {
            Debug.LogError("Index " + index + " out of range.");
            return null;
        }

        return mDataSets[index];
    }


    // Returns the total number of datasets.
    public int GetNumDataSets()
    {
        return mDataSets.Count;
    }


    // Deactivates the currently active dataset and
    // destroys all datasets
    public void DestroyAllDataSets(bool destroyTrackables)
    {
        if (mActiveDataSet != null)
        {
            DeactivateDataSet(mActiveDataSet);
        }

        for (int i = mDataSets.Count - 1; i >= 0; i--)
        {
            DestroyDataSet(mDataSets[i], destroyTrackables);
        }

        mDataSets.Clear();
    }


    public void RemoveDisabledTrackablesFromQueue(ref LinkedList<int> trackableIDs)
    {
        if (mActiveDataSet == null)
        {
            return;
        }

        LinkedListNode<int> idNode = trackableIDs.First;

        while (idNode != null)
        {
            LinkedListNode<int> next = idNode.Next;

            DataSetTrackableBehaviour trackableBehaviour;
            if (mActiveDataSet.TryGetTrackableByID(idNode.Value,
                                                   out trackableBehaviour))
            {
                if (trackableBehaviour.enabled == false)
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
        if (mActiveDataSet == null)
        {
            return;
        }

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
                        DataSetTrackableBehaviour originTrackable = null;
                        if (mActiveDataSet.TryGetTrackableByID(originTrackableID,
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


    // Method used to update poses of all active Image Targets
    // in the scene
    public void UpdateTrackablePoses(Camera arCamera,
                            QCARManager.TrackableData[] trackableDataArray,
                            int originTrackableID)
    {
        if (mActiveDataSet == null)
        {
            return;
        }

        foreach (QCARManager.TrackableData trackableData in trackableDataArray)
        {
            // For each Trackable data struct from native
            DataSetTrackableBehaviour trackableBehaviour;
            if (mActiveDataSet.TryGetTrackableByID(trackableData.id,
                                                   out trackableBehaviour))
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
                        trackableBehaviour.enabled)
                {
                    // The Trackable object is visible and enabled,
                    // move it into position in relation to the camera
                    // (which we moved earlier)
                    PositionTrackable(trackableBehaviour, arCamera,
                                      trackableData.pose);
                }
            }
        }

        // Update each Trackable
        // Do this once all Trackables have been moved into place
        foreach (QCARManager.TrackableData trackableData in trackableDataArray)
        {
            DataSetTrackableBehaviour trackableBehaviour;
            if (mActiveDataSet.TryGetTrackableByID(trackableData.id,
                                                   out trackableBehaviour))
            {
                if (trackableBehaviour.enabled)
                {
                    trackableBehaviour.OnTrackerUpdate(
                                            trackableData.status);
                }
            }
        }
    }


    // Update Virtual Button states.
    public void UpdateVirtualButtons(int numVirtualButtons, IntPtr virtualButtonPtr)
    {
        for (int i = 0; i < numVirtualButtons; i++)
        {
            IntPtr vbPtr = new IntPtr(virtualButtonPtr.ToInt32() + i *
                                Marshal.SizeOf(typeof(QCARManager.VirtualButtonData)));
            QCARManager.VirtualButtonData vbData = (QCARManager.VirtualButtonData)
                    Marshal.PtrToStructure(vbPtr, typeof(QCARManager.VirtualButtonData));

            VirtualButtonBehaviour vb = null;
            if (mActiveDataSet.TryGetVirtualButtonByID(vbData.id, out vb))
            {
                ImageTargetBehaviour it = vb.GetImageTarget();
                if (it != null && it.enabled && vb.enabled)
                {
                    vb.OnTrackerUpdated(vbData.isPressed > 0);
                }
            }
        }
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private void DestroyTrackables(DataSet dataSet)
    {
        int numTrackables = dataSet.GetNumTrackables();

        for (int i = 0; i < numTrackables; ++i)
        {
            DataSetTrackableBehaviour objToDestroy = dataSet.GetTrackable(i);
            if (objToDestroy != null)
            {
                GameObject.Destroy(objToDestroy.gameObject);
            }
        }
    }

    #endregion // PRIVATE_METHODS



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerStart();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void imageTrackerStop();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern IntPtr imageTrackerCreateDataSet();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerDestroyDataSet(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerActivateDataSet(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTrackerDeactivateDataSet(IntPtr dataSetPtr);

#else // !UNITY_EDITOR

    private static int imageTrackerStart() { return 0; }

    private static void imageTrackerStop() { }

    private static IntPtr imageTrackerCreateDataSet() { return IntPtr.Zero; }

    private static int imageTrackerDestroyDataSet(IntPtr dataSetPtr) { return 0; }

    private static int imageTrackerActivateDataSet(IntPtr dataSetPtr) { return 0; }

    private static int imageTrackerDeactivateDataSet(IntPtr dataSetPtr) { return 0; }

#endif // !UNITY_EDITOR

    #endregion // NATIVE_FUNCTIONS
}