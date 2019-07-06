//-----------------------------------------------------------------------
// <copyright file="CloudAnchorsExampleController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------


using GoogleARCore;
using UnityEngine;
using Photon.Pun;
using GoogleARCore.Examples.ObjectManipulation;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controller for the Cloud Anchors Example. Handles the ARCore lifecycle.
/// </summary>
public class CloudAnchorsExampleController : Manipulator
{
    public GameObject FirstPersonCamera;
    private bool isAnchorHosted;
    private bool m_IsOriginPlaced = false;
    private bool m_IsQuitting = false;
    private Component m_WorldOriginAnchor = null;
    private Pose? m_LastHitPose = null;
    private ApplicationMode m_CurrentMode = ApplicationMode.Ready;
    public enum ApplicationMode
    {
        Ready,
        Hosting,
        Resolving,
    }

    public GameObject selectionCanvas;

    [Header("ARCore")]
    public GameObject ARCoreRoot;
    public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;

    /// <summary>
    /// Unity Start() method.
    /// </summary>
    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _ShowAndroidToastMessage("Master");
            m_CurrentMode = ApplicationMode.Hosting;
        }
        else
        {
            _ShowAndroidToastMessage("Student");
            m_CurrentMode = ApplicationMode.Resolving;
        }
        gameObject.name = "CloudAnchorsExampleController";
        DontDestroyOnLoad(gameObject);
        _ResetStatus();
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    /// 
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        //_ShowAndroidToastMessage("Can start");
        if (gesture.TargetObject == null)
        {
            return true;
        }
        //_ShowAndroidToastMessage("Selected " + gesture.TargetObject.name);
        gesture.TargetObject.GetComponent<PhotonView>().RequestOwnership();
        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        //_ShowAndroidToastMessage("END");
        _UpdateApplicationLifecycle();
        if (gesture.WasCancelled)
        {
            return;
        }

        // If gesture is targeting an existing object we are done.
        if (gesture.TargetObject != null)
        {
            _ShowAndroidToastMessage("target Not Null onEnd");
            return;
        }

        // If selection panel is on we are done
        if(selectionCanvas.GetComponent<selectScreenToggle>().toggs)
        {
            return;
        }
        // If we are neither in hosting nor resolving mode then the update is complete.
        if (m_CurrentMode == ApplicationMode.Ready)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                m_CurrentMode = ApplicationMode.Hosting;
                _ShowAndroidToastMessage("Hello Master");
              //  hostSet = true;
            }
            else
            {
              
                m_CurrentMode = ApplicationMode.Resolving;
                _ShowAndroidToastMessage("Hello Slave");
            }
          //  return;

        }

        // If the origin anchor has not been placed yet, then update in resolving mode is
        // complete.
        if (m_CurrentMode == ApplicationMode.Resolving && !m_IsOriginPlaced)
        {
            return;
        }

        //We don't need it because we already have touch data under gesture class
        //// If the player has not touched the screen then the update is complete.
        //Touch touch;
        //if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        //{
        //    return;
        //}

        TrackableHit arcoreHitResult = new TrackableHit();
        m_LastHitPose = null;

        // Raycast against the location the player touched to search for planes.
        if (ARCoreWorldOriginHelper.Raycast(gesture.StartPosition.x,gesture.StartPosition.y,
                TrackableHitFlags.PlaneWithinPolygon, out arcoreHitResult))
        {
            m_LastHitPose = arcoreHitResult.Pose;
        }
   //    if (hostSet)



        // If there was an anchor placed, then instantiate the corresponding object.
        if (m_LastHitPose != null)
        {
            // The first touch on the Hosting mode will instantiate the origin anchor. Any
            // subsequent touch will instantiate a star, both in Hosting and Resolving modes.
            if ((arcoreHitResult.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - arcoreHitResult.Pose.position,
                        arcoreHitResult.Pose.rotation * Vector3.up) < 0)
            {
                _ShowAndroidToastMessage("Hit at back of the current DetectedPlane");
            }

            else
            {
                if (_CanPlaceStars())
                {
                    string s;
                    int sel = selectionCanvas.GetComponent<selectScreenToggle>().selec;
                    if (sel==1)
                    {
                        s = "armChairM1";
                    }
                    else if(sel==2)
                    {
                        s = "armChairM2";
                    }
                    else if (sel == 3)
                    {
                        s = "coffeeTableM";
                    }
                    else if(sel == 4)
                    {
                        s = "couchM";
                    }
                    else
                    {
                        s = "AllForOne";
                        _ShowAndroidToastMessage("Selection Error");
                    }
                    if (selectionCanvas.GetComponent<selectScreenToggle>().justSelec)
                    {
                        var allForOneObj = PhotonNetwork.Instantiate(s, m_LastHitPose.Value.position, m_LastHitPose.Value.rotation);
                        //  _ShowAndroidToastMessage("instantiation succesful of"+s);
                        var localAnchor = arcoreHitResult.Trackable.CreateAnchor(arcoreHitResult.Pose);
                        //ctr++;
                        //allForOneObj.name = (string)("spawnable "+(char)ctr);
                        allForOneObj.transform.parent = localAnchor.transform;
                        //manipulator.transform.parent = localAnchor.transform;
                        allForOneObj.GetComponent<Manipulator>().Select();
                        //manipulator.GetComponent<Manipulator>().Select();
                        //_ShowAndroidToastMessage(s);
                        selectionCanvas.GetComponent<selectScreenToggle>().justSelec = false;
                    }

                }
                else if (!m_IsOriginPlaced && m_CurrentMode == ApplicationMode.Hosting)
                {
                    m_WorldOriginAnchor = arcoreHitResult.Trackable.CreateAnchor(arcoreHitResult.Pose);
                    SetWorldOrigin(m_WorldOriginAnchor.transform);
                    _InstantiateAnchor();
                }
            }
        }
    }

    /// <summary>
    /// Sets the apparent world origin so that the Origin of Unity's World Coordinate System
    /// coincides with the Anchor. This function needs to be called once the Cloud Anchor is
    /// either hosted or resolved.
    /// </summary>
    /// <param name="anchorTransform">Transform of the Cloud Anchor.</param>
    public void SetWorldOrigin(Transform anchorTransform)
    {
        if (m_IsOriginPlaced)
        {
            Debug.LogWarning("The World Origin can be set only once.");
            return;
        }

        m_IsOriginPlaced = true;

      //  _ShowAndroidToastMessage("World origin for this client set");
        ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);

    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was hosted.
    /// </summary>
    /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
    /// successfully.</param>
    /// <param name="response">The response string received.</param>
    public void OnAnchorHosted(bool success, string response)
    {
        if (success)
        {
            _ShowAndroidToastMessage("Hosted");
            isAnchorHosted = true;
        }
        else
        {
            isAnchorHosted = false;
        }
    }

    /// <summary>
    /// Instantiates the anchor object at the pose of the m_LastPlacedAnchor Anchor. This will
    /// host the Cloud Anchor.
    /// </summary>
    private void _InstantiateAnchor()
    {
        _ShowAndroidToastMessage("Network Anchor Instantiated");
        var anchorObject = PhotonNetwork.Instantiate("Anchor", Vector3.zero, Quaternion.identity);
        anchorObject.GetComponent<AnchorController>().HostLastPlacedAnchor(m_WorldOriginAnchor);
    }

    ///// <summary>
    ///// Instantiates a star object that will be synchronized over the network to other clients.
    ///// </summary>
    //private void _InstantiateStar()
    //{
    //    //_ShowAndroidToastMessage("Watashi Ga Kita");
    //    //var allForOneObj = PhotonNetwork.Instantiate("AllForOne", m_LastHitPose.Value.position, m_LastHitPose.Value.rotation);
    //    //var localAnchor = arcoreHitResult.Trackable.CreateAnchor(arcoreHitResult.Pose);
    //    //allForOneObj.transform.parent = localAnchor.transform;
    //    //allForOneObj.GetComponent<Manipulator>().Select();
    //}

    /// <summary>
    /// Indicates whether a star can be placed.
    /// </summary>
    /// <returns><c>true</c>, if stars can be placed, <c>false</c> otherwise.</returns>
    private bool _CanPlaceStars()
    {
        if (m_CurrentMode == ApplicationMode.Resolving)
        {
         //   _ShowAndroidToastMessage("star mode on slave");
            return m_IsOriginPlaced;
        }

        if (m_CurrentMode == ApplicationMode.Hosting)
        {
       //    _ShowAndroidToastMessage("star mode on host");
            return m_IsOriginPlaced && isAnchorHosted;
        }

        return false;
    }

    /// <summary>
    /// Resets the internal status.
    /// </summary>
    private void _ResetStatus()
    {
        // Reset internal status.
        m_CurrentMode = ApplicationMode.Ready;
        if (m_WorldOriginAnchor != null)
        {
            Destroy(m_WorldOriginAnchor.gameObject);
        }

        m_WorldOriginAnchor = null;
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        var sleepTimeout = SleepTimeout.NeverSleep;

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            sleepTimeout = lostTrackingSleepTimeout;
        }

        Screen.sleepTimeout = sleepTimeout;

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            m_IsQuitting = true;
            Invoke("_DoQuit", 5.0f);
        }
        else if (Session.Status.IsError())
        {
            m_IsQuitting = true;
            Invoke("_DoQuit", 5.0f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Android Toast Messages for debugging purposes
    /// </summary>
    /// <param name="message"></param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
