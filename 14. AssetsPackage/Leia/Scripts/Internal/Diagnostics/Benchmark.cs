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
using UnityEngine;
using UnityEngine.UI;
namespace LeiaUnity
{
    public class Benchmark : MonoBehaviour
    {
        bool isTracking = true;

        [SerializeField]
        EyeTracking eyeTracking;

        [SerializeField]
        Text blinkText;

        [SerializeField]
        LeiaUnity.LeiaDisplay leiaDisplay;

        [SerializeField]
        Text backlightText;

        [SerializeField]
        Text interlacingText;

        void Start()
        {
#if UNITY_ANDROID
            Application.targetFrameRate = 60;
#endif
        }


        public void ToggleTracking()
        {
            if (isTracking)
            {
                eyeTracking.StopTracking();
                blinkText.text = "Face Tracking [Off]";
            }
            else
            {
                eyeTracking.StartTracking();
                blinkText.text = "Face Tracking [On]";
            }

            isTracking = !isTracking;
        }
    }
}