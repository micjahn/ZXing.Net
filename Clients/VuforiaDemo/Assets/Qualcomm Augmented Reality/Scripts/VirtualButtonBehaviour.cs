/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// This behaviour associates a Virtual Button with a game object. Use the
// functionality in ImageTargetBehaviour to create and destroy Virtual Buttons
// at run-time.
public class VirtualButtonBehaviour : MonoBehaviour
{
    #region NESTED

    // Sensitivity of press detection.
    public enum Sensitivity
    {
        HIGH,           // Fast detection.
        MEDIUM,         // Balanced between fast and robust.
        LOW             // Robust detection.
    }

    // This struct defines the 2D coordinates of a rectangle. It is used for
    // rectangular Virtual Button definitions.
    // The struct is used internally by the VirtualButtonBehaviour.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RectangleData
    {
        public float leftTopX;
        public float leftTopY;
        public float rightBottomX;
        public float rightBottomY;
    }

    #endregion // NESTED



    #region PROPERTIES

    // The names of this virtual button.
    public string VirtualButtonName
    {
        get { return mName; }
    }
    
    // The ID of this virtual button.
    public int ID
    {
        get { return mID; }
    }

    // Returns true if this button is currently pressed.
    public bool Pressed
    {
        get { return mPressed; }
    }

    // The sensitivity of this virtual button. This is a trade off between fast
    // detection and robustness again accidental occlusion.
    public Sensitivity SensitivitySetting
    {
        get
        {
            return mSensitivity;
        }
        set
        {
            mSensitivity = value;
            mSensitivityDirty = true;
        }
    }

    // Unregistering Virtual Buttons should only be done if they have been 
    // registered at runtime. This property is automatically set by
    // ImageTargetBehaviour on registration.
    public bool UnregisterOnDestroy
    {
        get
        {
            return mUnregisterOnDestroy;
        }

        set
        {
            mUnregisterOnDestroy = value;
        }
    }

    #endregion // PROPERTIES



    #region PUBLIC_MEMBER_VARIABLES

    // Constants:
    public const float          TARGET_OFFSET       = 0.001f;
    public const Sensitivity    DEFAULT_SENSITIVITY = Sensitivity.LOW;

    #endregion // PUBLIC_MEMBER_VARIABLES



    #region PRIVATE_MEMBER_VARIABLES

    [SerializeField]
    [HideInInspector]
    private string mName;

    [SerializeField]
    [HideInInspector]
    private Sensitivity mSensitivity;

    private bool mSensitivityDirty;
    private bool mPreviouslyEnabled;
    private bool mPressed;
    private int mID;
    private List<IVirtualButtonEventHandler> mHandlers = null;
    private Vector2 mLeftTop;
    private Vector2 mRightBottom;
    private bool mInitialized;
    private bool mUnregisterOnDestroy;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region EDITOR_ONLY_MEMBER_VARIABLES

    [SerializeField]
    [HideInInspector]
    public bool mHasUpdatedPose = false;

    [SerializeField]
    [HideInInspector]
    public Matrix4x4 mPrevTransform = Matrix4x4.zero;

    [SerializeField]
    [HideInInspector]
    public GameObject mPrevParent = null;

    #endregion // EDITOR_ONLY_MEMBER_VARIABLES



    #region CONSTRUCTION

    public VirtualButtonBehaviour()
    {
        mName = "";
        mPressed = false;
        mSensitivity = DEFAULT_SENSITIVITY;
        mSensitivityDirty = false;
        mHandlers = new List<IVirtualButtonEventHandler>();
        mInitialized = false;
        mHasUpdatedPose = false;
    }

    #endregion // CONSTRUCTION

    
    
    #region PUBLIC_METHODS

    // Registers an event handler with this Virtual Button which will be called
    // when a state changed is detected.
    public void RegisterEventHandler(IVirtualButtonEventHandler eventHandler)
    {
        mHandlers.Add(eventHandler);
    }


    // Registers an event handler with this Virtual Button which will be called
    // when a state changed is detected.
    // Returns true on success. False otherwise.
    public bool UnregisterEventHandler(IVirtualButtonEventHandler eventHandler)
    {
        return mHandlers.Remove(eventHandler);
    }


