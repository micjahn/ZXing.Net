/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

// A trackable that is made up of multiple targets with a fixed spatial
// relation.
public class MultiTargetBehaviour : DataSetTrackableBehaviour
{
    #region CONSTRUCTION

    public MultiTargetBehaviour()
    {
        // Remove as soon as this is solved by type
        mTrackableType = TrackableType.MULTI_TARGET;
    }

    #endregion // CONSTRUCTION
}
