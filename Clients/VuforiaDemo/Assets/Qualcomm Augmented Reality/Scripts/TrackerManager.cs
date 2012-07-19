/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;
using System.Runtime.InteropServices;

public class TrackerManager
{
    #region PROPERTIES

    // Returns an instance of a TrackerManager (thread safe)
    public static TrackerManager Instance
    {
        get
        {
            // Make sure only one instance of TrackerManager is created.
            if (mInstance == null)
            {
                lock (typeof(TrackerManager))
                {
                    if (mInstance == null)
                    {
                        mInstance = new TrackerManager();
                    }
                }
            }
            return mInstance;
        }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    private static TrackerManager mInstance = null;

    private ImageTracker mImageTracker = null;

    private MarkerTracker mMarkerTracker = null;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // Returns the instance of the given tracker type
    // See the Tracker base class for a list of available tracker classes.
    // This function will return null if the tracker of the given type has
    // not been initialized.
    public Tracker GetTracker(Tracker.Type trackerType)
    {
        if (trackerType == Tracker.Type.IMAGE_TRACKER)
        {
            return mImageTracker;
        }
        else if (trackerType == Tracker.Type.MARKER_TRACKER)
        {
            return mMarkerTracker;
        }
        else
        {
            Debug.LogError("Could not return tracker. Unknow tracker type.");
            return null;
        }
    }


    // Initializes the tracker of the given type
    // Initializing a tracker must not be done when the CameraDevice
    // is initialized or started. This function will return null if the
    // CameraDevice is currently initialized.
    public Tracker InitTracker(Tracker.Type trackerType)
    {
        if (Application.isEditor)
        {
            return null;
        }

        if (trackerManagerInitTracker((int)trackerType) == 0)
        {
            Debug.LogError("Could not initialize the tracker.");
            return null;
        }

        if (trackerType == Tracker.Type.IMAGE_TRACKER)
        {
            if (mImageTracker == null)
            {
                mImageTracker = new ImageTracker();
            }
            return mImageTracker;
        }
        else if (trackerType == Tracker.Type.MARKER_TRACKER)
        {
            if (mMarkerTracker == null)
            {
                mMarkerTracker = new MarkerTracker();
            }
            return mMarkerTracker;
        }
        else
        {
            Debug.LogError("Could not initialize tracker. Unknown tracker type.");
            return null;
        }
    }


    // Deinitializes the tracker of the given type and frees any resources
    // used by the tracker.
    // Deinitializing a tracker must not be done when the CameraDevice
    // is initialized or started. This function will return false if the
    // tracker of the given type has not been initialized or if the
    // CameraDevice is currently initialized.
    public bool DeinitTracker(Tracker.Type trackerType)
    {
        if (trackerManagerDeinitTracker((int)trackerType) == 0)
        {
            Debug.LogError("Could not deinitialize the tracker.");
            return false;
        }

        if (trackerType == Tracker.Type.IMAGE_TRACKER)
        {
            mImageTracker = null;
        }
        else if (trackerType == Tracker.Type.MARKER_TRACKER)
        {
            mMarkerTracker = null;
        }
        else
        {
            Debug.LogError("Could not deinitialize tracker. Unknown tracker type.");
            return false;
        }

        return true;
    }

    #endregion // PUBLIC_METHODS



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int trackerManagerInitTracker(int trackerType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int trackerManagerDeinitTracker(int trackerType);

#else // !UNITY_EDITOR

    private static int trackerManagerInitTracker(int trackerType) { return 0; }

    private static int trackerManagerDeinitTracker(int trackerType) { return 0; }

#endif // !UNITY_EDITOR

    #endregion // NATIVE_FUNCTIONS
}
