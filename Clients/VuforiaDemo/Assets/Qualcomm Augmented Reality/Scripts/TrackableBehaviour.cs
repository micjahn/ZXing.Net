/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;


// The base class behaviour for all trackable types in QCAR.
public abstract class TrackableBehaviour : MonoBehaviour
{    
    #region NESTED

    // The supported trackable types.
    public enum TrackableType
    {
        UNKNOWN_TYPE,       ///< A trackable of unknown type
        IMAGE_TARGET,       ///< A trackable of ImageTarget type
        MULTI_TARGET,       ///< A trackable of MultiTarget type
        MARKER,             ///< A trackable of Marker type
    }

    // The tracking status of the trackable.
    public enum Status
    {
        UNKNOWN,
        UNDEFINED,
        NOT_FOUND,
        DETECTED,
        TRACKED
    }

    #endregion //NESTED



    #region PROPERTIES

    // The name of the trackable.
    public string TrackableName
    {
        get
        {
            return mTrackableName;
        }

        set
        {
            mTrackableName = value;
        }
    }

    // The unique id for all trackable objects.
    public int TrackableID
    {
        get { return mTrackableID; }
    }

    // The tracking status of the trackable.
    public Status CurrentStatus
    {
        get { return mStatus; }
    }

    #endregion // PROPERTIES



    #region PUBLIC_MEMBER_VARIABLES

    [HideInInspector]
    public TrackableType mTrackableType = TrackableType.IMAGE_TARGET;
    
    #endregion // PUBLIC_MEMBER_VARIABLES



    #region PROTECTED_MEMBER_VARIABLES

    [SerializeField]
    [HideInInspector]
    protected string mTrackableName = "";

    [SerializeField]
    [HideInInspector]
    public Vector3 mPreviousScale = Vector3.one;

    protected int mTrackableID = -1;
    protected Status mStatus = Status.UNKNOWN;
    
    #endregion // PROTECTED_MEMBER_VARIABLES



    #region PRIVATE_MEMBER_VARIABLES

    private List<ITrackableEventHandler> mTrackableEventHandlers =
                                new List<ITrackableEventHandler>();

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region EDITOR_ONLY_MEMBER_VARIABLES

    [HideInInspector]
    public bool mPreserveChildSize = false;

    [HideInInspector]
    public bool mInitializedInEditor = false;

    #endregion // EDITOR_ONLY_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // This method registers a new Tracker event handler at the Tracker.
    // These handlers are called as soon as ALL Trackables have been updated
    // in this frame.
    public void RegisterTrackableEventHandler(
                                ITrackableEventHandler trackableEventHandler)
    {
        mTrackableEventHandlers.Add(trackableEventHandler);
    }


    // This method unregisters a Tracker event handler.
    // Returns "false" if event handler does not exist.
    public bool UnregisterTrackableEventHandler(
                                ITrackableEventHandler trackableEventHandler)
    {
        return mTrackableEventHandlers.Remove(trackableEventHandler);
    }


    // Is triggered by the TrackerBehavior after it has updated.
    public void OnTrackerUpdate(Status newStatus)
    {
        // Update status:
        Status prevStatus = mStatus;
        mStatus = newStatus;

        if (prevStatus != newStatus)
        {
            foreach (ITrackableEventHandler handler in mTrackableEventHandlers)
            {
                handler.OnTrackableStateChanged(prevStatus, newStatus);
            }
        }
    }


    // Initializes the trackable ID. Should only be called by the
    // QCARBehaviour on initialization.
    public void InitializeID(int id)
    {
        mTrackableID = id;
    }


    // Scales Trackable uniformly
    public virtual bool CorrectScale()
    {
        return false;
    }

    #endregion // PUBLIC_METHODS



    #region UNITY_MONOBEHAVIOUR_METHODS

    // Overriding standard Unity MonoBehaviour methods.

    public void Start()
    {
        // Note: Empty, but this forces the enabled checkbox in the editor
        // to become visible.
    }


    public void OnDisable()
    {
        // Update status:
        Status prevStatus = mStatus;
        mStatus = Status.NOT_FOUND;

        if (prevStatus != Status.NOT_FOUND)
        {
            foreach (ITrackableEventHandler handler in mTrackableEventHandlers)
            {
                handler.OnTrackableStateChanged(prevStatus, Status.NOT_FOUND);
            }
        }
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS
}
