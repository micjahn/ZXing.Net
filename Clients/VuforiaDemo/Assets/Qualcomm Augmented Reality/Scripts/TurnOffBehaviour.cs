/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

// A utility behaviour to disable rendering of a game object at run time.
public class TurnOffBehaviour : MonoBehaviour
{

    #region UNITY_MONOBEHAVIOUR_METHODS

    void Awake()
    {
        // We remove the mesh components at run-time only, but keep them for
        // visualization when running in the editor:
#if !UNITY_EDITOR
        MeshRenderer targetMeshRenderer = this.GetComponent<MeshRenderer>();
        Destroy(targetMeshRenderer);
        MeshFilter targetMesh = this.GetComponent<MeshFilter>();
        Destroy(targetMesh);
#endif
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

}