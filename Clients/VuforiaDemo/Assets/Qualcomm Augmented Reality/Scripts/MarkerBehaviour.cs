/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

// A trackable behaviour for representing rectangular markers.
public class MarkerBehaviour : TrackableBehaviour
{

    #region PROPERTIES

    // The marker ID (as opposed to the trackable's ID)
    public int MarkerID
    {
        get
        {
            return mMarkerID;
        }
        set
        {
            mMarkerID = value;
        }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES
    
    [SerializeField]
    [HideInInspector]
    private int mMarkerID;
    
    #endregion // PRIVATE_MEMBER_VARIABLES



    #region CONSTRUCTION

    public MarkerBehaviour()
    {
        // Remove as soon as this is solved by type
        mTrackableType = TrackableType.MARKER;
        mMarkerID = -1;
    }

    #endregion // CONSTRUCTION



    #region PUBLIC_METHODS

    // Returns the size of this target in scene units:
    public Vector2 GetSize()
    {
        return new Vector2(transform.localScale.x, transform.localScale.y);
    }


    // Scales the Trackable uniformly
    public override bool CorrectScale()
    {
        bool scaleChanged = false;

        for (int i = 0; i < 3; ++i)
        {
            // Force uniform scale:
            if (this.transform.localScale[i] != this.mPreviousScale[i])
            {
                this.transform.localScale =
                    new Vector3(this.transform.localScale[i],
                                this.transform.localScale[i],
                                this.transform.localScale[i]);

                this.mPreviousScale = this.transform.localScale;
                scaleChanged = true;
                break;
            }
        }

        return scaleChanged;
    }

    #endregion // PUBLIC_METHODS
}