    // UpdatePose() is called each frame to ensure the virtual button is clamped
    // to the image target plane and remains axis-aligned with respect to the
    // target. Return true if the defining area of the virtual button has
    // changed, false otherwise.
    public bool UpdatePose()
    {
        // The image target to which the button belongs:
        ImageTargetBehaviour itb = this.GetImageTarget();

        // If there is no image target we return:
        if (itb == null)
        {
            return false;
        }

        // We explicitly disallow any objects with non-uniform scaling in the
        // object hierachy of the virtual button. Combined with a rotation
        // this would result in skewing the virtual button.
        Transform t = transform.parent;
        while (t != null)
        {
            if (t.localScale[0] != t.localScale[1] ||
                t.localScale[0] != t.localScale[2])
            {
                Debug.LogWarning("Detected non-uniform scale in virtual " +
                    " button object hierarchy. Forcing uniform scaling of " +
                    "object '" + t.name + "'.");

                //  Force uniform scale:
                t.localScale = new Vector3(t.localScale[0],t.localScale[0],
                                            t.localScale[0]);
            }
            t = t.parent;
        }

        // Remember we have updated once:
        mHasUpdatedPose = true;

        // Clamp to center of parent object:
        if (transform.parent != null &&
            transform.parent.gameObject != itb.gameObject)
        {
            transform.localPosition = Vector3.zero;
        }

        // Clamp position to image target plane:
        Vector3 vbPosITSpace = itb.transform.InverseTransformPoint(
                                                    this.transform.position);
        
        // Set the y offset in Image Target space:
        vbPosITSpace.y = TARGET_OFFSET;
        Vector3 vbPosWorldSpace = itb.transform.TransformPoint(vbPosITSpace);
        this.transform.position = vbPosWorldSpace;

        // Clamp orientation to the image target plane:
        this.transform.rotation = itb.transform.rotation;

        // Update the button area:
        Vector2 leftTop, rightBottom;
        CalculateButtonArea(out leftTop, out rightBottom);
        
        // Change the button area only if the change is larger than a fixed
        // proportion of the image target size:
        float threshold = itb.transform.localScale[0] * 0.001f;
        
        if (!Equals(leftTop, mLeftTop, threshold) ||
            !Equals(rightBottom, mRightBottom, threshold))
        {
            // Area has changed significantly:
            mLeftTop = leftTop;
            mRightBottom = rightBottom;
            return true;
        }

        // Area has not changed significantly:
        return false;
    }


    // Calculates the 2D button area that the Virtual Button currently occupies
    // in the Image Target.
    // Returns true if the area was computed successfully. False otherwise.
    // Passes out the top left and bottom right position of the rectangle area.
    public bool CalculateButtonArea(out Vector2 topLeft,
                                    out Vector2 bottomRight)
    {
        // Error if we don't have an image target as a root:
        ImageTargetBehaviour itb = this.GetImageTarget();
        if (itb == null)
        {
            topLeft = bottomRight = Vector2.zero;
            return false;
        }

        Vector3 vbPosITSpace = itb.transform.InverseTransformPoint(
                                                this.transform.position);

        // The scale of the image Target:
        float itScale = itb.transform.lossyScale[0];

        // Scale the button position:
        Vector2 pos = new Vector2(vbPosITSpace[0] * itScale,
                                  vbPosITSpace[2] * itScale);

        // Scale the button area:
        Vector2 scale = new Vector2(this.transform.lossyScale[0],
                                    this.transform.lossyScale[2]);

        // Calculate top left and bottom right points:
        Vector2 radius = Vector2.Scale(scale * 0.5F, new Vector2(1.0f, -1.0f));

        topLeft = pos - radius;
        bottomRight = pos + radius;

        // Done:
        return true;
    }


    // Update the virtual button rect in native
    public bool UpdateAreaRectangle(DataSet dataSet)
    {
        if (dataSet == null)
        {
            Debug.LogError("Attempting to update a virtual button without a dataset");
            return false;
        }

        string trackableName = this.GetImageTarget().TrackableName;
        RectangleData rectData = new RectangleData();

        rectData.leftTopX = mLeftTop.x;
        rectData.leftTopY = mLeftTop.y;
        rectData.rightBottomX = mRightBottom.x;
        rectData.rightBottomY = mRightBottom.y;

        IntPtr rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(
                                typeof(RectangleData)));
        Marshal.StructureToPtr(rectData, rectPtr, false);
        int success = virtualButtonSetAreaRectangle(dataSet.DataSetPtr,
            trackableName, this.VirtualButtonName, rectPtr);
        Marshal.FreeHGlobal(rectPtr);

        if (success == 0)
        {
            Debug.LogError("Virtual Button area rectangle could not be set.");
            return false;
        }

