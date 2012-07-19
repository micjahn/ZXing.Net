/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

public abstract class DataSetTrackableBehaviour : TrackableBehaviour
{
    #region PROPERTIES

    // The name of the data set the Trackable belongs to.
    // Please be aware that the data set name is not a unique identifier at runtime!
    public string DataSetName
    {
        get
        { 
            // Create the data set name from path.
            string nameWithExtension = QCARRuntimeUtilities.StripFileNameFromPath(mDataSetPath);

            string extension = QCARRuntimeUtilities.StripExtensionFromPath(mDataSetPath);

            int extensionLength = extension.Length;

            if (extensionLength > 0)
            {
                // Add "dot" if the file had an extension.
                ++extensionLength;

                return nameWithExtension.Remove(nameWithExtension.Length - extensionLength);
            }

            return nameWithExtension;
        }
    }

    // The path to the data set in the file system.
    // The path together with the storage type can be used as unique identifier.
    public string DataSetPath
    {
        get
        { 
            return mDataSetPath;
        }

        set
        {
            mDataSetPath = value;
        }
    }

    // The storage type that is used to store the data set.
    // The path together with the storage type can be used as unique identifier.
    public DataSet.StorageType DataSetStorageType
    {
        get
        {
            return mStorageType;
        }

        set
        {
            mStorageType = value;
        }
    }

    #endregion // PROPERTIES



    #region PROTECTED_MEMBER_VARIABLES

    [SerializeField]
    [HideInInspector]
    protected string mDataSetPath = "";

    [SerializeField]
    [HideInInspector]
    protected DataSet.StorageType mStorageType =
        DataSet.StorageType.STORAGE_APPRESOURCE;

    #endregion // PROTECTED_MEMBER_VARIABLES



    #region PROTECTED_METHODS

    // Checks if this object should be part of the given data set.
    public bool References(DataSet dataSet)
    {
        return (mDataSetPath == dataSet.Path &&
            mStorageType == dataSet.FileStorageType);
    }
 
    #endregion // PROTECTED_METHODS
}
