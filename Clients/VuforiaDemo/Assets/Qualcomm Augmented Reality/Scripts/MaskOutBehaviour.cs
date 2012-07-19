/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

// Helper behaviour used to hide augmented objects behind the video background.
public class MaskOutBehaviour : MonoBehaviour
{

    #region PUBLIC_MEMBER_VARIABLES

    public Material maskMaterial;

    #endregion // PUBLIC_MEMBER_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS

    void Start ()
    {
        this.renderer.sharedMaterial = maskMaterial;
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS
}
