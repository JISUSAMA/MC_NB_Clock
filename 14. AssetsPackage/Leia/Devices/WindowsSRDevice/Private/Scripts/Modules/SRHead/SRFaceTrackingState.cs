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
namespace LeiaUnity
{
    public class SRFaceTrackingState : Singleton<SRFaceTrackingState>
    {
        private FaceTrackingStateEngine _faceTrackingStateEngine;
        public float AnimatedBaseline => _faceTrackingStateEngine.eyeTrackingAnimatedBaselineScalar;

        private bool _previousFaceDetected;
        private bool _triggered2D;

        private void Awake()
        {
            _faceTrackingStateEngine = gameObject.AddComponent<FaceTrackingStateEngine>();
        }

        private void Update()
        {   
            if(SRUnity.SRCore.IsSimulatedRealityAvailable())
            {
                Handle2DTrigger();
                UpdateFaceTrackingState();
            }
        }

        private void Handle2DTrigger()
        {
            if (!SRUnity.SrRenderModeHint.ShouldRender3D() && !_triggered2D)
            {
                _triggered2D = true;
            }
        }

        private void UpdateFaceTrackingState()
        {
            if (_triggered2D)
            {
                _faceTrackingStateEngine.faceTransitionState = FaceTrackingStateEngine.FaceTransitionState.SlidingCameras;
                _triggered2D = false;
            }

            var eyePosition = SRUnity.SRHead.Instance.GetEyePosition(ISRSettingsInterface.GetProjectSettings(null));
            var defaultPosition = SRUnity.SRHead.Instance.GetDefaultHeadPosition(ISRSettingsInterface.GetProjectSettings(null));

            if (eyePosition != defaultPosition)
            {
                HandleEyePositionDetected();
            }
            else
            {
                HandleNoEyePositionDetected();
            }
        }

        private void HandleEyePositionDetected()
        {
            if ((_faceTrackingStateEngine.faceTransitionState == FaceTrackingStateEngine.FaceTransitionState.SlidingCameras ||
                _faceTrackingStateEngine.faceTransitionState == FaceTrackingStateEngine.FaceTransitionState.NoFace) &&
                !_previousFaceDetected)
            {
                _faceTrackingStateEngine.faceTransitionState = FaceTrackingStateEngine.FaceTransitionState.IncreasingBaseline;
                RenderTrackingDevice.Instance.Set3DMode(true);
            }
            _previousFaceDetected = true;
        }

        private void HandleNoEyePositionDetected()
        {
            if (_faceTrackingStateEngine.faceTransitionState == FaceTrackingStateEngine.FaceTransitionState.FaceLocked && _previousFaceDetected)
            {
                _faceTrackingStateEngine.faceTransitionState = FaceTrackingStateEngine.FaceTransitionState.ReducingBaseline;
            }
            else if (_faceTrackingStateEngine.faceTransitionState == FaceTrackingStateEngine.FaceTransitionState.SlidingCameras)
            {
                RenderTrackingDevice.Instance.Set3DMode(false);
            }
            _previousFaceDetected = false;
        }

        private void LogTrackingStatus()
        {
            Debug.Log($"BaselineScalar: {_faceTrackingStateEngine.eyeTrackingAnimatedBaselineScalar} faceTransitionState: {_faceTrackingStateEngine.faceTransitionState}");
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                ResetTrackingState();
            }
        }

        private void ResetTrackingState()
        {
            _triggered2D = false;
            _faceTrackingStateEngine.faceTransitionState = FaceTrackingStateEngine.FaceTransitionState.NoFace;
            _faceTrackingStateEngine.eyeTrackingAnimatedBaselineScalar = 0;
            _previousFaceDetected = false;
        }
    }
}
