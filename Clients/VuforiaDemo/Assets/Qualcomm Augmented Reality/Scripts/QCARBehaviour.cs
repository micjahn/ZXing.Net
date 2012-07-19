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

// The QCARBehaviour class handles tracking and triggers native video
// background rendering. The class updates all Trackables in the scene.
[RequireComponent(typeof(Camera))]
public class QCARBehaviour : MonoBehaviour
{
    #region NESTED

    // The world center mode defines how the relative coordinates between
    // Trackables and camera are translated into Unity world coordinates.
    // If a world center is present the virtual camera in the Unity scene is
    // transformed with respect to that.
    // The world center mode is set through the Unity inspector.
    public enum WorldCenterMode
    {
        // User defines a single Trackable that defines the world center.
        USER,
        // Tracker uses the first Trackable that comes into view as the world
        // center (world center changes during runtime).
        AUTO,
        // Do not define a world center but only move Trackables with respect
        // to a fixed camera.
        NONE
    }

    // State of the camera.
    private enum CameraState
    {
        UNINITED,        // Camera is not yet initialized.
        DEVICE_INITED,   // Camera device is initialized.
        RENDERING_INITED // Video rendering is initialized.
    }

    #endregion // NESTED



    #region PROPERTIES

    // This property is used to query the active world center mode.
    public WorldCenterMode WorldCenterModeSetting
    {
        get { return mWorldCenterMode; }
    }

    // This property is used to query the world center Trackable
    // (will return null in "NONE" mode).
    public TrackableBehaviour WorldCenter
    {
        get { return mWorldCenter; }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    [SerializeField]
    private CameraDevice.CameraDeviceMode CameraDeviceModeSetting =
                    CameraDevice.CameraDeviceMode.MODE_DEFAULT;

    [SerializeField]
    private int MaxSimultaneousImageTargets = 1;

    // split detection of multiple targets over multiple frames
    [SerializeField]
    private bool MultiFrameEnabled = true;

    // tie the framerate to the camera framerate
    [SerializeField]
    private bool SynchronousVideo = false;

    [SerializeField]
    [HideInInspector]
    private WorldCenterMode mWorldCenterMode = WorldCenterMode.AUTO;

    [SerializeField]
    [HideInInspector]
    private TrackableBehaviour mWorldCenter = null;

    private List<ITrackerEventHandler> mTrackerEventHandlers =
                    new List<ITrackerEventHandler>();

    private bool mIsInitialized = false;

    private CameraState mCameraState = CameraState.UNINITED;
    private Material mClearMaterial;
    private Rect mViewportRect;
    private int mClearBuffers;

    private bool mHasStartedOnce = false;

    private ScreenOrientation mProjectionOrientation = ScreenOrientation.Unknown;

    private bool mCachedDrawVideoBackground;
    private CameraClearFlags mCachedCameraClearFlags;
    private Color mCachedCameraBackgroundColor;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // This method registers a new Tracker event handler at the Tracker.
    // These handlers are called as soon as ALL Trackables have been updated
    // in this frame.
    public void RegisterTrackerEventHandler(
                                ITrackerEventHandler trackerEventHandler)
    {
        mTrackerEventHandlers.Add(trackerEventHandler);
    }


    // This method unregisters a Tracker event handler.
    // Returns "false" if event handler does not exist.
    public bool UnregisterTrackerEventHandler(
                                ITrackerEventHandler trackerEventHandler)
    {
        return mTrackerEventHandlers.Remove(trackerEventHandler);
    }


    // This method is used to set the world center mode in the Unity editor.
    // Switching modes is not supported at runtime.
    public void SetWorldCenterMode(WorldCenterMode value)
    {
        if (!Application.isEditor)
            return;

        mWorldCenterMode = value;
    }


    // This method is used to set the world center in the Unity editor in
    // "USER" mode. Switching modes is not supported at runtime.
    public void SetWorldCenter(TrackableBehaviour value)
    {
        if (!Application.isEditor)
            return;

        mWorldCenter = value;
    }


    // This method simply returns the viewport rectangle.
    public Rect GetViewportRectangle()
    {
        return mViewportRect;
    }


    // This method returns the surface orientation.
    public ScreenOrientation GetSurfaceOrientation()
    {
        return (ScreenOrientation)getSurfaceOrientation();
    }

    #endregion // PUBLIC_METHODS



    #region UNITY_MONOBEHAVIOUR_METHODS

    // Starts up the QCAR extension with the properties that were set in the
    // Unity inspector.
    void Start()
    {
        // First we check if QCAR initialized correctly.
        if (QCAR.CheckInitializationError() != QCAR.InitError.INIT_SUCCESS)
        {
            mIsInitialized = false;
            return;
        }

        // Initialize the trackers if they haven't already been initialized.
        if (TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER) == null)
        {
            TrackerManager.Instance.InitTracker(Tracker.Type.MARKER_TRACKER);
        }

        if (TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER) == null)
        {
            TrackerManager.Instance.InitTracker(Tracker.Type.IMAGE_TRACKER);
        }

