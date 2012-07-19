/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/


// Helper structure for defining macros.
struct QCARMacros
{
    #region PUBLIC_MEMBER_VARIABLES

#if UNITY_ANDROID
    public const string PLATFORM_DLL = "QCARWrapper";
#elif UNITY_IPHONE
    public const string PLATFORM_DLL = "__Internal";
#else
    public const string PLATFORM_DLL = "QCARWrapper";
#endif

    #endregion // PUBLIC_MEMBER_VARIABLES
}
