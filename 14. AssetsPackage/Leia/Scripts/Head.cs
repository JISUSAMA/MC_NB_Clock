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
using UnityEditor;
using UnityEngine;
using static LeiaUnity.LeiaDisplay;
using UnityEngine.Rendering;

namespace LeiaUnity
{
    [ExecuteInEditMode]
    public class Head : MonoBehaviour
    {
        public LeiaDisplay leiaDisplay;
        public List<Eye> eyes;
        public Vector3 HeadPositionMM;
        public Camera headcamera;
        public LayerMask CullingMask;

        public List<Vector2> ViewConfig;

        private Vector3 InitPosition;

        private readonly Dictionary<Camera, List<System.Action>> beginViewRenderingEvents = new Dictionary<Camera, List<System.Action>>();
        private readonly Dictionary<Camera, List<System.Action>> endViewRenderingEvents = new Dictionary<Camera, List<System.Action>>();

        public void InitHead(List<Vector2> ViewConfig, LeiaDisplay leiaDisplay)
        {
            CullingMask = -1; //Show all layers by default
            headcamera = gameObject.AddComponent<Camera>();
            headcamera.depth = 1f;
#if LEIA_HDRP_DETECTED
            if (headcamera != null)
            {
                var additionalCameraData = gameObject.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData>();
                if (additionalCameraData == null)
                {
                    additionalCameraData = gameObject.AddComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData>();
                }
            }
#endif
            HeadPositionMM = new Vector3(0, 0, leiaDisplay.ViewingDistanceMM);

            this.leiaDisplay = leiaDisplay;
            this.ViewConfig = ViewConfig;

            eyes = new List<Eye>();

            int counter = 0;
            foreach (var offset in ViewConfig)
            {
                GameObject pivotGO;
                if (counter == 0)
                {
                    pivotGO = new GameObject("LeftEye");
                } 
                else if (counter == 1)
                {
                    pivotGO = new GameObject("RightEye");
                }
                else
                {
                    pivotGO = new GameObject("Eye");
                }
                pivotGO.transform.parent = transform;
                pivotGO.transform.localPosition = Vector3.zero;
                pivotGO.transform.localRotation = Quaternion.identity;
                Eye newEye = pivotGO.AddComponent<Eye>();
                newEye.offset = offset;
                newEye.leiaDisplay = leiaDisplay;
                eyes.Add(newEye);
                newEye.EyeUpdate();

                counter++;
            }
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                CullingMask = headcamera.cullingMask;
                headcamera.cullingMask = 0;
            }
            // this has to be Start, not OnEnable, because in OnEnable the Eyes.eyecamera is not set set
#if UNITY_EDITOR || PLATFORM_ANDROID
            if (RenderPipelineUtils.IsUnityRenderPipeline())
            {
                Debug.Log("Render Pipeline detected!");
                RenderPipelineManager.endFrameRendering += EndFrameRenderHook;
                // this code calls the camera pre-render events events every frame
                RenderPipelineManager.beginCameraRendering += BeginCameraRenderingHook;
                // this code calls the camera post-render events every frame
                RenderPipelineManager.endCameraRendering += EndCameraRenderingHook;

                // this code sets some pre-Render events for each eyecamera each frame
                for (int i = 0; i < eyes.Count; ++i)
                {
                    if (!beginViewRenderingEvents.ContainsKey(eyes[i].Eyecamera))
                    {
                        beginViewRenderingEvents[eyes[i].Eyecamera] = new List<System.Action>();
                    }

                    // have to not capture the value within the callback
                    float viewID = i * 1.0f;
                    // create the action event here here. will be called later
                    System.Action setViewID = () =>
                    {
                        Shader.SetGlobalFloat("_LeiaViewID", viewID);
                    };

                    beginViewRenderingEvents[eyes[i].Eyecamera].Add(setViewID);
                }

                // this code sets some post-Render events for each LeiaView each frame
                for (int i = 0; i < eyes.Count; ++i)
                {
                    if (!endViewRenderingEvents.ContainsKey(eyes[i].Eyecamera))
                    {
                        endViewRenderingEvents[eyes[i].Eyecamera] = new List<System.Action>();
                    }

                    System.Action unsetViewID = () =>
                    {
                        Shader.SetGlobalFloat("_LeiaViewID", -1);
                    };

                    endViewRenderingEvents[eyes[i].Eyecamera].Add(unsetViewID);
                }
            }
#endif
        }
        void OnDisable()
        {
            if (RenderPipelineUtils.IsUnityRenderPipeline())
            {
                RenderPipelineManager.endFrameRendering -= EndFrameRenderHook;
                RenderPipelineManager.beginCameraRendering -= BeginCameraRenderingHook;
            }
        }
        void BeginCameraRenderingHook(ScriptableRenderContext context, Camera renderingCam)
        {
            LogUtil.Log(LogLevel.Info, string.Format("Head::BeginCameraRenderingHook, context: {0}", context));
            // call events for each view. Generally these will be shader SetFloat events
            if (beginViewRenderingEvents.ContainsKey(renderingCam))
            {
                foreach (System.Action action in beginViewRenderingEvents[renderingCam])
                {
                    action.Invoke();
                }
            }
        }

