/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

// Interface for handling trackable state changes.
public interface ITrackableEventHandler
{
    // Called when the trackable state has changed.
    void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus);
}
