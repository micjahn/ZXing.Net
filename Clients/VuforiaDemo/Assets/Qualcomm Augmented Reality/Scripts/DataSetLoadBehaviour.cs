/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;

public class DataSetLoadBehaviour : MonoBehaviour
{
    #region PROPERTIES

    public string DataSetToActivate
    {
        get
        {
            return mDataSetToActivate;
        }

        set
        {
            mDataSetToActivate = value;
        }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    [SerializeField]
    [HideInInspector]
    private string mDataSetToActivate;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_MEMBER_VARIABLES
    [SerializeField]
    [HideInInspector]
    public List<string> mDataSetsToLoad = new List<string>();
    #endregion // PUBLIC_MEMBER_VARIABLES



    #region UNITY_MONOBEHAVIOUR_METHODS

    void Awake()
    {
        if (Application.isEditor)
        {
            return;
        }
        
        if (TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER) == null)
        {
            TrackerManager.Instance.InitTracker(Tracker.Type.IMAGE_TRACKER);
        }

        if (mDataSetsToLoad.Count <= 0)
        {
            Debug.LogWarning("No data sets defined. Not loading any data sets.");
            return;
        }

        foreach (string dataSetName in mDataSetsToLoad)
        {
            if (!DataSet.Exists(dataSetName))
            {
                Debug.LogError("Data set " + dataSetName + " does not exist.");
                continue;
            }

            ImageTracker imageTracker = (ImageTracker)TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
            DataSet dataSet = imageTracker.CreateDataSet();

            if (!dataSet.Load(dataSetName))
            {
                Debug.LogError("Failed to load data set " + dataSetName + ".");
                continue;
            }

            // Activate the data set if it is the one specified in the editor.
            if (mDataSetToActivate == dataSetName)
            {
                imageTracker.ActivateDataSet(dataSet);
            }
        }
    }


    void OnDestroy()
    {
        // Note we do not destroy the dataset as this is handled by the
        // QCARBehaviour.
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

}
