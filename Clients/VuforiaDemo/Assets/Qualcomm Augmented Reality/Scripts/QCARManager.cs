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

public class QCARManager
{
    #region NESTED

    // This struct stores 3D pose information as a position-vector,
    // orientation-Quaternion pair. The pose is given relatively to the camera.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PoseData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Vector3 position;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Quaternion orientation;
    }

    // This struct stores general Trackable data like its 3D pose, its status
    // and its unique id.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TrackableData
    {
        public PoseData pose;
        public TrackableBehaviour.Status status;
        public int id;
    }

    // This struct stores Virtual Button data like its current status (pressed
    // or not pressed) and its unique id.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VirtualButtonData
    {
        public int id;
        public int isPressed;
    }

    // This struct stores data of an image header. It includes the width and
    // height of the image, the byte stride in the buffer, the buffer size
    // (which can differ from the image size e.g. when image is converted to a
    // power of two size) and the format of the image
    // (e.g. RGB565, grayscale, etc.).
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct ImageHeaderData
    {
        public int width;
        public int height;
        public int stride;
        public int bufferWidth;
        public int bufferHeight;
        public int format;
        public int reallocate;
        public int updated;
        public IntPtr data;
    }

    #endregion // NESTED



    #region PROPERTIES

    // Returns an instance of a QCARManager (thread safe)
    public static QCARManager Instance
    {
        get
        {
            // Make sure only one instance of QCARManager is created.
            if (mInstance == null)
            {
                lock (typeof(QCARManager))
                {
                    if (mInstance == null)
                    {
                        mInstance = new QCARManager();
                    }
                }
            }
            return mInstance;
        }
    }

    // World Center Mode setting on the ARCamera
    public QCARBehaviour.WorldCenterMode WorldCenterMode
    {
        set { mWorldCenterMode = value; }
        get { return mWorldCenterMode; }
    }

    // World Center setting on the ARCamera
    public TrackableBehaviour WorldCenter
    {
        set { mWorldCenter = value; }
        get { return mWorldCenter; }
    }

    // A handle to the ARCamera object
    public Camera ARCamera
    {
        set { mARCamera = value; }
        get { return mARCamera; }
    }

    // True to have QCAR render the video background image natively
    // False to bind the video background to the texture set in
    // QCARRenderer.SetVideoBackgroundTextureID
    public bool DrawVideoBackground
    {
        set { mDrawVideobackground = value; }
        get { return mDrawVideobackground; }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    private static QCARManager mInstance = null;

    private QCARBehaviour.WorldCenterMode mWorldCenterMode;
    private TrackableBehaviour mWorldCenter = null;
    private Camera mARCamera = null;

    private int mAbsoluteNumTrackables = 0;
    private int mAbsoluteNumVirtualButtons = 0;
    private IntPtr mTrackablePtr = IntPtr.Zero;
    private IntPtr mVirtualButtonPtr = IntPtr.Zero;
    private TrackableData[] mTrackableDataArray = null;

    private LinkedList<int> mTrackableFoundQueue = new LinkedList<int>();

    private bool mTrackableDataArrayDirty = false;
    private bool mVBDataArrayDirty = false;

    private IntPtr mImageHeaderData = IntPtr.Zero;
    private int mNumImageHeaders = 0;

    private bool mDrawVideobackground = true;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // Initialization
    public bool Init()
    {
        mAbsoluteNumTrackables = 0;
        mAbsoluteNumVirtualButtons = 0;
        mTrackablePtr = IntPtr.Zero;
        mVirtualButtonPtr = IntPtr.Zero;
        mTrackableDataArray = null;

        mTrackableFoundQueue = new LinkedList<int>();

        mTrackableDataArrayDirty = false;
        mVBDataArrayDirty = false;

        mImageHeaderData = IntPtr.Zero;
        mNumImageHeaders = 0;

        InitializeTrackableContainer();

        InitializeVirtualButtonContainer();

        return true;
    }


    // Reinitialize the trackable and virtual button arrays on the next pass
    public bool Reinit()
    {
        mTrackableDataArrayDirty = true;
        mVBDataArrayDirty = true;

        return true;
    }


    // Process the camera image and tracking data for this frame
    public void Update(ScreenOrientation counterRotation)
    {
        // Reinitialize the trackable data container if required:
        if (mTrackableDataArrayDirty)
        {
            InitializeTrackableContainer();
            mTrackableDataArrayDirty = false;
        }

        // Reinitialize the virtual button data container if required:
        if (mVBDataArrayDirty)
        {
            InitializeVirtualButtonContainer();
            mVBDataArrayDirty = false;
        }

        // Prepare the camera image container
        UpdateImageContainer();

        // Draw the video background or update the video texture
        // Retrieve trackable and virtual button state for this frame
        // Also retrieve registered camera images for this frame
        updateQCAR(mTrackablePtr, mAbsoluteNumTrackables,
            mVirtualButtonPtr, mAbsoluteNumVirtualButtons,
            mImageHeaderData, mNumImageHeaders,
            (int)counterRotation, mDrawVideobackground ? 0 : 1);

        // Handle the camera image data
        UpdateCameraFrame();

        // Handle the trackable data
        UpdateTrackers();
    }


    // Free globally allocated containers
    public void Deinit()
    {
        Marshal.FreeHGlobal(mTrackablePtr);
        Marshal.FreeHGlobal(mVirtualButtonPtr);
        Marshal.FreeHGlobal(mImageHeaderData);
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    // Initialize the container for retrieving tracking data from native
    private void InitializeTrackableContainer()
    {
        // Destroy if the container has been allocated.
        Marshal.FreeHGlobal(mTrackablePtr);

        mAbsoluteNumTrackables = 0;

        ImageTracker imageTracker = (ImageTracker) TrackerManager.Instance.GetTracker(
                                        Tracker.Type.IMAGE_TRACKER);
        MarkerTracker markerTracker = (MarkerTracker) TrackerManager.Instance.GetTracker(
                                        Tracker.Type.MARKER_TRACKER);

        if (imageTracker != null)
        {
            DataSet activeDataSet = imageTracker.GetActiveDataSet();
            if (activeDataSet != null)
            {
                mAbsoluteNumTrackables += activeDataSet.GetNumTrackables();
            }
        }

        if (markerTracker != null)
        {
            mAbsoluteNumTrackables += markerTracker.GetNumMarkers();
        }

        if (mAbsoluteNumTrackables > 0)
        {
            mTrackablePtr = Marshal.AllocHGlobal(Marshal.SizeOf(
                    typeof(TrackableData)) * mAbsoluteNumTrackables);
        }
        else
        {
            mTrackablePtr = IntPtr.Zero;
        }

        mTrackableDataArray = new TrackableData[mAbsoluteNumTrackables];

        if (!Application.isEditor)
        {
            Debug.Log("Absolute number of Trackables: " + mAbsoluteNumTrackables);
        }
    }


    // Initialize the container for retrieving virtual button data from native
    private void InitializeVirtualButtonContainer()
    {
        // Destroy if the container has been allocated.
        Marshal.FreeHGlobal(mVirtualButtonPtr);

        mAbsoluteNumVirtualButtons = 0;

        ImageTracker imageTracker = (ImageTracker) TrackerManager.Instance.GetTracker(
                                        Tracker.Type.IMAGE_TRACKER);

        if (imageTracker != null)
        {
            DataSet activeDataSet = imageTracker.GetActiveDataSet();
            if (activeDataSet != null)
            {
                mAbsoluteNumVirtualButtons = dataSetGetNumVirtualButtons(activeDataSet.DataSetPtr);
            }
        }

        if (mAbsoluteNumVirtualButtons > 0)
        {
            mVirtualButtonPtr = Marshal.AllocHGlobal(Marshal.SizeOf(
                    typeof(VirtualButtonData)) * mAbsoluteNumVirtualButtons);
        }
        else
        {
            mVirtualButtonPtr = IntPtr.Zero;
        }
        
        if (!Application.isEditor)
        {
            Debug.Log("Absolute number of virtual buttons: " + mAbsoluteNumVirtualButtons);
        }
    }


    // Unmarshal and process the tracking data
    private void UpdateTrackers()
    {
        if (Application.isEditor)
        {
            UpdateTrackablesEditor();
            return;
        }

        // Unmarshal the trackable data
        // Take our array of unmanaged data from native and create an array of
        // TrackableData structures to work with (one per trackable, regardless
        // of whether or not that trackable is visible this frame).
        for (int i = 0; i < mAbsoluteNumTrackables; i++)
        {
            IntPtr trackablePtr = new IntPtr(mTrackablePtr.ToInt32() + i *
                    Marshal.SizeOf(typeof(TrackableData)));
            TrackableData trackableData = (TrackableData)
                    Marshal.PtrToStructure(trackablePtr, typeof(TrackableData));
            mTrackableDataArray[i] = trackableData;
        }

        // Add newly found Trackables to the queue, remove lost ones
        // We keep track of the order in which Trackables become visible for the
        // AUTO World Center mode. This keeps the camera from jumping around in the
        // scene too much.
        foreach (TrackableData trackableData in mTrackableDataArray)
        {
            // We found a trackable (or set of Trackables) that match this id
            if ((trackableData.status == TrackableBehaviour.Status.DETECTED
                 || trackableData.status ==
                 TrackableBehaviour.Status.TRACKED))
            {
                if (!mTrackableFoundQueue.Contains(trackableData.id))
                {
                    // The trackable just became visible, add it to the queue
                    mTrackableFoundQueue.AddLast(trackableData.id);
                }
            }
            else
            {
                if (mTrackableFoundQueue.Contains(trackableData.id))
                {
                    // The trackable just disappeared, remove it from the queue
                    mTrackableFoundQueue.Remove(trackableData.id);
                }
            }
        }

        ImageTracker imageTracker = (ImageTracker)
            TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
        MarkerTracker markerTracker = (MarkerTracker)
            TrackerManager.Instance.GetTracker(Tracker.Type.MARKER_TRACKER);

        // The "scene origin" is only used in world center mode auto or user.
        int originTrackableID = -1;

        if (mWorldCenterMode == QCARBehaviour.WorldCenterMode.USER &&
            mWorldCenter != null)
        {
            originTrackableID = mWorldCenter.TrackableID;
        }
        else if (mWorldCenterMode == QCARBehaviour.WorldCenterMode.AUTO)
        {
            imageTracker.RemoveDisabledTrackablesFromQueue(ref mTrackableFoundQueue);
            markerTracker.RemoveDisabledTrackablesFromQueue(ref mTrackableFoundQueue);
            if (mTrackableFoundQueue.Count > 0)
            {
                originTrackableID = mTrackableFoundQueue.First.Value;
            }
        }

        // Update the Camera pose before Trackable poses are updated.
        imageTracker.UpdateCameraPose(mARCamera, mTrackableDataArray, originTrackableID);
        markerTracker.UpdateCameraPose(mARCamera, mTrackableDataArray, originTrackableID);

        // Update the Trackable poses.
        imageTracker.UpdateTrackablePoses(mARCamera, mTrackableDataArray, originTrackableID);
        markerTracker.UpdateTrackablePoses(mARCamera, mTrackableDataArray, originTrackableID);

        imageTracker.UpdateVirtualButtons(mAbsoluteNumVirtualButtons, mVirtualButtonPtr);
    }


    // Simulate tracking in the editor
    private void UpdateTrackablesEditor()
    {
        // When running within the Unity editor:
        TrackableBehaviour[] trackableBehaviours = (TrackableBehaviour[])
                UnityEngine.Object.FindObjectsOfType(typeof(TrackableBehaviour));

        // Simulate all Trackables were tracked successfully:    
        for (int i = 0; i < trackableBehaviours.Length; i++)
        {
            TrackableBehaviour trackable = trackableBehaviours[i];
            if (trackable.enabled)
            {
                trackable.OnTrackerUpdate(TrackableBehaviour.Status.TRACKED);
            }
        }
    }


    // Update the image container for the currently registered formats
    private void UpdateImageContainer()
    {
        CameraDevice cameraDevice = CameraDevice.Instance;

        // Reallocate the data container if the number of requested images has
        // changed, or if the container is not allocated
        if (mNumImageHeaders != cameraDevice.GetAllImages().Count ||
           (cameraDevice.GetAllImages().Count > 0 && mImageHeaderData == IntPtr.Zero))
        {

            mNumImageHeaders = cameraDevice.GetAllImages().Count;

            Marshal.FreeHGlobal(mImageHeaderData);
            mImageHeaderData = Marshal.AllocHGlobal(Marshal.SizeOf(
                                typeof(ImageHeaderData)) * mNumImageHeaders);
        }

        // Update the image info:
        int i = 0;
        foreach (Image image in cameraDevice.GetAllImages().Values)
        {
            IntPtr imagePtr = new IntPtr(mImageHeaderData.ToInt32() + i *
                   Marshal.SizeOf(typeof(ImageHeaderData)));

            ImageHeaderData imageHeader = new ImageHeaderData();
            imageHeader.width = image.Width;
            imageHeader.height = image.Height;
            imageHeader.stride = image.Stride;
            imageHeader.bufferWidth = image.BufferWidth;
            imageHeader.bufferHeight = image.BufferHeight;
            imageHeader.format = (int)image.PixelFormat;
            imageHeader.reallocate = 0;
            imageHeader.updated = 0;
            imageHeader.data = image.UnmanagedData;

            Marshal.StructureToPtr(imageHeader, imagePtr, false);
            ++i;
        }
    }


    // Unmarshal the camera images for this frame
    private void UpdateCameraFrame()
    {
        // Unmarshal the image data:
        int i = 0;
        foreach (Image image in CameraDevice.Instance.GetAllImages().Values)
        {
            IntPtr imagePtr = new IntPtr(mImageHeaderData.ToInt32() + i *
                   Marshal.SizeOf(typeof(ImageHeaderData)));
            ImageHeaderData imageHeader = (ImageHeaderData)
                Marshal.PtrToStructure(imagePtr, typeof(ImageHeaderData));

            // Copy info back to managed Image instance:
            image.Width = imageHeader.width;
            image.Height = imageHeader.height;
            image.Stride = imageHeader.stride;
            image.BufferWidth = imageHeader.bufferWidth;
            image.BufferHeight = imageHeader.bufferHeight;
            image.PixelFormat = (Image.PIXEL_FORMAT) imageHeader.format;

            // Reallocate if required:
            if (imageHeader.reallocate == 1)
            {
                image.Pixels = new byte[qcarGetBufferSize(image.BufferWidth,
                                                    image.BufferHeight,
                                                    (int)image.PixelFormat)];

                Marshal.FreeHGlobal(image.UnmanagedData);

                image.UnmanagedData = Marshal.AllocHGlobal(qcarGetBufferSize(image.BufferWidth,
                                    image.BufferHeight,
                                    (int)image.PixelFormat));

                // Note we don't copy the data this frame as the unmanagedVirtualButtonBehaviour
                // buffer was not filled.
            }
            else if (imageHeader.updated == 1)
            {
                // Copy data:
                image.CopyPixelsFromUnmanagedBuffer();
            }

            ++i;
        }
    }

    #endregion // PRIVATE_METHODS



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int updateQCAR([In, Out]IntPtr trackableDataArray,
                                    int trackableArrayLength,
                                    [In, Out]IntPtr vbDataArray,
                                    int vbArrayLength,
                                    [In, Out]IntPtr imageHeaderDataArray,
                                    int imageHeaderArrayLength,
                                    int screenOrientation,
                                    int bindVideoBackground);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerGetId(String nTrackableName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int markerSetSize(int trackableIndex, [In, Out]IntPtr size);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetNumVirtualButtons(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarGetBitsPerPixel(int format);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int qcarGetBufferSize(int width, int height,
                                    int format);

#else

    private static int updateQCAR([In, Out]IntPtr trackableDataArray,
                                    int trackableArrayLength,
                                    [In, Out]IntPtr vbDataArray,
                                    int vbArrayLength,
                                    [In, Out]IntPtr imageHeaderDataArray,
                                    int imageHeaderArrayLength,
                                    int screenOrientation,
                                    int bindVideoBackground) { return 0; }

    private static int markerGetId(String nTrackableName) { return 0; }

    private static int markerSetSize(int trackableIndex, [In, Out]IntPtr size) { return 0; }

    private static int dataSetGetNumVirtualButtons(IntPtr dataSetPtr) { return 0; }

    private static int qcarGetBitsPerPixel(int format) { return 0; }

    private static int qcarGetBufferSize(int width, int height,
                                    int format) { return 0; }

#endif

    #endregion // NATIVE_FUNCTIONS
}
