/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

public class QCARRuntimeUtilities
{
    #region PUBLIC_METHODS

    // Returns the file name without the path.
    public static string StripFileNameFromPath(string fullPath)
    {
        string[] pathParts = fullPath.Split(new char[] { '/' });
        string fileName = pathParts[pathParts.Length - 1];

        return fileName;
    }


    // Returns the extension without the path.
    public static string StripExtensionFromPath(string fullPath)
    {
        string[] pathParts = fullPath.Split(new char[] { '.' });

        // Return empty string if there is no extension.
        if (pathParts.Length <= 1)
        {
            return "";
        }

        string extension = pathParts[pathParts.Length - 1];

        return extension;
    }

    #endregion // PUBLIC_METHODS
}