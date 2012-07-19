/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class QCARRenderer
{
    #region NESTED

    // This struct stores 2D integer vectors.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vec2I
    {
        public int x;
        public int y;
        
        public Vec2I(int v1, int v2)
        {
            x = v1;
            y = v2;
        }
    }

    // This struct stores Video Background configuration data. It stores if
    // background rendering is enabled, if it happens synchronously and it
    // stores position and size of the video background on the screen.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VideoBGCfgData
    {
        public int enabled;
        public int synchronous;
        public Vec2I position;
        public Vec2I size;
    }

    // Describes the size of the texture in the graphics unit as well as
    // the size of the image inside the texture. The latter corresponds
    // to the size of the image delivered by the camera
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VideoTextureInfo
    {
        public Vec2I textureSize;
        public Vec2I imageSize;
    }

    #endregion // NESTED



    #region PROPERTIES

    // Returns an instance of a QCARRenderer (thread safe)
    public static QCARRenderer Instance
    {
        get
        {
            // Make sure only one instance of CameraDevice is created.
            if (mInstance == null)
            {
                lock (typeof(QCARRenderer))
                {
                    if (mInstance == null)
                    {
                        mInstance = new QCARRenderer();
                    }
                }
            }
            return mInstance;
        }
    }


    // True to have QCAR render the video background image natively
    // False to bind the video background to the texture set in
    // QCARRenderer.SetVideoBackgroundTextureID
    public bool DrawVideoBackground
    {
        // Forward to QCAR Manager:
        set { QCARManager.Instance.DrawVideoBackground = value; }
        get { return QCARManager.Instance.DrawVideoBackground; }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBERS

    private static QCARRenderer mInstance = null;

    #endregion // PRIVATE_MEMBERS



    #region PUBLIC_METHODS

	// Retrieves the current layout configuration of the video background.
    public VideoBGCfgData GetVideoBackgroundConfig()
    {
        IntPtr configPtr = Marshal.AllocHGlobal(
                            Marshal.SizeOf(typeof(VideoBGCfgData)));
        rendererGetVideoBackgroundCfg(configPtr);
        VideoBGCfgData config = (VideoBGCfgData) Marshal.PtrToStructure
                            (configPtr, typeof(VideoBGCfgData));
        Marshal.FreeHGlobal(configPtr);

        return config;
    }


	// Configures the layout of the video background (location on the screen
    // and size).
    public void SetVideoBackgroundConfig(VideoBGCfgData config)
    {
        IntPtr configPtr = Marshal.AllocHGlobal(
                            Marshal.SizeOf(typeof(VideoBGCfgData)));
        Marshal.StructureToPtr(config, configPtr, true);
        rendererSetVideoBackgroundCfg(configPtr);
        Marshal.FreeHGlobal(configPtr);
    }


    // Tells QCAR where the texture id to use for updating video
    // background data
    public bool SetVideoBackgroundTextureID(int textureID)
    {
        return rendererSetVideoBackgroundTextureID(textureID) != 0;
    }


	// Check if video background info is available
    public bool IsVideoBackgroundInfoAvailable()
    {
        return rendererIsVideoBackgroundTextureInfoAvailable() != 0;
    }


	// Returns the texture info associated with the current video background
    public VideoTextureInfo GetVideoTextureInfo()
    {
        IntPtr ptr = Marshal.AllocHGlobal(
                            Marshal.SizeOf(typeof(VideoTextureInfo)));

        rendererGetVideoBackgroundTextureInfo(ptr);

        VideoTextureInfo info = (VideoTextureInfo)Marshal.PtrToStructure
                        (ptr, typeof(VideoTextureInfo));

        Marshal.FreeHGlobal(ptr);
        return info;
    }

    #endregion // PUBLIC_METHODS




    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererSetVideoBackgroundCfg(
                                    [In, Out]IntPtr bgCfg);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererGetVideoBackgroundCfg(
                                    [In, Out]IntPtr bgCfg);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void rendererGetVideoBackgroundTextureInfo(
                                    [In, Out]IntPtr texInfo);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int rendererSetVideoBackgroundTextureID(
                            int textureID);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int rendererIsVideoBackgroundTextureInfoAvailable();

#else

    private static void rendererSetVideoBackgroundCfg(
                                    [In, Out]IntPtr bgCfg) { }


    private static void rendererGetVideoBackgroundCfg(
                                    [In, Out]IntPtr bgCfg) { }

    private static void rendererGetVideoBackgroundTextureInfo(
                                [In, Out]IntPtr texInfo) { }

    private static int rendererSetVideoBackgroundTextureID(int textureID)
                                    { return 0; }

    private static int rendererIsVideoBackgroundTextureInfoAvailable() { return 0; }

#endif

    #endregion // NATIVE_FUNCTIONS
}
