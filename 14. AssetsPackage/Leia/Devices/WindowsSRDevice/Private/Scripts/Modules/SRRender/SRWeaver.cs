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

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

#if !UNITY_2019_1_OR_NEWER
using UnityEngine.Experimental.Rendering;
#warning "SRUnity: weaving is not support before Unity 2019.1"
#endif

namespace SRUnity
{
    // Weaver class that handles weaving on all rendering pipeline. Default: hooks into OnRenderImage and calls the weaver logic. SRP: Executes the weaver logic on endFrameRendering.
    public class SRWeaver
    {
        [DllImport("SRUnityNative")]
        private static extern IntPtr GetNativeGraphicsEvent();

        [DllImport("SRUnityNative")]
        private static extern void SetWeaverContextPtr(IntPtr context);

        [DllImport("SRUnityNative")]
        private static extern void SetWeaverResourcePtr(IntPtr src, IntPtr dst);

        [DllImport("SRUnityNative")]
        private static extern void SetWeaverOutputResolution(int Width, int Height);

        [DllImport("SRUnityNative")]
        private static extern bool GetWeaverEnabled();
        [DllImport("SRUnityNative")]
        private static extern void EnableLateLatching(bool enable);
        [DllImport("SRUnityNative")]
        private static extern bool IsLateLatchingEnabled();

        public void Init()
        {
            UpdateWeavingData(null, null);
        }

        public void Destroy()
        {
        }

        public bool CanWeave()
        {
            return GetWeaverEnabled();
        }

        public void WeaveToContext(ScriptableRenderContext context, Texture frameBuffer)
        {
#if UNITY_EDITOR
            if (Camera.current != null && Camera.current.cameraType != CameraType.Game) return;
#endif
            UpdateWeavingData(frameBuffer, null);

            CommandBuffer cb = new CommandBuffer();
            cb.name = "SRWeave";
            cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            cb.IssuePluginEvent(GetNativeGraphicsEvent(), 1);
            context.ExecuteCommandBuffer(cb);
        }

        public void WeaveToTarget(RenderTexture target, RenderTexture frameBuffer, bool clearFramebuffer)
        {
            UpdateWeavingData(frameBuffer, target);

            RenderTexture.active = target;
            GL.IssuePluginEvent(GetNativeGraphicsEvent(), 1);

            // Clear framebuffer
            if (clearFramebuffer)
            {
                RenderTexture.active = frameBuffer;
                GL.Clear(true, true, Color.clear);
            }
        }

        private void UpdateWeavingData(Texture frameBuffer, Texture target)
        {
            SetWeaverContextPtr(SRCore.Instance.GetSrContext());

            IntPtr src = frameBuffer != null ? frameBuffer.GetNativeTexturePtr() : IntPtr.Zero;
            IntPtr dst = target != null ? target.GetNativeTexturePtr() : IntPtr.Zero;

            SetWeaverResourcePtr(src, dst);

            SetWeaverOutputResolution((int)Screen.width, (int)Screen.height);
        }

        public void EnableLateLatchingDX11(bool enable)
        {
            EnableLateLatching(enable);
        }

        public bool IsLateLatchingEnabledDX11()
        {
            return IsLateLatchingEnabled();
        }
    }
}