        // Cache the camera start values
        mCachedDrawVideoBackground = QCARManager.Instance.DrawVideoBackground;
        mCachedCameraClearFlags = this.camera.clearFlags;
        mCachedCameraBackgroundColor = this.camera.backgroundColor;

        // Reset the camera clear flags and create a simple material
        ResetCameraClearFlags();
        mClearMaterial = new Material(Shader.Find("Diffuse"));

        // Set QCAR hints from the Inspector options
        QCAR.SetHint(QCAR.QCARHint.HINT_MAX_SIMULTANEOUS_IMAGE_TARGETS,
                            MaxSimultaneousImageTargets);
        QCAR.SetHint(QCAR.QCARHint.HINT_IMAGE_TARGET_MULTI_FRAME_ENABLED,
                            MultiFrameEnabled ? 1 : 0);

        // Set the Unity version for internal use
        QCAR.SetUnityVersion();

        // Find markers in the scene and register them with QCAR
        MarkerBehaviour[] markerBehaviours = (MarkerBehaviour[])
            UnityEngine.Object.FindObjectsOfType(typeof(MarkerBehaviour));
        MarkerTracker markerTracker = (MarkerTracker)TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER);
        if (markerTracker != null)
        {
            markerTracker.AddMarkers(markerBehaviours);
        }

        // Start the camera and tracker
        StartQCAR();

        // Initialize the QCARManager
        QCARManager.Instance.WorldCenterMode = mWorldCenterMode;
        QCARManager.Instance.WorldCenter = mWorldCenter;
        QCARManager.Instance.ARCamera = camera;
        QCARManager.Instance.Init();

        // Initialize local variables
        mIsInitialized = true;
        mHasStartedOnce = true;
    }


    // Restart the camera and tracker if the QCARBehaviour is reenabled.
    // Note that we check specifically that Start() has been called once
    // where QCAR is fully initialized.
    void OnEnable()
    {
        if (mHasStartedOnce)
        {
            StartQCAR();
        }
    }


    // Updates the scene with new tracking data. Calls registered
    // ITrackerEventHandlers
    void Update()
    {
        if (!mIsInitialized)
            return;

        // Get the current orientation of the surface:
        ScreenOrientation surfaceOrientation = (ScreenOrientation)getSurfaceOrientation();
        
        // Check if we need to update the video background configuration and projection matrix:
        if (QCAR.IsRendererDirty() || mProjectionOrientation != surfaceOrientation)
        {
            ConfigureVideoBackground();
            UpdateProjection(surfaceOrientation);
        }

        // Bind a simple material to clear the OpenGL state
        mClearMaterial.SetPass(0);

        // QCARManager renders the camera image and updates the trackables
        QCARManager.Instance.Update(mProjectionOrientation);

        // Tell Unity that we may have changed the OpenGL state behind the scenes
        GL.InvalidateState();

        // Update the camera clear flags
        UpdateCameraClearFlags();

        // Let the trackable event handlers know that all trackables have been updated
        foreach (ITrackerEventHandler handler in mTrackerEventHandlers)
        {
            handler.OnTrackablesUpdated();
        }
    }


    // Called after the scene has been rendered
    void OnPostRender()
    {
        // Clear the framebuffer as many times as defined upon init
        if (mClearBuffers > 0)
        {
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            mClearBuffers--;
        }
    }


    // Stops QCAR when the application is sent to the background.
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopQCAR();
            // On iOS make sure suspend/resume doesn't animate an old image
            #if UNITY_IPHONE
                GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            #endif
        }
        else
        {
            StartQCAR();
            // Clear any artifacts from the buffers on resume
            mClearBuffers = 12;
        }
    }


    // Stop the tracker and camera when QCARBehaviour is disabled.
    void OnDisable()
    {
        StopQCAR();

        ResetCameraClearFlags();
    }


    // Deinitialize QCAR and trackers when QCARBehaviour is destroyed.
    void OnDestroy()
    {
        // Destroy all the datasets
        ImageTracker imageTracker = (ImageTracker)TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
        if (imageTracker != null)
        {
            imageTracker.DestroyAllDataSets(false);
        }

        // Destroy all the markers
        MarkerTracker markerTracker = (MarkerTracker)TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER);
        if (markerTracker != null)
        {
            markerTracker.DestroyAllMarkers(false);
        }

        // Deinit the QCARManager
        QCARManager.Instance.Deinit();

        // Deinit the trackers
        if (TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER) != null)
        {
            TrackerManager.Instance.DeinitTracker(Tracker.Type.MARKER_TRACKER);
        }

        if (TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER) != null)
        {
            TrackerManager.Instance.DeinitTracker(Tracker.Type.IMAGE_TRACKER);
        }
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS

    // Start the camera and trackers and update the camera projection
    private void StartQCAR()
    {
        Debug.Log("StartQCAR");

        CameraDevice.Instance.Init();

        ConfigureVideoBackground();

        CameraDevice.Instance.SelectVideoMode(CameraDeviceModeSetting);
        CameraDevice.Instance.Start();

        if (TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER) != null)
        {
            TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER).Start();
        }

        if (TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER) != null)
        {
            TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER).Start();
        }

        ScreenOrientation surfaceOrientation = (ScreenOrientation)getSurfaceOrientation();
        UpdateProjection(surfaceOrientation);
    }


    // Stop the trackers, stop and deinit the camera
    private void StopQCAR()
    {
        Debug.Log("StopQCAR");

        if (TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER) != null)
        {
            TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER).Stop();
        }

        if (TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER) != null)
        {
            TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER).Stop();
        }

        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();
    }


    // Reset the camera state and clear any artifacts from the buffers
    private void ResetCameraClearFlags()
    {
        mCameraState = CameraState.UNINITED;
        mClearBuffers = 12;

        // Restore clear settings to Inspector values
        this.camera.clearFlags = mCachedCameraClearFlags;
        this.camera.backgroundColor = mCachedCameraBackgroundColor;
    }


    // Update the camera clear flags and background color in response
    // to QCAR settings
    private void UpdateCameraClearFlags()
    {
        // Specifically handle when running in the editor:
        if (Application.isEditor)
        {
            mCameraState = CameraState.UNINITED;
            return;
        }

        // Update camera clear flags if necessary
        switch (mCameraState)
        {
            case CameraState.UNINITED:
                mCameraState = CameraState.DEVICE_INITED;
                break;

            case CameraState.DEVICE_INITED:
                // Check whether QCAR requires a transparent clear color
                if (QCAR.RequiresAlpha())
                {
                    // Camera clears both depth and color buffer,
                    // We set the clear color to transparent black as
                    // required by QCAR.
                    this.camera.clearFlags = CameraClearFlags.SolidColor;
                    this.camera.backgroundColor = new Color(0, 0, 0, 0);
                    Debug.Log("Setting camera clear flags to transparent black");
                }
                else
                {
                    // Check whether QCAR is rendering the video in native
                    if (mCachedDrawVideoBackground)
                    {
                        // Clear only depth
                        this.camera.clearFlags = CameraClearFlags.Depth;
                        Debug.Log("Setting camera clear flags to depth only");
                    }
                    else
                    {
                        // Restore clear settings to Inspector values
                        this.camera.clearFlags = mCachedCameraClearFlags;
                        this.camera.backgroundColor = mCachedCameraBackgroundColor;
                        Debug.Log("Setting camera clear flags to Inspector values");
                    }
                }

                mCameraState = CameraState.RENDERING_INITED;                
                break;

            case CameraState.RENDERING_INITED:
                // Check if the DrawVideoBackground setting has changed
                bool drawVideoBackground = QCARManager.Instance.DrawVideoBackground;
                if (drawVideoBackground != mCachedDrawVideoBackground)
                {
                    mCameraState = CameraState.DEVICE_INITED;
                    mCachedDrawVideoBackground = drawVideoBackground;
                }
                break;
        }
    }


    // Configure the size and position of the video background rendered
    // natively when QCARManager.DrawVideoBackground is true
    private void ConfigureVideoBackground()
    {
        QCARRenderer.VideoBGCfgData config = QCARRenderer.Instance.GetVideoBackgroundConfig();
        CameraDevice.VideoModeData videoMode = CameraDevice.Instance.GetVideoMode(CameraDeviceModeSetting);

        config.enabled = 1;
        config.synchronous = (SynchronousVideo ? 1 : 0);
        config.position = new QCARRenderer.Vec2I(0, 0);

        if (Screen.width > Screen.height)
        {
            float height = videoMode.height * (Screen.width / (float)
                            videoMode.width);
            config.size = new QCARRenderer.Vec2I(Screen.width, (int) height);

            if (config.size.y < Screen.height)
            {
                // Correcting rendering background size to handle missmatch
                // between screen and video aspect ratios
                config.size.x = (int) (Screen.height
                                    * (videoMode.width / (float) videoMode.height));
                config.size.y = Screen.height;
            }
        }
        else
        {
            float width = videoMode.height * (Screen.height / (float)
                            videoMode.width);
            config.size = new QCARRenderer.Vec2I((int) width, Screen.height);

            if (config.size.x < Screen.width)
            {
                // Correcting rendering background size to handle missmatch
                // between screen and video aspect ratios
                config.size.x = Screen.width;
                config.size.y = (int) (Screen.width * 
                                  (videoMode.width / (float) videoMode.height));
            }
        }

        QCARRenderer.Instance.SetVideoBackgroundConfig(config);

        int viewportX = config.position.x + (Screen.width - config.size.x) / 2;
        int viewportY = config.position.y + (Screen.height - config.size.y) / 2;
        mViewportRect = new Rect(viewportX, viewportY,
                                    config.size.x, config.size.y);
    }


    // Update the camera projection matrix to match QCAR settings
    private void UpdateProjection(ScreenOrientation orientation)
    {
        if (Application.isEditor)
        {
            // Skip this when running in the editor
            return;
        }

        // This member stores the screen orientation used for the last projection matrix
        // update. It is used to counter rotate the poses later.
        mProjectionOrientation = orientation;

        Matrix4x4 projectionMatrix = QCAR.GetProjectionGL(camera.nearClipPlane,
                                    camera.farClipPlane, mProjectionOrientation);

        if (mViewportRect.width != Screen.width)
        {
            float viewportDistort = mViewportRect.width / Screen.width;
            projectionMatrix[0] *= viewportDistort;
        }

        if (mViewportRect.height != Screen.height)
        {
            float viewportDistort = mViewportRect.height / Screen.height;
            projectionMatrix[5] *= viewportDistort;
        }

        this.camera.projectionMatrix = projectionMatrix;
    }

    #endregion // PRIVATE_METHODS




    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int getSurfaceOrientation();

#else

    private static int getSurfaceOrientation() { return 0; }

#endif

    #endregion // NATIVE_FUNCTIONS
}