        void EndCameraRenderingHook(ScriptableRenderContext context, Camera renderingCam)
        {
            LogUtil.Log(LogLevel.Info, string.Format("Head::EndCameraRenderingHook, context: {0}", context));
            if (endViewRenderingEvents.ContainsKey(renderingCam))
            {
                foreach (System.Action action in endViewRenderingEvents[renderingCam])
                {
                    action.Invoke();
                }
            }
        }

        void EndFrameRenderHook(ScriptableRenderContext context, Camera[] cams)
        {
            LogUtil.Log(LogLevel.Info, string.Format("Head::EndFrameRenderHook, context: {0}", context));
            if (cams != null && headcamera != null && cams[0] == headcamera)
            {
                // only need to run EndFramRenderHook for Head's headcamera
                Camera.SetupCurrent(headcamera);
                OnPostRender();
            }
        }
        
        public void HeadUpdate()
        {
            if (leiaDisplay == null || leiaDisplay.ViewersHead != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

#if UNITY_EDITOR
            transform.localPosition = leiaDisplay.RealToVirtualCenterFacePosition(HeadPositionMM);
            Matrix4x4 p = leiaDisplay.GetProjectionMatrixForCamera(headcamera, Vector3.zero, false);
            headcamera.projectionMatrix = p;
#else
            transform.localPosition = Vector3.zero;
#endif
#if !UNITY_EDITOR
            if (RenderTrackingDevice.Instance != null)
            {
                var realToVirtual = leiaDisplay.VirtualHeight / leiaDisplay.HeightMM;
#if PLATFORM_ANDROID
                if (LeiaUnity.RenderTrackingDevice.Instance.currentVersion.IsGreaterOrEqual(0, 10, 9))
                {
#endif
                    var lookAroundEyesPosition = RenderTrackingDevice.Instance.GetLookAroundEyesPosition();

                    if (lookAroundEyesPosition[0] != Vector3.zero && lookAroundEyesPosition[1] != Vector3.zero)
                    {   
                        transform.localPosition = Vector3.zero;

                        var eyeCenter = 0.5f * (lookAroundEyesPosition[0] + lookAroundEyesPosition[1]);

                        var adjustedEyeCenter = new Vector3(
                            eyeCenter.x * leiaDisplay.LookAroundFactor,
                            eyeCenter.y * leiaDisplay.LookAroundFactor,
                            leiaDisplay.FOVFactor > 1 ?
                                -((leiaDisplay.ViewingDistanceMM + leiaDisplay.LookAroundFactor * (eyeCenter.z - leiaDisplay.ViewingDistanceMM)) / leiaDisplay.FOVFactor) :
                                -(leiaDisplay.ViewingDistanceMM / leiaDisplay.FOVFactor + leiaDisplay.LookAroundFactor * (eyeCenter.z - leiaDisplay.ViewingDistanceMM))
                        );

                        var eyeDelta = lookAroundEyesPosition[1] - lookAroundEyesPosition[0];
                        
                        eyes[0].transform.localPosition = (adjustedEyeCenter - 0.5f * (leiaDisplay.DepthFactor / leiaDisplay.FOVFactor) * eyeDelta) * realToVirtual;
                        eyes[1].transform.localPosition = (adjustedEyeCenter + 0.5f * (leiaDisplay.DepthFactor / leiaDisplay.FOVFactor) * eyeDelta) * realToVirtual;
                    }
                    else
                    {
                        transform.localPosition = leiaDisplay.RealToVirtualCenterFacePosition(HeadPositionMM);
                        eyes[0].transform.localPosition = Vector3.zero;
                        eyes[1].transform.localPosition = Vector3.zero;
                    }
#if PLATFORM_ANDROID
                }
                else
                {
                    var lookAroundEyesPosition = RenderTrackingDevice.Instance.GetNonPredictedFacePosition();

                    if (lookAroundEyesPosition != Vector3.zero)
                    {   
                        transform.localPosition = Vector3.zero;

                        var eyeCenter = lookAroundEyesPosition;

                        var adjustedEyeCenter = new Vector3(
                            eyeCenter.x * leiaDisplay.LookAroundFactor,
                            eyeCenter.y * leiaDisplay.LookAroundFactor,
                            leiaDisplay.FOVFactor > 1 ?
                                -((leiaDisplay.ViewingDistanceMM + leiaDisplay.LookAroundFactor * (eyeCenter.z - leiaDisplay.ViewingDistanceMM)) / leiaDisplay.FOVFactor) :
                                -(leiaDisplay.ViewingDistanceMM / leiaDisplay.FOVFactor + leiaDisplay.LookAroundFactor * (eyeCenter.z - leiaDisplay.ViewingDistanceMM))
                        );

                        Vector3  eyeDelta = new Vector3(1.0f, 0, 0) * leiaDisplay.IPDMM;
                        eyes[0].transform.localPosition = (adjustedEyeCenter - 0.5f * (leiaDisplay.DepthFactor / leiaDisplay.FOVFactor) * eyeDelta) * realToVirtual;
                        eyes[1].transform.localPosition = (adjustedEyeCenter + 0.5f * (leiaDisplay.DepthFactor / leiaDisplay.FOVFactor) * eyeDelta) * realToVirtual;
                    }
                    else
                    {
                        transform.localPosition = leiaDisplay.RealToVirtualCenterFacePosition(HeadPositionMM);
                        eyes[0].transform.localPosition = Vector3.zero;
                        eyes[1].transform.localPosition = Vector3.zero;
                    }
                }
#endif
            }
#endif
            foreach (Eye eye in eyes)
            {
                eye.EyeUpdate();
            }

            if (leiaDisplay.DriverCamera != null)
            {
                leiaDisplay.DriverCamera.enabled = false;
                LeiaUtils.CopyCameraParameters(leiaDisplay.DriverCamera, headcamera);
                CullingMask = leiaDisplay.DriverCamera.cullingMask;
            }

            if (Application.isPlaying && RenderTrackingDevice.Instance.DesiredLightfieldMode == RenderTrackingDevice.LightfieldMode.On)
            {
                //Set head camera culling mask to nothing so that only the Eye cameras render in 3d mode
                headcamera.cullingMask = 0;
            }
            else
            {
                //Render the head camera in 2D mode and in edit mode
                headcamera.cullingMask = CullingMask;
            }
        }

        void OnPostRender()
        {
            if (Application.isPlaying)
            {
                leiaDisplay.RenderImage();
            }
        }
    }
}
