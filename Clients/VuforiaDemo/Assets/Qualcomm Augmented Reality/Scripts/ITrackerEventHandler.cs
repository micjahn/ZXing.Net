/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

// Interface for handling tracker events.
public interface ITrackerEventHandler
{
    // Called after all the trackable objects have been updated:
    void OnTrackablesUpdated();
}
