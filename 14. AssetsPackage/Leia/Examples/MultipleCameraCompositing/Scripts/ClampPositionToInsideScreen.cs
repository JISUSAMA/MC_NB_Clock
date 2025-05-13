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

namespace LeiaUnity.Examples.Asteroids
{
    [DefaultExecutionOrder(100)]
    public class ClampPositionToInsideScreen : MonoBehaviour
    {
        [SerializeField] private Transform screenLeft;
        [SerializeField] private Transform screenRight;

        void LateUpdate()
        {
            ClampPositionToScreen();
        }

        void ClampPositionToScreen()
        {
            transform.position =
                new Vector3(
                Mathf.Clamp(transform.position.x, screenLeft.position.x, screenRight.position.x),
                transform.position.y,
                transform.position.z
                );
        }
    }
}