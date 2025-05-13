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
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
#if UNITY_2021_1_OR_NEWER
using System.IO.Compression;
#endif
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace LeiaUnity.EditorUI
{
    [InitializeOnLoad]
    public class LeiaVersionUpdateWindow
    {
        private static readonly float PatchNotesWidth = 500;
        private static readonly float PatchNotesMinHeight = 250.0f;

        private static Vector2 scrollPositionPatchNotes;
        private static GUIStyle _centeredStyle = GUIStyle.none;
        private static GUIStyle _versionStyle = GUIStyle.none;
        private static GUIStyle _patchNotesStyle = GUIStyle.none;
        private static bool _isInitialized;
        private static bool _isExpanded;

        public static GUIStyle CenteredStyle
        {
            get
            {
                if (_centeredStyle == GUIStyle.none)
                {
                    _centeredStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16 };
                }
                return _centeredStyle;
            }
        }
        public static GUIStyle VersionStyle
        {
            get
            {
                if (_versionStyle == GUIStyle.none)
                {
                    _versionStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        fontSize = 14,
                        margin = new RectOffset(20, 20, 0, 0)
                    };
                }
                return _versionStyle;
            }
        }
        public static GUIStyle PatchNotesStyle
        {
            get
            {
                if (_patchNotesStyle == GUIStyle.none)
                {
                    _patchNotesStyle = new GUIStyle(EditorStyles.textArea)
                    {
                        richText = true,
                        margin = new RectOffset(10, 10, 10, 10),
                        padding = new RectOffset(10, 10, 10, 10)
                    };
                }
                return _patchNotesStyle;
            }
        }

        //TODO: Restore update checker functionality
        static LeiaVersionUpdateWindow()
        {
            /// <remove_from_public>
#pragma warning disable CS0162 //Unreachable code detected
            /// </remove_from_public>
            UpdateChecker.CheckForUpdates();
            EditorApplication.update += Update;
            /// <remove_from_public>
#pragma warning restore CS0162
            /// </remove_from_public>
        }

        static void Update()
        {
            if (!_isInitialized && UpdateChecker.UpdateChecked && !string.IsNullOrEmpty(UpdateChecker.CurrentSDKVersion))
            {
                _isInitialized = true;
                _isExpanded = !UpdateChecker.CheckUpToDate();
                EditorApplication.update -= Update;
            }
        }

        private void Title()
        {
            EditorWindowUtils.Space(20);
            string updateText;
            if (!UpdateChecker.UpdateChecked)
            {
                updateText = "Checking for updates...";
            }
            else
            {
                if (!UpdateChecker.CheckUpToDate())
                {
                    updateText = "A new version of the Leia Unity SDK is available!";
                }
                else
                {
                    updateText = "Your Leia Unity SDK is up to date!";
                }
            }
            EditorWindowUtils.Label(updateText, CenteredStyle);
            EditorWindowUtils.Space(20);
            EditorWindowUtils.Label("Currently installed version: " + UpdateChecker.CurrentSDKVersion, VersionStyle);
            EditorWindowUtils.Space(5);
            EditorWindowUtils.Label("Latest version: " + UpdateChecker.LatestSDKVersion, VersionStyle);
            EditorWindowUtils.Space(10);
            EditorWindowUtils.HorizontalLine();
            EditorWindowUtils.Space(10);
        }

        private static void Changes()
        {
            EditorWindowUtils.Label("Changes for " + UpdateChecker.LatestSDKVersion + ":", VersionStyle);
            EditorWindowUtils.Space(5);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorWindowUtils.Space(5);
                using (new EditorGUILayout.VerticalScope())
                {
                    scrollPositionPatchNotes = EditorWindowUtils.DrawScrollableSelectableLabel(
                        scrollPositionPatchNotes,
                        PatchNotesWidth,
                        // this is the string that is displayed in welcome panel. it is set in UpdateChecker.cs
                        UpdateChecker.Patchnotes,
                        PatchNotesStyle,
                        20.0f,
                        20.0f,
                        PatchNotesMinHeight
                    );
                }
            }
        }

        private static void UpdateInstructions()
        {
            EditorWindowUtils.Label("Instructions to update:"
            , VersionStyle);
        }

        private static void UpdateFoldout()
        {
            EditorWindowUtils.BeginHorizontal();
            _isExpanded = EditorGUILayout.Foldout(_isExpanded, string.Format("Updates [ {0}! ]", UpdateChecker.CheckUpToDate() ? "Up To Date" : "Update Available"), true);
            EditorWindowUtils.EndHorizontal();
        }

        private void ShowDeleteButton()
        {
            EditorWindowUtils.Space(20);
            EditorWindowUtils.Button(() =>
            {
                Directory.Delete(Application.dataPath + "/Leia", true);

            }, "Delete Current SDK");
        }

        void DeleteCurrentSDK()
        {
            Directory.Delete(Application.dataPath + "/Leia", true);
        }

        private void Download()
        {
            EditorWindowUtils.Space(20);
            EditorWindowUtils.Button(() =>
            {
                DownloadFileAsync(UpdateChecker.SDKDownloadLink, projectFolderPath + "/LeiaSDKUpdate.zip");
            }, "Download Update");
        }

        static bool DownloadingNewSDK;
        static bool InstallingNewSDK;
        void DownloadNewSDK()
        {
            DownloadFileAsync(UpdateChecker.SDKDownloadLink, projectFolderPath + "/LeiaSDKUpdate.zip");
            DownloadingNewSDK = true;
        }

        private void Unzip()
        {
            EditorWindowUtils.Space(20);
            EditorWindowUtils.Button(() =>
            {
                UnzipFolderAtPath(projectFolderPath + "/LeiaSDKUpdate.zip", projectFolderPath + "/LeiaSDKUpdate");
            }, "Unzip");
        }

        void UnzipNewSDK()
        {
            UnzipFolderAtPath(projectFolderPath + "/LeiaSDKUpdate.zip", projectFolderPath + "/LeiaSDKUpdate");
        }

        private void InstallPackage()
        {
            EditorWindowUtils.Space(20);
            EditorWindowUtils.Button(() =>
            {
                string packagePath = projectFolderPath + "/LeiaSDKUpdate/LeiaUnitySDK_v" + UpdateChecker.LatestSDKVersion + "/LeiaUnitySDK_v" + UpdateChecker.LatestSDKVersion + ".unitypackage";
                Debug.Log("packagePath = " + packagePath);
                AssetDatabase.ImportPackage(packagePath, true);
            }, "InstallPackage");
        }

        void InstallUnityPackage()
        {
            string packagePath = projectFolderPath + "/LeiaSDKUpdate/LeiaUnitySDK_v" + UpdateChecker.LatestSDKVersion + "/LeiaUnitySDK_v" + UpdateChecker.LatestSDKVersion + ".unitypackage";
            Debug.Log("packagePath = " + packagePath);
            AssetDatabase.ImportPackage(packagePath, true);
            InstallingNewSDK = true;
        }

        // Get the directory of the parent folder (project folder)
        static string projectFolderPath
        {
            get
            {
                return Path.GetDirectoryName(Application.dataPath);
            }
        }


        static async void DownloadFileAsync(string fileUrl, string savePath)
        {
            if (!IsValidUrl(fileUrl))
            {
                Console.WriteLine("Invalid URL or domain not allowed.");
            }

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false, // Disable automatic redirection
                                           // Additional restrictions here
            };

            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(fileUrl))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            // Ensure savePath is secure to prevent directory traversal
                            string safeSavePath = SecureSavePath(savePath);
                            if (string.IsNullOrEmpty(safeSavePath))
                            {
                                Console.WriteLine("Invalid save path.");
                            }

                            using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                   stream = new FileStream(safeSavePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await contentStream.CopyToAsync(stream);
                                Debug.Log("Finished downloading " + safeSavePath);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during download: {ex.Message}");
                }
            }
        }

        static bool IsValidUrl(string url)
        {
            var allowedHosts = new HashSet<string> { "example.com", "www.example2.com" };
            if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            {
                return allowedHosts.Contains(uriResult.Host) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            return false;
        }

        static string SecureSavePath(string originalPath)
        {
            string rootDirectory = Path.GetFullPath("YourSafeDirectory"); 
            string fullPath = Path.GetFullPath(Path.Combine(rootDirectory, originalPath));

            if (!fullPath.StartsWith(rootDirectory))
            {
                return null;
            }
            return fullPath;
        }

        private static void UnzipFolderAtPath(string sourceZipPath, string outputFolderPath)
        {
            if (File.Exists(sourceZipPath))
            {
                try
                {
#if UNITY_2021_1_OR_NEWER
                    ZipFile.ExtractToDirectory(sourceZipPath, outputFolderPath);
#else
                    Debug.LogError("Could not extract zip file. System.IO.Compression is not supported in Unity 2020 and earlier. Try using Unity 2021 or later or manually extracting the zip file "+ sourceZipPath + " to the path "+ outputFolderPath + ", then install the included LeiaUnitySDK_vX.Y.Z.unitypackage.");
#endif
                }
                catch (IOException e)
                {
                    Debug.LogError("IO error extracting folder: " + e.Message);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error extracting folder: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Source zip folder not found.");
            }
        }

        public void DrawGUI()
        {
            string currentVersion = RemoveNonNumericCharacters(UpdateChecker.CurrentSDKVersion);
            string latestVersion = RemoveNonNumericCharacters(UpdateChecker.LatestSDKVersion);

            bool isUpToDate = IsVersionGreaterThanOrEqual(currentVersion, latestVersion);

            if (!_isInitialized)
            {
                return;
            }
            UpdateFoldout();

            if (_isExpanded)
            {
                EditorWindowUtils.HorizontalLine();
                Title();
                Changes();
                if (!isUpToDate)
                {
                    UpdateInstructions();
                }
            }
            EditorWindowUtils.HorizontalLine();

            // Create a GUIStyle
            GUIStyle incompleteStyle = new GUIStyle(GUI.skin.label);
            GUIStyle inProgressStyle = new GUIStyle(GUI.skin.label);
            GUIStyle completeStyle = new GUIStyle(GUI.skin.label);

            // Set the text color of the GUIStyle
            incompleteStyle.normal.textColor = Color.red;
            inProgressStyle.normal.textColor = Color.yellow;
            completeStyle.normal.textColor = Color.green;

            List<Task> tasks = new List<Task>();

            if (!isUpToDate)
            {
                bool downloadedNewSDK = File.Exists(projectFolderPath + "/LeiaSDKUpdate.zip");

                tasks.Add(
                    new Task(
                    "Delete old Leia folder",
                    (isUpToDate || (!isUpToDate && !Directory.Exists(Application.dataPath + "/Leia")) || downloadedNewSDK)
                        ? Task.State.DONE : Task.State.TODO,
                    "DeleteCurrentSDK"
                    )
                );

                Task.State downloadState;
                downloadState = (downloadedNewSDK) ? Task.State.DONE : Task.State.TODO;
                if (downloadState == Task.State.TODO && DownloadingNewSDK)
                {
                    downloadState = Task.State.IN_PROGRESS;
                }

                tasks.Add(
                    new Task(
                    "Download new SDK",
                    downloadState,
                    "DownloadNewSDK"
                    )
                );
                tasks.Add(
                    new Task(
                    "Unzip new SDK",
                    (Directory.Exists(projectFolderPath + "/LeiaSDKUpdate")) ? Task.State.DONE : Task.State.TODO,
                    "UnzipNewSDK"
                    )
                );

                Task.State installState;
                installState = (currentVersion == latestVersion && Directory.Exists(Application.dataPath + "/Leia"))
                        ? Task.State.DONE : Task.State.TODO;
                if (installState == Task.State.TODO && InstallingNewSDK)
                {
                    installState = Task.State.IN_PROGRESS;
                }
                tasks.Add(
                    new Task(
                    "Install unitypackage",
                    installState,
                    "InstallUnityPackage"
                    )
                );

                int step = 0;
                bool previousStepDone = false;
                foreach (Task task in tasks)
                {
                    step++;
                    GUIStyle style = incompleteStyle;

                    if (task.state == Task.State.IN_PROGRESS)
                    {
                        style = inProgressStyle;
                    }
                    else
                    if (task.state == Task.State.DONE)
                    {
                        style = completeStyle;
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(step + ": " + task.description, GUILayout.Width(350f));
                    GUILayout.Label(task.state.ToString().Replace("_", " "), style);

                    GUILayout.EndHorizontal();
                    GUILayout.Space(10f);

                    if (previousStepDone || step == 1)
                    {
                        if (task.state == Task.State.TODO
                            && GUILayout.Button(
                                "DO IT",
                                GUILayout.Width(200),
                                GUILayout.Height(30))
                            )
                        {
                            System.Reflection.MethodInfo methodInfo = GetType().GetMethod(task.taskAction,
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                            if (methodInfo == null)
                            {
                                Debug.LogError("methodInfo is null");
                            }
                            methodInfo.Invoke(this, null);
                        }
                    }
                    GUILayout.Space(20f);

                    previousStepDone = (task.state == Task.State.DONE);
                }
            }
        }

        string RemoveNonNumericCharacters(string inputString)
        {
            // Use a StringBuilder to efficiently build the cleaned string
            System.Text.StringBuilder cleanedStringBuilder = new System.Text.StringBuilder();

            // Loop through each character in the input string
            foreach (char c in inputString)
            {
                // Check if the character is a digit or a dot
                if (char.IsDigit(c) || c == '.')
                {
                    // Append the character to the cleaned string
                    cleanedStringBuilder.Append(c);
                }
            }

            // Convert the StringBuilder to a string and return it
            return cleanedStringBuilder.ToString();
        }

        public static bool IsVersionGreaterThanOrEqual(string currentVersion, string latestVersion)
        {
            var currentVersionParts = Array.ConvertAll(currentVersion.Split('.'), int.Parse);
            var latestVersionParts = Array.ConvertAll(latestVersion.Split('.'), int.Parse);

            for (int i = 0; i < Math.Max(currentVersionParts.Length, latestVersionParts.Length); i++)
            {
                int currentPart = i < currentVersionParts.Length ? currentVersionParts[i] : 0;
                int latestPart = i < latestVersionParts.Length ? latestVersionParts[i] : 0;

                if (currentPart > latestPart)
                    return true;
                if (currentPart < latestPart)
                    return false;
            }

            return true;
        }

    }


    class Task
    {
        public string description;
        public enum State { TODO, IN_PROGRESS, DONE };
        public State state;
        public string taskAction;
        public bool InProgress;

        public Task(string description, State state, string taskAction)
        {
            this.description = description;
            this.state = state;
            this.taskAction = taskAction;
        }
    }
}
