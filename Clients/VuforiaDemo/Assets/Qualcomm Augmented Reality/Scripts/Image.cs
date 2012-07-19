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

// An image - Used to expose the camera frame.
// 
// The image's pixel buffer can have a different size than the
// Wdith and Height properties report. This is e.g. the
// case when an image is used for rendering as a texture without
// non-power-of-two support.
// The real size of the image's pixel buffer can be queried using
// BufferWidth and BufferHeight. 
public class Image
{
    #region NESTED

    public enum PIXEL_FORMAT
    {
    UNKNOWN_FORMAT = 0,         // Unknown format - default pixel type for
                                // undefined images
    RGB565 = 1,                 // A color pixel stored in 2 bytes using 5
                                // bits for red, 6 bits for green and 5 bits
                                // for blue
    RGB888 = 2,                 // A color pixel stored in 3 bytes using
                                // 8 bits each
    GRAYSCALE = 4,              // A grayscale pixel stored in one byte
    YUV = 8,                    // A color pixel stored in 12 or more bits
                                // using Y, U and V planes
    };

    #endregion // NESTED



    #region PROPERTIES

    // The width of the image in pixels. Note the pixel buffer can be
    // wider than this.
    public int Width
    {
        get { return mWidth; }
        set { mWidth = value; }
    }

    // The height of the image in pixels. Note that the pixel buffer
    // can be higher than this.
    public int Height
    {
        get { return mHeight; }
        set { mHeight = value; }
    }

    // The number bytes from one row of pixels to the next row.
    public int Stride
    {
        get { return mStride; }
        set { mStride = value; }
    }

    // The number of pixel columns that fit into the pixel buffer.
    public int BufferWidth
    {
        get { return mBufferWidth; }
        set { mBufferWidth = value; }
    }

    // The number of rows that fit into the pixel buffer.
    public int BufferHeight
    {
        get { return mBufferHeight; }
        set { mBufferHeight = value; }
    }

    // The pixel format of the image.
    public PIXEL_FORMAT PixelFormat
    {
        get { return mPixelFormat; }
        set { mPixelFormat = value; }
    }

    // The pixel data.
    public byte[] Pixels
    {
        get { return mData; }
        set { mData = value; }
    }

    // The unmanaged memory buffer used for marshaling.
    public IntPtr UnmanagedData
    {
        get { return mUnmanagedData; }
        set { mUnmanagedData = value; }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    // Header:
    private int mWidth;
    private int mHeight;
    private int mStride;
    private int mBufferWidth;
    private int mBufferHeight;
    private PIXEL_FORMAT mPixelFormat;

    // Data:
    private byte[] mData;
    private IntPtr mUnmanagedData;
    private bool mDataSet;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region CONSTRUCTION

    public Image()
    {
        mWidth = 0;
        mHeight = 0;
        mStride = 0;
        mBufferWidth = 0;
        mBufferHeight = 0;
        mPixelFormat = PIXEL_FORMAT.UNKNOWN_FORMAT;

        mData = null;
        mUnmanagedData = IntPtr.Zero;
        mDataSet = false;
    }


    ~Image()
    {
        Marshal.FreeHGlobal(mUnmanagedData);
        mUnmanagedData = IntPtr.Zero;
    }

    #endregion // CONSTRUCTION



    #region PUBLIC_METHODS

    // Returns true if this image has been fully initiailzed with valid data.
    // False otherwise.
    public bool IsValid()
    {
        return (mWidth > 0) && (mHeight > 0) && (mStride > 0) &&
            (mBufferWidth > 0) && (mBufferHeight > 0) && (mData != null) &&
            mDataSet; 
    }


    // Called by the QCARBehaviour when the unmanaged data buffer has been
    // filled:
    public void CopyPixelsFromUnmanagedBuffer()
    {
        if (mData == null || mUnmanagedData == IntPtr.Zero)
        {
            Debug.LogError("Image: Cannot copy image image data.");
            return;
        }

        Marshal.Copy(mUnmanagedData, mData, 0, mBufferWidth * mBufferHeight);
        mDataSet = true;
    }

    #endregion // PUBLIC_METHODS
}
