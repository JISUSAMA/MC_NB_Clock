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
using UnityEngine.Events;
namespace LeiaUnity.Examples.Asteroids
{
    public class Button : MonoBehaviour
    {
        bool pressedPrev;
        int layerMask;
        Vector3 startPos;
        [SerializeField] private Transform movingButton;
        [SerializeField] private Vector3 pressOffset;
        [SerializeField] private UnityEvent onClick;
        [SerializeField] private UnityEvent onHold;
        [SerializeField] private UnityEvent onRelease;

        void Start()
        {
            layerMask = LayerMask.GetMask("UI");
            startPos = movingButton.localPosition;
        }

        void Update()
        {
            bool pressed = false;

#if UNITY_EDITOR
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                if (hit.transform == transform)
                {
                    if (Input.GetMouseButton(0))
                    {
                        pressed = true;
                    }
                }
                else
                {
                    if (pressedPrev)
                    {
                        onRelease.Invoke();
                    }
                }
            }
#endif
            if (Input.touchCount > 0)
            {
                int count = Input.touchCount;

                for (int i = 0; i < count; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Ray touchRay = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit touchHit;
                    if (Physics.Raycast(touchRay, out touchHit, 100f, layerMask))
                    {
                        if (touchHit.transform == transform)
                        {
                            pressed = true;
                        }
                    }
                }
            }

            if (pressed && pressedPrev)
            {
                onHold.Invoke();
            }

            if (pressed)
            {
                movingButton.localPosition = startPos + pressOffset;
                if (!pressedPrev)
                {
                    onClick.Invoke();
                    pressedPrev = true;
                }
            }
            else
            {
                if (pressedPrev)
                {
                    onRelease.Invoke();
                }
                movingButton.localPosition = startPos;
            }

            pressedPrev = pressed;
        }
    }
}