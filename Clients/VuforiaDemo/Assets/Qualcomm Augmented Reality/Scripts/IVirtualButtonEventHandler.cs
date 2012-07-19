/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

// An interface for handling virtual button state changes.
public interface IVirtualButtonEventHandler
{
    // Called when the virtual button has just been pressed.
    void OnButtonPressed(VirtualButtonBehaviour vb);

    // Called when the virtual button has just been released.
    void OnButtonReleased(VirtualButtonBehaviour vb);
}
