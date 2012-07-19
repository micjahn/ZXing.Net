/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

// A trackable behaviour to represent a flat natural feature target.
public class ImageTargetBehaviour : DataSetTrackableBehaviour
{

    #region PROPERTIES

    // The aspect ratio of the target.
    public float AspectRatio
    {
        get
        {
            return mAspectRatio;
        }

        set
        {
            // Only to be called by DataSet.CreateImageTarget
            mAspectRatio = value;
        }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    [SerializeField]
    [HideInInspector]
    private float mAspectRatio;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region CONSTRUCTION

    public ImageTargetBehaviour()
    {
        mTrackableType = TrackableType.IMAGE_TARGET;
        mAspectRatio = 1.0f;
    }

    #endregion // CONSTRUCTION



    #region PUBLIC_METHODS

    // Returns the Virtual Button with the given name.
    // Returns null if the button cannot be found.
    public VirtualButtonBehaviour GetVirtualButton(string name)
    {
        VirtualButtonBehaviour[] vbs =
                            GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < vbs.Length; ++i)
        {
            if (vbs[i].VirtualButtonName.Equals(name))
            {
                return vbs[i];
            }
        }

        // Not found:
        return null;
    }


    // This method creates a Virtual Button and adds it to this Image Target as
    // a direct child.
    public VirtualButtonBehaviour CreateVirtualButton(string vbName,
                                                      Vector2 position,
                                                      Vector2 size)
    {
        GameObject virtualButtonObject = new GameObject(vbName);
        VirtualButtonBehaviour newVBB =
            virtualButtonObject.AddComponent<VirtualButtonBehaviour>();

        // Add Virtual Button to its parent game object
        virtualButtonObject.transform.parent = this.transform;

        // Set Virtual Button attributes
        newVBB.InitializeName(vbName);
        newVBB.transform.localScale = new Vector3(size.x, 1.0f, size.y);
        newVBB.transform.localPosition = new Vector3(position.x, 1.0f,
                                                        position.y);

        // Only register the virtual button with the qcarBehaviour at run-time:
        if (!Application.isEditor)
        {
            ImageTracker imageTracker = (ImageTracker)
                TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
            int numDataSets = imageTracker.GetNumDataSets();

            for (int i = 0; i < numDataSets; ++i)
            {
                DataSet dataSet = imageTracker.GetDataSet(i);
                if (References(dataSet))
                {
                    if (!dataSet.RegisterVirtualButton(newVBB,
                                                       this.TrackableName))
                    {
                        Debug.LogError("Could not register Virtual Button.");
                        GameObject.Destroy(virtualButtonObject);
                        return null;
                    }
                }
            }
        }
        
        // If we manually register the button it should be unregistered if the
        // Unity object is destroyed.
        newVBB.UnregisterOnDestroy = true;
        
        return newVBB;
    }


    // This methods adds the Virtual Button as a child of "immediateParent".
    // Returns null if "immediateParent" is not an Image Target or a child of an
    // Image Target.
    public static VirtualButtonBehaviour CreateVirtualButton(string vbName,
                                                  Vector2 localScale,
                                                  GameObject immediateParent)
    {
        GameObject virtualButtonObject = new GameObject(vbName);
        VirtualButtonBehaviour newVBB =
            virtualButtonObject.AddComponent<VirtualButtonBehaviour>();

        GameObject rootParent = immediateParent.transform.root.gameObject;
        ImageTargetBehaviour parentImageTarget =
            rootParent.GetComponentInChildren<ImageTargetBehaviour>();

        if (parentImageTarget == null)
        {
            Debug.LogError("Could not create Virtual Button. " +
                           "immediateParent\"immediateParent\" object is not " +
                           "an Image Target or a child of one.");
            GameObject.Destroy(virtualButtonObject);
            return null;
        }

        // Add Virtual Button to its parent game object
        virtualButtonObject.transform.parent = immediateParent.transform;

        // Set Virtual Button attributes
        newVBB.InitializeName(vbName);
        newVBB.transform.localScale = new Vector3(localScale[0], 1.0f, localScale[1]); 

        ImageTracker imageTracker = (ImageTracker)
            TrackerManager.Instance.GetTracker(Tracker.Type.IMAGE_TRACKER);
        int numDataSets = imageTracker.GetNumDataSets();

        bool registerSuccess = false;

        for (int i = 0; i < numDataSets; ++i)
        {
            DataSet dataSet = imageTracker.GetDataSet(i);
            if (parentImageTarget.References(dataSet))
            {
                registerSuccess = dataSet.RegisterVirtualButton(newVBB,
                    parentImageTarget.TrackableName);
            }
        }

        if (!registerSuccess)
        {
            Debug.LogError("Could not register Virtual Button.");
            GameObject.Destroy(virtualButtonObject);
            return null;
        }
        else
        {
            Debug.Log("Registered Virtual Button successfully.");
        }

        // If we manually register the button it should be unregistered if the
        // Unity object is destroyed.
        newVBB.UnregisterOnDestroy = true;

        return newVBB;
    }


    // Destroys the virtual button with the given name.
    public void DestroyVirtualButton(string vbName)
    {
        VirtualButtonBehaviour[] virtualButtons =
            this.GetComponentsInChildren<VirtualButtonBehaviour>();

        foreach (VirtualButtonBehaviour vb in virtualButtons)
        {
            if (vb.VirtualButtonName == vbName)
            {
                // Unregister pre-existing buttons when explicitly destroyed
                vb.UnregisterOnDestroy = true;
                GameObject.Destroy(vb.gameObject);
                return;
            }
        }
    }


    // Returns the size of this target in scene units:
    public Vector2 GetSize()
    {
        if (mAspectRatio <= 1.0f)
        {
            return new Vector2(transform.localScale.x,
                                transform.localScale.x * mAspectRatio);
        }
        else
        {
            return new Vector2(transform.localScale.x / mAspectRatio,
                                transform.localScale.x);
        }
    }


    // Scales the Trackable uniformly
    public override bool CorrectScale()
    {
        bool scaleChanged = false;

        for (int i = 0; i < 3; ++i)
        {
            // Force uniform scale:
            if (this.transform.localScale[i] != this.mPreviousScale[i])
            {
                this.transform.localScale =
                    new Vector3(this.transform.localScale[i],
                                this.transform.localScale[i],
                                this.transform.localScale[i]);

                this.mPreviousScale = this.transform.localScale;
                scaleChanged = true;
                break;
            }
        }

        return scaleChanged;
    }

    #endregion // PUBLIC_METHODS
}
