/*
 * Copyright 2024 (c) Leia Inc.  All rights reserved.
 *
 * NOTICE:  All information contained herein is, and remains
 * the property of Leia Inc. and its suppliers, if any.  The
 * intellectual and technical concepts contained herein are
 * proprietary to Leia Inc. and its suppliers and may be covered
 * by U.S. and Foreign Patents, patents in process, and are
 * protected by trade secret or copyright law.  Dissemination of
 * this information or reproduction of this materials strictly
 * forbidden unless prior written permission is obtained from
 * Leia Inc.
 */

/*!
 * A demo to read display parameters.
 * 
 * Attach this script to a GameObject in unity and use the SimulatedReality API.
 */

using System;
using UnityEngine;
#if !UNITY_EDITOR && PLATFORM_STANDALONE_WIN
using SimulatedReality;
#endif
namespace SRDemo
{
    class DisplayParameters : MonoBehaviour
    {        
        public int resolutionHeight;
        public int resolutionWidth;
        public int physicalResolutionHeight;
        public int physicalResolutionWidth;
        public float getPhysicalSizeHeight;
        public float getPhysicalSizeWidth;
        public float getDotPitch;

        private void Start()
        {
            SRUnity.SRCore.OnContextChanged += OnContextChanged;
            OnContextChanged(SRUnity.SRContextChangeReason.Unknown);
        }

        private void Destroy()
        {
            SRUnity.SRCore.OnContextChanged -= OnContextChanged;
        }

        private void OnContextChanged(SRUnity.SRContextChangeReason contextChangeReason)
        {
            SRUnity.SRUtility.Debug(string.Format("OnContextChange: {0}", contextChangeReason.ToString()));
            resolutionHeight = SRUnity.SRCore.Instance.getResolution().y;
            resolutionWidth = SRUnity.SRCore.Instance.getResolution().x;
            physicalResolutionHeight = SRUnity.SRCore.Instance.getPhysicalResolution().y;
            physicalResolutionWidth = SRUnity.SRCore.Instance.getPhysicalResolution().x;
            getPhysicalSizeHeight = SRUnity.SRCore.Instance.getPhysicalSize().y;
            getPhysicalSizeWidth = SRUnity.SRCore.Instance.getPhysicalSize().x;
            getDotPitch = SRUnity.SRCore.Instance.getDotPitch();

            // Print the screen parameters
            Debug.Log("getResolutionHeight: " + resolutionHeight);
            Debug.Log("getResolutionWidth: " + resolutionWidth);
            Debug.Log("getPhysicalResolutionHeight: " + physicalResolutionHeight);
            Debug.Log("getPhysicalResolutionWidth: " + physicalResolutionWidth);
            Debug.Log("getPhysicalSizeHeight: " + getPhysicalSizeHeight);
            Debug.Log("getPhysicalSizeWidth: " + getPhysicalSizeWidth);
            Debug.Log("getDotPitch: " + getDotPitch);
        }
    }
}