        return true;
    }


    // Update sensitivity in native
    public bool UpdateSensitivity(DataSet dataSet)
    {
        if (dataSet == null)
        {
            Debug.LogError("Attempting to update a virtual button without a dataset");
            return false;
        }

        ImageTargetBehaviour itb = this.GetImageTarget();

        int success = virtualButtonSetSensitivity(dataSet.DataSetPtr,
                                                  itb.TrackableName,
                                                  this.VirtualButtonName,
                                                  (int)mSensitivity);

        if (success == 0)
        {
            Debug.LogError("Virtual Button sensitivity could not be set.");
            return false;
        }

        return true;
    }


    // Update enabled status in native
    public bool UpdateEnabled(DataSet dataSet)
    {
        if (dataSet == null)
        {
            Debug.LogError("Attempting to update a virtual button without an active dataset");
            return false;
        }

        ImageTargetBehaviour itb = this.GetImageTarget();
        if (itb == null)
        {
            Debug.LogError("Image Target not found. " +
                  "Virtual Button enabled value could not be set.");
            return false;
        }

        int success = virtualButtonSetEnabled(dataSet.DataSetPtr, itb.TrackableName,
                                        this.VirtualButtonName, enabled ? 1 : 0);

        if (success == 0)
        {
            Debug.LogError("Virtual Button enabled value could not be set.");
            return false;
        }

        return true;
    }


    // Called after the QCARBehaviour has updated.
    public void OnTrackerUpdated(bool pressed)
    {
        if (mPreviouslyEnabled != enabled)
        {
            mPreviouslyEnabled = enabled;
            UpdateEnabled(GetDataSet());
        }

        if (!enabled)
        {
            return;
        }

        // Trigger the appropriate callback if there was state change:
        if (mPressed != pressed && mHandlers != null)
        {
            if (pressed)
            {
                foreach (IVirtualButtonEventHandler handler in mHandlers)
                {
                    handler.OnButtonPressed(this);
                }
            }
            else
            {
                foreach (IVirtualButtonEventHandler handler in mHandlers)
                {
                    handler.OnButtonReleased(this);
                }
            }
        }

        // Cache pressed state:
        mPressed = pressed;
    }


    // Returns the Image Target that this Virtual Button is associated with.
    public ImageTargetBehaviour GetImageTarget()
    {
        if (transform.parent == null)
            return null;

        GameObject p = transform.parent.gameObject;

        while (p != null)
        {
            ImageTargetBehaviour itb = p.GetComponent<ImageTargetBehaviour>();
            if (itb != null)
            {
                return itb;
            }

            if (p.transform.parent == null)
            {
                // Not found:
                return null;
            }

            p = p.transform.parent.gameObject;
        }

        // Not found:
        return null;
    }


    // Initializes the Virtual Button ID. Should only be called by the
    // ImageTargetBehaviour or QCARBehaviour on initialization.
    public void InitializeID(int id)
    {
        mID = id;
    }


    // Initializes the Virtual Button name. Should only be called by the
    // ImageTargetBehaviour when a new Virtual Button is created.
    public void InitializeName(string name)
    {
        mName = name;
    }


    // Sets position and scale in the transform component of the Virtual Button
    // game object. The values are calculated from rectangle values (top-left
    // and bottom-right corners).
    // Returns false if Virtual Button is not child of an Image Target.
    public bool SetPosAndScaleFromButtonArea(Vector2 topLeft,
                                             Vector2 bottomRight)
    {
        // Error if we don't have an image target as a root:
        ImageTargetBehaviour itb = this.GetImageTarget();
        if (itb == null)
        {
            return false;
        }

        float itScale = itb.transform.lossyScale[0];

        Vector2 pos = (topLeft + bottomRight) * 0.5f;

        Vector2 scale = new Vector2(bottomRight[0] - topLeft[0],
                                    topLeft[1] - bottomRight[1]);

        Vector3 vbPosITSpace =
            new Vector3(pos[0] / itScale, VirtualButtonBehaviour.TARGET_OFFSET,
                        pos[1] / itScale);


        Vector3 vbScaleITSpace =
            new Vector3(scale[0],
                        (scale[0] + scale[1]) * 0.5f,
                        scale[1]);

        this.transform.position = itb.transform.TransformPoint(vbPosITSpace);

        // Image Target scale is canceled out (included in both scales)
        this.transform.localScale =
            vbScaleITSpace / this.transform.parent.lossyScale[0];

        // Done:
        return true;
    }

    #endregion // PUBLIC_METHODS



    #region UNITY_MONOBEHAVIOUR_METHODS

    // Overriding standard Unity MonoBehaviour methods.

    public void Update()
    {
        // Initalize the virtual button if we haven't done so yet:
        if (!mInitialized)
        {
            mLeftTop = Vector2.zero;
            mRightBottom = Vector2.zero;
            mInitialized = CalculateButtonArea(out mLeftTop, out mRightBottom);
        }
    }


    public void LateUpdate()
    {
        // Update the button pose:
        if (UpdatePose())
        {
            // Area has changed, update the QCAR trackable:
            UpdateAreaRectangle(GetDataSet());
        }

        // Update the sensitivity of the button if it has changed since the
        // last update:
        if (mSensitivityDirty)
        {
            if (UpdateSensitivity(GetDataSet()))
            {
                mSensitivityDirty = false;
            }
        }
    }


    public void OnDisable()
    {
        if (!Application.isEditor)
        {
            if (mPreviouslyEnabled != enabled)
            {
                mPreviouslyEnabled = enabled;
                UpdateEnabled(GetDataSet());
            }
    
            // Trigger the appropriate callback if there was state change:
            if (mPressed && mHandlers != null)
            {
                foreach (IVirtualButtonEventHandler handler in mHandlers)
                {
                    handler.OnButtonReleased(this);
                }
            }
    
            // Cache pressed state:
            mPressed = false;
        }
    }


    public void OnDestroy()
    {
        if (!Application.isEditor)
        {
            if (mUnregisterOnDestroy)
            {
                ImageTracker imageTracker = (ImageTracker)
                    TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
                int numDataSets = imageTracker.GetNumDataSets();

                for (int i = 0; i < numDataSets; ++i)
                {
                    DataSet dataSet = imageTracker.GetDataSet(i);
                    if (this.GetImageTarget().References(dataSet))
                    {
                        bool isActiveDataSet =
                            (dataSet == imageTracker.GetActiveDataSet());

                        if (isActiveDataSet)
                        {
                            imageTracker.DeactivateDataSet(dataSet);
                        }
                        if (dataSet.UnregisterVirtualButton(this,
                                this.GetImageTarget().TrackableName))
                        {
                            Debug.Log("Unregistering virtual button successfully");
                        }
                        else
                        {
                            Debug.LogError("Failed to unregister virtual button.");
                        }
                        if (isActiveDataSet)
                        {
                            imageTracker.ActivateDataSet(dataSet);
                        }
                    }
                }
            }
        }
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS
    
    private static bool Equals(Vector2 vec1, Vector2 vec2, float threshold)
    {
        Vector2 diff = vec1 - vec2;
        return (Math.Abs(diff.x) < threshold) && (Math.Abs(diff.y) < threshold);
    }

    private DataSet GetDataSet()
    {
        ImageTargetBehaviour itb = GetImageTarget();

        if (itb == null)
        {
            Debug.LogError("Virtual Button is not part of a data set.");
            return null;
        }

        DataSet container = null;

        ImageTracker imageTracker = (ImageTracker)
            TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);

        if (imageTracker != null)
        {
            int numDataSets = imageTracker.GetNumDataSets();
    
            for (int i = 0; i < numDataSets; ++i)
            {
                DataSet dataSet = imageTracker.GetDataSet(i);
                if (itb.References(dataSet))
                {
                    container = dataSet;
                }
            }
        }

        return container;
    }

    #endregion // PRIVATE_METHODS



    #region NATIVE_FUNCTIONS

