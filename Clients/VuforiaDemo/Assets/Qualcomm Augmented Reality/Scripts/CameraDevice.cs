/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class CameraDevice
{
    #region NESTED

    // The mode used for camera capturing and video rendering.
    // The camera device mode is set through the Unity inspector.
    public enum CameraDeviceMode
    {
        // Best compromise between speed and quality.
        MODE_DEFAULT = -1,
        // Optimize for speed. Quality of the video background could suffer.
        MODE_OPTIMIZE_SPEED = -2,
        // Optimize for quality. Application performance could go down.
        MODE_OPTIMIZE_QUALITY = -3
    }

    public enum FocusMode
    {
        FOCUS_MODE_NORMAL,           // Default focus mode
        FOCUS_MODE_TRIGGERAUTO,      // Triggers a single autofocus operation
        FOCUS_MODE_CONTINUOUSAUTO,   // Continuous autofocus mode
        FOCUS_MODE_INFINITY,         // Focus set to infinity
        FOCUS_MODE_MACRO             // Macro mode for close-up focus
    }

    // This struct stores video mode data. This includes the width and height of
    // the frame and the framerate of the camera.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VideoModeData
    {
        public int width;
        public int height;
        public float frameRate;
    }

    #endregion // NESTED



    #region PROPERTIES

    // Returns an instance of a CameraDevice (thread safe)
    public static CameraDevice Instance
    {
        get
        {
            // Make sure only one instance of CameraDevice is created.
            if (mInstance == null)
            {
                lock (typeof(CameraDevice))
                {
                    if (mInstance == null)
                    {
                        mInstance = new CameraDevice();
                    }
                }
            }
            return mInstance;
        }
    }

    #endregion // NESTED



    #region PRIVATE_MEMBERS

    private static CameraDevice mInstance = null;

    private Dictionary<Image.PIXEL_FORMAT, Image> mCameraImages;

    #endregion // PRIVATE_MEMBERS

    

    #region PUBLIC_METHODS

    // Initializes the camera.
    public bool Init()
    {
        if (cameraDeviceInitCamera() == 0)
        {
            return false;
        }

        return true;
    }


    // Deinitializes the camera.
    public bool Deinit()
    {
        if (cameraDeviceDeinitCamera() == 0)
        {
            return false;
        }

        return true;
    }


    // Starts the camera. Frames are being delivered.
    public bool Start()
    {
        if (cameraDeviceStartCamera() == 0)
        {
            return false;
        }

        return true;
    }


    // Stops the camera if video feed is not required
    // (e.g. in non-AR mode of an application).
    public bool Stop()
    {
        if (cameraDeviceStopCamera() == 0)
        {
            return false;
        }

        return true;
    }


    // Get the video mode data that matches the given CameraDeviceMode.
    public VideoModeData GetVideoMode(CameraDeviceMode mode)
    {
        IntPtr videoModePtr = Marshal.AllocHGlobal(
                            Marshal.SizeOf(typeof(VideoModeData)));
        cameraDeviceGetVideoMode((int) mode, videoModePtr);
        VideoModeData videoMode = (VideoModeData) Marshal.PtrToStructure
                            (videoModePtr, typeof(VideoModeData));
        Marshal.FreeHGlobal(videoModePtr);

        return videoMode;
    }


    // Chooses a video mode out of the list of modes.
    public bool SelectVideoMode(CameraDeviceMode mode)
    {
        if (cameraDeviceSelectVideoMode((int) mode) == 0)
        {
            return false;
        }

        return true;
    }


    // Activate or deactivate the camera device flash.
    // Returns false if flash is not available or can't be activated.
    public bool SetFlashTorchMode(bool on)
    {
        bool result = cameraDeviceSetFlashTorchMode(on ? 1 : 0) != 0;
        Debug.Log("Toggle flash " + (on ? "ON" : "OFF") + " " + (result ?
                  "WORKED" : "FAILED"));
        return result;
    }


    // Set the active focus mode.
    // Returns false if this mode is not available or can't be activated.
    public bool SetFocusMode(FocusMode mode)
    {
        bool result = cameraDeviceSetFocusMode((int)mode) != 0;
        Debug.Log("Requested Focus mode " + mode + (result ?
                  " successfully." : ".  Not supported on this device."));
        return result;
    }


    // Enables or disables the request of the camera image in the desired pixel
    // format. Returns true on success, false otherwise. Note that this may
    // result in processing overhead. Image are accessed using GetCameraImage.
    // Note that there may be a delay of several frames until the camera image
    // becomes availables.
    public bool SetFrameFormat(Image.PIXEL_FORMAT format, bool enabled)
    {
        if (enabled)
        {
            if (!mCameraImages.ContainsKey(format))
            {
                if (qcarSetFrameFormat((int)format, 1) == 0)
                {
                    Debug.LogError("Failed to set frame format");
                    return false;
                }

                Image newImage = new Image();
                newImage.PixelFormat = format;
                mCameraImages.Add(format, newImage);
                return true;
            }
        }
        else
        {
            if (mCameraImages.ContainsKey(format))
            {
                if (qcarSetFrameFormat((int)format, 0) == 0)
                {
                    Debug.LogError("Failed to set frame format");
                    return false;
                }

                return mCameraImages.Remove(format);
            }
        }

        return true;
    }


    // Returns a camera images for the requested format. Returns null if
    // this image is not available. You must call SetFrameFormat before
    // accessing the corresponding camera image.
    public Image GetCameraImage(Image.PIXEL_FORMAT format)
    {
        // Has the format been requested:
        if (mCameraImages.ContainsKey(format))
        {
            // Check the image is valid:
            Image image = mCameraImages[format];
            if (image.IsValid())
            {
                return image;
            }
        }

        // No valid image of this format:
        return null;
    }


    // Returns the container of all requested images. The images may or may 
    // not be initialized. Please use GetCameraImage for a list of
    // available and valid images. Used only by the QCARBehaviour.
    public Dictionary<Image.PIXEL_FORMAT, Image> GetAllImages()
    {
        return mCameraImages;
    }

    #endregion // PUBLIC_METHODS



    #region CONSTRUCTION

    private CameraDevice()
    {
        mCameraImages = new Dictionary<Image.PIXEL_FORMAT, Image>();
    }

    #endregion // CONSTRUCTION



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceInitCamera();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceDeinitCamera();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceStartCamera();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceStopCamera();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceGetNumVideoModes();

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern void cameraDeviceGetVideoMode(int idx,
                                    [In, Out]IntPtr videoMode);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceSelectVideoMode(int idx);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceSetFlashTorchMode(int on);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int cameraDeviceSetFocusMode(int focusMode);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarSetFrameFormat(int format, int enabled);

#else

    private static int cameraDeviceInitCamera() { return 0; }


    private static int cameraDeviceDeinitCamera() { return 0; }


    private static int cameraDeviceStartCamera() { return 0; }


    private static int cameraDeviceStopCamera() { return 0; }


    private static int cameraDeviceGetNumVideoModes() { return 0; }


    private static void cameraDeviceGetVideoMode(int idx,
                                    [In, Out]IntPtr videoMode) { }


    private static int cameraDeviceSelectVideoMode(int idx) { return 0; }


    private static int cameraDeviceSetFlashTorchMode(int on) { return 1; }


    private static int cameraDeviceSetFocusMode(int focusMode) { return 1; }


    private static int qcarSetFrameFormat(int format, int enabled) { return 1; }

#endif

    #endregion // NATIVE_FUNCTIONS
}
