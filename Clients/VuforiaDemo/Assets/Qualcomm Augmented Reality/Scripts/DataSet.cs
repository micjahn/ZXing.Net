/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DataSet
{
    #region NESTED

    // Storage type is used to interpret a given path string.
    public enum StorageType
    {
        STORAGE_APP, 
        STORAGE_APPRESOURCE,
        STORAGE_ABSOLUTE
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct ImageTargetData
    {
        public int id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Vector2 size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct MultiTargetData
    {
        public int id;
    }

    #endregion // NESTED



    #region PROPERTIES

    // Returns a data set instance.
    public IntPtr DataSetPtr
    {
        get { return mDataSetPtr; }
    }

    // Returns the path to the data set.
    public string Path
    {
        get { return mPath; }
    }

    // Returns the storage type of the data set.
    public StorageType FileStorageType
    {
        get { return mStorageType; }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    // Pointer stores address of a native DataSet instance.
    private IntPtr mDataSetPtr = IntPtr.Zero;
    // Path to the data set file on device storage.
    private string mPath = "";
    // Storage type of the data set file.
    private StorageType mStorageType = StorageType.STORAGE_APPRESOURCE;
    // Dictionary that contains Trackables that belong to this data set.
    private Dictionary<int, DataSetTrackableBehaviour> mTrackableBehaviourDict =
        new Dictionary<int, DataSetTrackableBehaviour>();
    // Dictionary that contains Virtual Buttons that belong to this data set.
    private Dictionary<int, VirtualButtonBehaviour> mVBBehaviourDict =
        new Dictionary<int, VirtualButtonBehaviour>();

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region CONSTRUCTION

    // Constructor allows to set native pointer.
    public DataSet(IntPtr dataSetPtr)
    {
        mDataSetPtr = dataSetPtr;
    }


    // We enforce construction with an IntPtr.
    private DataSet() { }

    #endregion // CONSTRUCTION



    #region PUBLIC_METHODS

    // Checks if a data set exists at the default "StreamingAssets/QCAR" directory.
    public static bool Exists(String name)
    {
        String path = "QCAR/" + name + ".xml";
        return Exists(path, StorageType.STORAGE_APPRESOURCE);
    }


    // Checks if a data set exists at the given path.
    // Storage type is used to correctly interpret the given path.
    public static bool Exists(String path, StorageType storageType)
    {
        return (dataSetExists(path, (int)storageType) == 1);
    }


    // Loads a data set from the default "StreamingAssets/QCAR" directory.
    public bool Load(String name)
    {
        String path = "QCAR/" + name + ".xml";
        return Load(path, StorageType.STORAGE_APPRESOURCE);
    }


    // Loads data set from the given path.
    // Storage type is used to correctly interpret the given path.
    public bool Load(String path, StorageType storageType)
    {
        if (mDataSetPtr == IntPtr.Zero)
        {
            Debug.LogError("Called Load without a data set object");
            return false;
        }

        if (dataSetLoad(path, (int)storageType, mDataSetPtr) == 0)
        {
            Debug.LogError("Did not load: " + path);
            return false;
        }

        // Set path and storage type to associate data sets with Trackables.
        mPath = path;
        mStorageType = storageType;

        
        // Step 1: Add all TrackableBehaviours that belong to this data set and
        // are already instantiated in the scene to the dictionary.
        DataSetTrackableBehaviour[] trackableBehaviours = (DataSetTrackableBehaviour[])
            UnityEngine.Object.FindObjectsOfType(typeof(DataSetTrackableBehaviour));
        AddTrackables(trackableBehaviours);

        // Step 2: Add all VirtualButtonBehaviours that belong to this data set
        // and are already instantiated in the scene to the dictionary.
        VirtualButtonBehaviour[] vbBehaviours = (VirtualButtonBehaviour[])
            UnityEngine.Object.FindObjectsOfType(typeof(VirtualButtonBehaviour));
        AddVirtualButtons(vbBehaviours);

        // Step 3: Create TrackableBehaviours that are not existing in the scene.
        CreateImageTargets();
        CreateMultiTargets();

        return true;
    }


    // Returns the number of trackables that are defined in the data set.
    public int GetNumTrackables()
    {
        if (mDataSetPtr == IntPtr.Zero)
        {
            Debug.LogError("Called GetNumTrackables without a data set object");
            return -1;
        }
        return dataSetGetNumTrackables(mDataSetPtr);
    }


    // Returns the Trackable at the given index.
    public DataSetTrackableBehaviour GetTrackable(int index)
    {
        Dictionary<int, DataSetTrackableBehaviour>.Enumerator enumerator =
            mTrackableBehaviourDict.GetEnumerator();

        DataSetTrackableBehaviour tb = null;

        for (int i = 0; i <= index; ++i)
        {
            if (!enumerator.MoveNext())
            {
                Debug.LogError("Trackable index invalid.");
                tb = null;
                break;
            }

            tb = enumerator.Current.Value;
        }

        return tb;
    }


    // Returns the Trackable with the given unique ID.
    // Unique IDs for Trackables are created when data sets are loaded.
    public bool TryGetTrackableByID(int id,
                                    out DataSetTrackableBehaviour trackable)
    {
        return mTrackableBehaviourDict.TryGetValue(id, out trackable);
    }


    public bool TryGetVirtualButtonByID(int id,
                                        out VirtualButtonBehaviour vb)
    {
        return mVBBehaviourDict.TryGetValue(id, out vb);
    }


    // Registers a Virtual Button at native code. This method is called
    // implicitly by the ImageTargetBehaviour.CreateVirtualButton method. This
    // method should not be called by user code.
    public bool RegisterVirtualButton(VirtualButtonBehaviour vb,
                                        string imageTargetName)
    {
        VirtualButtonBehaviour.RectangleData rectData =
                new VirtualButtonBehaviour.RectangleData();

        Vector2 leftTop, rightBottom;
        vb.CalculateButtonArea(out leftTop, out rightBottom);
        rectData.leftTopX = leftTop.x;
        rectData.leftTopY = leftTop.y;
        rectData.rightBottomX = rightBottom.x;
        rectData.rightBottomY = rightBottom.y;

        IntPtr rectPtr = Marshal.AllocHGlobal(
            Marshal.SizeOf(typeof(VirtualButtonBehaviour.RectangleData)));
        Marshal.StructureToPtr(rectData, rectPtr, false);

        bool registerWorked =
              (imageTargetCreateVirtualButton(DataSetPtr, imageTargetName,
                vb.VirtualButtonName, rectPtr) != 0);

        if (registerWorked)
        {
            int id = virtualButtonGetId(DataSetPtr, imageTargetName,
                vb.VirtualButtonName);

            // Initialize the id of the button:
            vb.InitializeID(id);

            // Check we don't have an entry for this id:
            if (!mVBBehaviourDict.ContainsKey(id))
            {
                // Add:
                mVBBehaviourDict.Add(id, vb);
            }
        }

        return registerWorked;
    }


    // Unregister a Virtual Button at native code. This method is called
    // implicitly by the ImageTargetBehaviour.DestroyVirtualButton method. This
    // method should not be called by user code.
    public bool UnregisterVirtualButton(VirtualButtonBehaviour vb,
                                            string imageTargetName)
    {
        int id = virtualButtonGetId(DataSetPtr, imageTargetName, vb.VirtualButtonName);

        bool unregistered = false;

        if (imageTargetDestroyVirtualButton(DataSetPtr, imageTargetName, vb.VirtualButtonName) != 0)
        {
            if (mVBBehaviourDict.Remove(id))
            {
                unregistered = true;
            }
        }

        if (!unregistered)
        {
            Debug.LogError("UnregisterVirtualButton: Failed to destroy " +
                            "the Virtual Button.");
        }

        return unregistered;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private void AddTrackables(DataSetTrackableBehaviour[] trackableBehaviours)
    {
        // Initialize all Image Targets
        foreach (DataSetTrackableBehaviour trackable in trackableBehaviours)
        {

            if (trackable.TrackableName == null)
            {
                Debug.LogError("Found Trackable without name.");
                continue;
            }

            int id = -1;
            
            if (trackable is ImageTargetBehaviour)
            {
                id = trackableGetId(trackable.TrackableName, mDataSetPtr);

                if (id >= 0)
                {
                    // Handle any changes to the image target in the scene
                    // that are not reflected in the config file
                    IntPtr sizePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Vector2)));
                    Marshal.StructureToPtr(((ImageTargetBehaviour)trackable).GetSize(), sizePtr, false);
                    // It is safe to assume that at loading stage the data set is not active.
                    imageTargetSetSize(mDataSetPtr, trackable.TrackableName, sizePtr);
                    Marshal.FreeHGlobal(sizePtr);
                }
            }
            else if (trackable is MultiTargetBehaviour)
            {
                id = trackableGetId(trackable.TrackableName, mDataSetPtr);
            }
            else
            {
                id = -1;
            }

            if (id >= 0)
            {
                trackable.InitializeID(id);
                if (!mTrackableBehaviourDict.ContainsKey(id))
                {
                    mTrackableBehaviourDict[id] = trackable;
                    Debug.Log("Found Trackable named " + trackable.TrackableName +
                                " with id " + trackable.TrackableID);
                }
            }
        }
    }


    private void AddVirtualButtons(VirtualButtonBehaviour[] vbBehaviours)
    {
        for (int i = 0; i < vbBehaviours.Length; ++i)
        {
            VirtualButtonBehaviour virtualButton = vbBehaviours[i];

            if (virtualButton.VirtualButtonName == null)
            {
                Debug.LogError("VirtualButton at " + i +
                                " has no name.");
                continue;
            }

            ImageTargetBehaviour imageTarget = virtualButton.GetImageTarget();

            if (imageTarget == null)
            {
                Debug.LogError("VirtualButton named " +
                                virtualButton.VirtualButtonName +
                                " is not attached to an ImageTarget.");
                continue;
            }

            // Image Target is not part of this data set.
            if (!imageTarget.References(this))
            {
                continue;
            }

            int id = virtualButtonGetId(DataSetPtr, imageTarget.TrackableName,
                        virtualButton.VirtualButtonName);

            if (id == -1)
            {
                // Create the virtual button
                if (RegisterVirtualButton(virtualButton, imageTarget.TrackableName))
                {
                    Debug.Log("Successfully created virtual button " +
                              virtualButton.VirtualButtonName +
                              " at startup");
                    virtualButton.UnregisterOnDestroy = true;
                    id = virtualButtonGetId(DataSetPtr, imageTarget.TrackableName,
                                virtualButton.VirtualButtonName);
                }
                else
                {
                    Debug.LogError("Failed to create virtual button " +
                                   virtualButton.VirtualButtonName +
                                   " at startup");
                }
            }

            if (id != -1)
            {
                //  Duplicate check:
                if (!mVBBehaviourDict.ContainsKey(id))
                {
                    // OK:
                    virtualButton.InitializeID(id);
                    mVBBehaviourDict.Add(id, virtualButton);
                    Debug.Log("Found VirtualButton named " +
                            virtualButton.VirtualButtonName + " with id " +
                            virtualButton.ID);

                    // Handle any changes to the virtual button in the scene
                    // that are not reflected in the config file
                    virtualButton.UpdatePose();
                    if (!virtualButton.UpdateAreaRectangle(this) ||
                        !virtualButton.UpdateSensitivity(this))
                    {
                        Debug.LogError("Failed to update virtual button " +
                                       virtualButton.VirtualButtonName +
                                       " at startup");
                    }
                    else
                    {
                        Debug.Log("Updated virtual button " +
                                  virtualButton.VirtualButtonName +
                                  " at startup");
                    }
                }
            }
        }
    }


    private void CreateImageTargets()
    {
        // Allocate array for all Image Targets.
        int numImageTargets = dataSetGetNumTrackableType(
            (int)TrackableBehaviour.TrackableType.IMAGE_TARGET,
            mDataSetPtr);
        IntPtr imageTargetDataPtr =
            Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImageTargetData)) * numImageTargets);

        // Copy Image Target properties from native.
        if (dataSetGetTrackablesOfType((int)TrackableBehaviour.TrackableType.IMAGE_TARGET,
                                       imageTargetDataPtr, numImageTargets,
                                       mDataSetPtr) == 0)
        {
            Debug.LogError("Could not create Image Target Behaviours");
            return;
        }

        // Create Image Target Behaviours.
        for (int i = 0; i < numImageTargets; ++i)
        {
            IntPtr trackablePtr = new IntPtr(imageTargetDataPtr.ToInt32() + i *
                    Marshal.SizeOf(typeof(ImageTargetData)));
            ImageTargetData trackableData = (ImageTargetData)
                    Marshal.PtrToStructure(trackablePtr, typeof(ImageTargetData));
            
            // Do not overwrite existing Trackables.
            if (mTrackableBehaviourDict.ContainsKey(trackableData.id))
            {
                continue;
            }

            // QCAR support names up to 64 characters in length, but here we allocate 
            // a slightly larger buffer:
            int nameLength = 128;
            System.Text.StringBuilder trackableName = new System.Text.StringBuilder(nameLength);
            dataSetGetTrackableName(mDataSetPtr, trackableData.id, trackableName, nameLength);
            
            ImageTargetBehaviour itb = CreateImageTarget(trackableData.id,
                trackableName.ToString(), trackableData.size);

            // Create Virtual Buttons for this Image Target.
            CreateVirtualButtons(itb);

            // Add newly created Image Target to dictionary.
            mTrackableBehaviourDict[trackableData.id] = itb;
        }

        Marshal.FreeHGlobal(imageTargetDataPtr);
    }


    private void CreateVirtualButtons(ImageTargetBehaviour itb)
    {
        // Allocate array for all Image Targets.
        int numVirtualButtons = imageTargetGetNumVirtualButtons(mDataSetPtr, itb.TrackableName);
        IntPtr virtualButtonDataPtr =
            Marshal.AllocHGlobal(Marshal.SizeOf(typeof(QCARManager.VirtualButtonData)) * numVirtualButtons);
        IntPtr rectangleDataPtr =
            Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VirtualButtonBehaviour.RectangleData)) * numVirtualButtons);

        // Copy Virtual Button data from native.
        imageTargetGetVirtualButtons(virtualButtonDataPtr,
                                     rectangleDataPtr,
                                     numVirtualButtons,
                                     mDataSetPtr,
                                     itb.TrackableName);

        for (int i = 0; i < numVirtualButtons; ++i)
        {
            IntPtr vbPtr = new IntPtr(virtualButtonDataPtr.ToInt32() + i *
                    Marshal.SizeOf(typeof(QCARManager.VirtualButtonData)));
            QCARManager.VirtualButtonData vbData = (QCARManager.VirtualButtonData)
                    Marshal.PtrToStructure(vbPtr, typeof(QCARManager.VirtualButtonData));

            // Do not overwrite existing Virtual Buttons.
            if (mVBBehaviourDict.ContainsKey(vbData.id))
            {
                continue;
            }

            IntPtr rectPtr = new IntPtr(rectangleDataPtr.ToInt32() + i *
                    Marshal.SizeOf(typeof(VirtualButtonBehaviour.RectangleData)));
            VirtualButtonBehaviour.RectangleData rectData = (VirtualButtonBehaviour.RectangleData)
                    Marshal.PtrToStructure(rectPtr, typeof(VirtualButtonBehaviour.RectangleData));

            // QCAR support names up to 64 characters in length, but here we allocate 
            // a slightly larger buffer:
            int nameLength = 128;
            System.Text.StringBuilder vbName = new System.Text.StringBuilder(nameLength);
            if (imageTargetGetVirtualButtonName(mDataSetPtr, itb.TrackableName,
                    i, vbName, nameLength) == 0)
            {
                Debug.LogError("Failed to get virtual button name.");
                continue;
            }
            
            VirtualButtonBehaviour vbb = CreateVirtualButton(
                vbData.id, vbName.ToString(),
                new Vector2(rectData.leftTopX, rectData.leftTopY),
                new Vector2(rectData.rightBottomX, rectData.rightBottomY),
                itb);

            mVBBehaviourDict.Add(vbData.id, vbb);
        }

        Marshal.FreeHGlobal(virtualButtonDataPtr);
        Marshal.FreeHGlobal(rectangleDataPtr);
    }


    private void CreateMultiTargets()
    {
        // Allocate array for all Multi Targets.
        int numMultiTargets = dataSetGetNumTrackableType(
            (int)TrackableBehaviour.TrackableType.MULTI_TARGET,
            mDataSetPtr);
        IntPtr multiTargetDataPtr =
            Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MultiTargetData)) * numMultiTargets);

        // Copy Multi Target properties from native.
        if (dataSetGetTrackablesOfType((int)TrackableBehaviour.TrackableType.MULTI_TARGET,
                                       multiTargetDataPtr, numMultiTargets,
                                       mDataSetPtr) == 0)
        {
            Debug.LogError("Could not create Multi Target Behaviours");
            return;
        }

        // Create Multi Target Behaviours.
        for (int i = 0; i < numMultiTargets; ++i)
        {
            IntPtr trackablePtr = new IntPtr(multiTargetDataPtr.ToInt32() + i *
                    Marshal.SizeOf(typeof(MultiTargetData)));
            MultiTargetData trackableData = (MultiTargetData)
                    Marshal.PtrToStructure(trackablePtr, typeof(MultiTargetData));

            // Do not overwrite existing Trackables.
            if (mTrackableBehaviourDict.ContainsKey(trackableData.id))
            {
                continue;
            }

            // QCAR support names up to 64 characters in length, but here we allocate 
            // a slightly larger buffer:
            int nameLength = 128;
            System.Text.StringBuilder trackableName = new System.Text.StringBuilder(nameLength);
            dataSetGetTrackableName(mDataSetPtr, trackableData.id, trackableName, nameLength);

            MultiTargetBehaviour mtb = CreateMultiTarget(trackableData.id, trackableName.ToString());

            // Add newly created Multi Target to dictionary.
            mTrackableBehaviourDict[trackableData.id] = mtb;
        }

        Marshal.FreeHGlobal(multiTargetDataPtr);
    }


    private bool IsTrackableInArray(DataSetTrackableBehaviour[] trackables, string trackableName)
    {
        bool trackableInArray = false;

        foreach (DataSetTrackableBehaviour trackable in trackables)
        {
            if (mPath == trackable.DataSetPath &&
                mStorageType == trackable.DataSetStorageType)
            {
                if (trackableName == trackable.TrackableName)
                {
                    trackableInArray = true;
                }
            }
        }
        
        return trackableInArray;
    }


    private ImageTargetBehaviour CreateImageTarget(int id,
                                                   string itName,
                                                   Vector2 itSize)
    {
        GameObject imageTargetObject = new GameObject();
        ImageTargetBehaviour newITB =
            imageTargetObject.AddComponent<ImageTargetBehaviour>();

        Debug.Log("Creating Image Target with values: " +
                  "\n ID:           " + id +
                  "\n Name:         " + itName +
                  "\n Path:         " + this.Path +
                  "\n Storage Type: " + this.FileStorageType.ToString() +
                  "\n Size:         " + itSize.x + "x" + itSize.y);

        // Set Image Target attributes.
        newITB.InitializeID(id);
        newITB.TrackableName = itName;
        newITB.DataSetPath = this.Path;
        newITB.DataSetStorageType = this.FileStorageType;
        newITB.transform.localScale = new Vector3(itSize.x, 1.0f, itSize.y);
        newITB.CorrectScale();
        newITB.AspectRatio = itSize[1] / itSize[0];

        return newITB;
    }


    private MultiTargetBehaviour CreateMultiTarget(int id,
                                                   string mtName)
    {
        GameObject multiTargetObject = new GameObject();
        MultiTargetBehaviour newMTB =
            multiTargetObject.AddComponent<MultiTargetBehaviour>();

        Debug.Log("Creating Multi Target with values: " +
          "\n ID:           " + id +
          "\n Name:         " + mtName +
          "\n Path:         " + this.Path +
          "\n Storage Type: " + this.FileStorageType.ToString());

        // Set Multi Target attributes.
        newMTB.InitializeID(id);
        newMTB.TrackableName = mtName;
        newMTB.DataSetPath = this.Path;
        newMTB.DataSetStorageType = this.FileStorageType;

        return newMTB;
    }


    private VirtualButtonBehaviour CreateVirtualButton(int id,
                                                       string vbName,
                                                       Vector2 topLeft,
                                                       Vector2 bottomRight,
                                                       ImageTargetBehaviour itb)
    {
        GameObject virtualButtonObject = new GameObject(vbName);
        VirtualButtonBehaviour newVBB =
            virtualButtonObject.AddComponent<VirtualButtonBehaviour>();

        // We need to set the Image Target as a parent BEFORE we set the size
        // of the Virtual Button.
        newVBB.transform.parent = itb.transform;

        Debug.Log("Creating Virtual Button with values: " +
                  "\n ID:           " + id +
                  "\n Name:         " + vbName +
                  "\n Rectangle:    " + topLeft.x + "," +
                                        topLeft.y + "," +
                                        bottomRight.x + "," +
                                        bottomRight.y);

        newVBB.InitializeID(id);
        newVBB.InitializeName(vbName);
        newVBB.SetPosAndScaleFromButtonArea(topLeft, bottomRight);
        // This button is part of a data set and should therefore not be
        // unregistered in native only because the Unity object is destroyed.
        newVBB.UnregisterOnDestroy = false;

        return newVBB;
    }

    #endregion // PRIVATE_METHODS



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetExists(string relativePath, int storageType);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetNumTrackables(IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray,
                                                         int trackableDataArrayLength, IntPtr dataSetPtr);
    
    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int dataSetGetTrackableName(IntPtr dataSetPtr, int trackableId,
                                                        System.Text.StringBuilder trackableName,
                                                        int nameMaxLength);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int trackableGetId(string nTrackableName, IntPtr dataSetPtr);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetCreateVirtualButton(IntPtr dataSetPtr,
        string trackableName, string virtualButtonName, [In, Out]IntPtr rectData);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetDestroyVirtualButton(IntPtr dataSetPtr,
        string trackableName, string virtualButtonName);
    
    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetGetNumVirtualButtons(IntPtr dataSetPtr,
        string trackableName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetGetVirtualButtons(
        [In, Out]IntPtr virtualButtonDataArray,
        [In, Out]IntPtr rectangleDataArray,
        int virtualButtonDataArrayLength,
        IntPtr dataSetPtr, string trackableName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int imageTargetGetVirtualButtonName(
        IntPtr dataSetPtr,
        string trackableName,
        int idx,
        System.Text.StringBuilder vbName,
        int nameMaxLength);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonGetId(IntPtr dataSetPtr,
        string trackableName, string virtualButtonName);

#else // !UNITY_EDITOR

    private static int dataSetExists(string relativePath, int storageType) { return 0; }

    private static int dataSetLoad(string relativePath, int storageType, IntPtr dataSetPtr) { return 0; }

    private static int dataSetGetNumTrackables(IntPtr dataSetPtr) { return 0; }

    private static int dataSetGetNumTrackableType(int trackableType, IntPtr dataSetPtr) { return 0; }

    private static int dataSetGetTrackablesOfType(int trackableType, [In, Out] IntPtr trackableDataArray,
                                                  int trackableDataArrayLength, IntPtr dataSetPtr) { return 0; }
    
    private static int dataSetGetTrackableName(IntPtr dataSetPtr, int trackableId,
                                                System.Text.StringBuilder trackableName,
                                                int nameMaxLength) { return 0; }

    private static int trackableGetId(string nTrackableName, IntPtr dataSetPtr) { return 0; }

    private static int imageTargetSetSize(IntPtr dataSetPtr, string trackableName, [In, Out]IntPtr size) { return 0; }

    private static int imageTargetCreateVirtualButton(IntPtr dataSetPtr, string trackableName,
                         string virtualButtonName, [In, Out]IntPtr rectData) { return 0; }

    private static int imageTargetDestroyVirtualButton(IntPtr dataSetPtr, string trackableName,
                         string virtualButtonName) { return 0; }

    private static int imageTargetGetNumVirtualButtons(IntPtr dataSetPtr, string trackableName) { return 0; }

    private static int imageTargetGetVirtualButtons([In, Out]IntPtr virtualButtonDataArray,
                                                    [In, Out]IntPtr rectangleDataArray,
                                                    int virtualButtonDataArrayLength,
                                                    IntPtr dataSetPtr, string trackableName) { return 0; }

    private int imageTargetGetVirtualButtonName(IntPtr dataSetPtr, string trackableName,
                                    int idx, System.Text.StringBuilder vbName, int nameMaxLength) { return 0; }

    private static int virtualButtonGetId(IntPtr dataSetPtr,
        string trackableName, string virtualButtonName) { return 0; }

#endif // !UNITY_EDITOR

    #endregion // NATIVE_FUNCTIONS
}