#if !UNITY_EDITOR

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonSetEnabled(IntPtr dataSetPtr,
                                                      String trackableName,
                                                      String virtualButtonName,
                                                      int enabled);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonIsEnabled(IntPtr dataSetPtr,
                                                     String trackableName,
                                                     String virtualButtonName);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonSetSensitivity(IntPtr dataSetPtr,
                                                        String trackableName,
                                                        String virtualButtonName,
                                                        int sensitivity);

    [DllImport(QCARMacros.PLATFORM_DLL)]
    private static extern int virtualButtonSetAreaRectangle(IntPtr dataSetPtr,
                                                    String trackableName,
                                                    String virtualButtonName,
                                                    [In, Out]IntPtr rectData);
#else

    // Stubs used when running within the editor:

    private static int virtualButtonSetEnabled(IntPtr dataSetPtr,
                                                   String trackableName,
                                                   String virtualButtonName,
                                                   int enabled)
                                                        { return 1; }

    private static int virtualButtonIsEnabled(IntPtr dataSetPtr,
                                                      String trackableName,
                                                      String virtualButtonName)
                                                        { return 0; }

    private static int virtualButtonSetSensitivity(IntPtr dataSetPtr,
                                                         String trackableName,
                                                         String virtualButtonName,
                                                         int sensitivity)
                                                        { return 1; }

    private static int virtualButtonSetAreaRectangle(IntPtr dataSetPtr,
                                                     String trackableName,
                                                     String virtualButtonName,
                                                     [In, Out]IntPtr rectData)
                                                        { return 1; }
#endif

    #endregion // NATIVE_FUNCTIONS
}
