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
using LeiaUnity;

namespace LeiaUnity.Examples
{
    public class SetPlaneDistanceForCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private LeiaDisplay targetDisplay;
        void Update()
        {
            if (targetDisplay.mode == LeiaDisplay.ControlMode.CameraDriven)
            {
                canvas.planeDistance = targetDisplay.FocalDistance;
            }
            else
            {
                Vector3 displayPosition = targetDisplay.transform.position;
                Vector3 cameraPosition = targetDisplay.HeadCamera.transform.position;
                canvas.planeDistance = Vector3.Distance(displayPosition, cameraPosition);
            }
        }
    }
}