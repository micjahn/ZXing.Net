/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class QCAR
{
    #region NESTED

    // InitError is an error value that is returned by QCAR if something goes
    // wrong at initialization.
    public enum InitError
    {
        // Device is not supported by QCAR.
        INIT_DEVICE_NOT_SUPPORTED = -2,
        // Another (unknown) initialization error has occured.
        INIT_ERROR = -1,
        // Everything is fine
        INIT_SUCCESS = 0
    }

    // Hints are used to push the tracker into certain behavior. Hints (as the
    // name suggests) are not always guaranteed to work.
    public enum QCARHint
    {
        // Specify the number of Image Targets that are handled by the tracker
        // at once.
        HINT_MAX_SIMULTANEOUS_IMAGE_TARGETS = 0,
        // Detection of Trackables is continued in the next frame if they could
        // not be found within a certain time interval.
        HINT_IMAGE_TARGET_MULTI_FRAME_ENABLED = 1,
        // Specifies the maximum time the detector should look for Trackables.
        HINT_IMAGE_TARGET_MILLISECONDS_PER_MULTI_FRAME = 2,
    }

    #endregion // NESTED



    #region PUBLIC_METHODS

    // Retrieves initialization error code or success
    public static InitError CheckInitializationError()
    {
        return (InitError)getInitErrorCode();
    }


    // Checks if the GL surface has changed
    public static bool IsRendererDirty()
    {
        return isRendererDirty() == 1;
    }


    // Sets a hint for the QCAR SDK
    // Hints help the SDK to understand the developer's needs.
    // However, depending on the device or SDK version the hints
    // might not be taken into consideration.
    // Returns false if the hint is unknown or deprecated.
    public static bool SetHint(QCARHint hint, int value)
    {
        return qcarSetHint((int)hint, value) == 1;
    }


    // Indicates whether the rendering surface needs to support an alpha channel
    // for transparency
    public static bool RequiresAlpha()
    {
        return qcarRequiresAlpha() == 1;
    }


    // Returns the QCAR projection matrix
    public static Matrix4x4 GetProjectionGL(float nearPlane, float farPlane, ScreenOrientation screenOrientation)
    {
        float[] projMatrixArray = new float[16];
        IntPtr projMatrixPtr = Marshal.AllocHGlobal(
                    Marshal.SizeOf(typeof(float)) * projMatrixArray.Length);

        getProjectionGL(nearPlane, farPlane, projMatrixPtr,
                    (int)screenOrientation);

        Marshal.Copy(projMatrixPtr, projMatrixArray, 0, projMatrixArray.Length);
        Matrix4x4 projMatrix = Matrix4x4.identity;
        for (int i = 0; i < 16; i++)
            projMatrix[i] = projMatrixArray[i];

        Marshal.FreeHGlobal(projMatrixPtr);

        return projMatrix;
    }


    // Sets the Unity version for internal use
    public static void SetUnityVersion()
    {
        int major  = 0;
        int minor  = 0;
        int change = 0;

        // Use non-numeric values as tokens for split
        string versionPattern = "[^0-9]";

        // Split Unity version string into multiple parts
        string[] unityVersionBits = Regex.Split(Application.unityVersion,
                                                versionPattern);

        // Sanity check if nothing went wrong
        if (unityVersionBits.Length >= 3)
        {
            major  = int.Parse(unityVersionBits[0]);
            minor  = int.Parse(unityVersionBits[1]);
            change = int.Parse(unityVersionBits[2]);
        }

        setUnityVersionNative(major, minor, change);
    }

    #endregion // PUBLIC_METHODS



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int getInitErrorCode();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int isRendererDirty();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarSetHint(int hint, int value);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarRequiresAlpha();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int getProjectionGL(float nearClip, float farClip,
                                    [In, Out]IntPtr projMatrix,
                                    int screenOrientation);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void setUnityVersionNative(int major, int minor,
                                    int change);

#else

    private static int getInitErrorCode() { return 0; }


    private static int isRendererDirty() { return 0; }


    private static int qcarSetHint(int hint, int value) { return 0; }


    private static int qcarRequiresAlpha() { return 0; }


    private static int getProjectionGL(float nearClip, float farClip,
                                    [In, Out]IntPtr projMatrix,
                                    int screenOrientation) { return 0; }


    private static void setUnityVersionNative(int major, int minor,
                                    int change) { }

#endif

    #endregion // NATIVE_FUNCTIONS

}
