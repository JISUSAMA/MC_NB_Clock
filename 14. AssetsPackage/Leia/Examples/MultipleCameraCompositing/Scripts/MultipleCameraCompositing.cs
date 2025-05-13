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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using LeiaUnity;

namespace LeiaUnity.Examples
{
    public class MultipleCameraCompositing : MonoBehaviour
    {
        private LeiaDisplay leiaDisplay;
        [SerializeField] private LeiaDisplay firstToRenderLeiaDisplay;
        private bool initialized;
        private int lastnunViews;

        void Start()
        {
            leiaDisplay = GetComponent<LeiaDisplay>();
        }

        public void LateUpdate()
        {
            if (initialized && leiaDisplay.GetEyeCamera(0).targetTexture != firstToRenderLeiaDisplay.GetEyeCamera(0).targetTexture ||
                initialized && leiaDisplay.GetEyeCamera(1).targetTexture != firstToRenderLeiaDisplay.GetEyeCamera(1).targetTexture)
            {
                initialized = false;
            }
            if (!initialized && firstToRenderLeiaDisplay != null || lastnunViews != leiaDisplay.GetViewCount())
            {
                Debug.Log("share textures");
                for (int i = 0; i < firstToRenderLeiaDisplay.GetViewCount(); i++)
                {
                    if (firstToRenderLeiaDisplay.GetEyeCamera(i).targetTexture == null)
                    {
                        return;
                    }
                    firstToRenderLeiaDisplay.HeadCamera.depth = 0;
                    leiaDisplay.HeadCamera.depth = 1;
                    leiaDisplay.HeadCamera.clearFlags = CameraClearFlags.Depth;
                    leiaDisplay.GetEyeCamera(i).targetTexture.Release();
                    leiaDisplay.GetEyeCamera(i).targetTexture = firstToRenderLeiaDisplay.GetEyeCamera(i).targetTexture;
                }
                lastnunViews = leiaDisplay.GetViewCount();
                initialized = true;
            }
        }
    }
}