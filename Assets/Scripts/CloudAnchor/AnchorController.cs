//-----------------------------------------------------------------------
// <copyright file="AnchorController.cs" company="Google">
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
using GoogleARCore.CrossPlatform;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// A Controller for the Anchor object that handles hosting and resolving the Cloud Anchor.
/// </summary>
public class AnchorController : MonoBehaviour, IPunObservable

{
    /// <summary>
    /// The Cloud Anchor ID that will be used to host and resolve the Cloud Anchor. This
    /// variable will be syncrhonized over all clients.
    /// </summary>
    private string m_CloudAnchorId = string.Empty;

    /// <summary>
    /// Indicates whether an attempt to resolve the Cloud Anchor should be made.
    /// </summary>
    private bool m_ShouldResolve = false;
    private bool firstTime = true;

    /// <summary>
    /// The Cloud Anchors example controller.
    /// </summary>
    private CloudAnchorsExampleController m_CloudAnchorsExampleController;

    /// <summary>
    /// The Unity Start() method.
    /// </summary>
    public void Start()
    {
        m_CloudAnchorsExampleController =
            GameObject.Find("CloudAnchorsExampleController")
                .GetComponent<CloudAnchorsExampleController>();
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        if (!PhotonNetwork.IsMasterClient && m_CloudAnchorId != string.Empty && !m_ShouldResolve && firstTime)
        {
            _ShowAndroidToastMessage("Master Created an Anchor");
            firstTime = false;
            m_ShouldResolve = true;
        }
        if (m_ShouldResolve)
        {
            _ResolveAnchorFromId(m_CloudAnchorId);
        }
    }

    /// <summary>
    /// Command run on the server to set the Cloud Anchor Id.
    /// </summary>
    /// <param name="cloudAnchorId">The new Cloud Anchor Id.</param>
    public void CmdSetCloudAnchorId(string cloudAnchorId)
    {
        m_CloudAnchorId = cloudAnchorId;
    }

    /// <summary>
    /// Gets the Cloud Anchor Id.
    /// </summary>
    /// <returns>The Cloud Anchor Id.</returns>
    public string GetCloudAnchorId()
    {
        return m_CloudAnchorId;
    }

    /// <summary>
    /// Hosts the user placed cloud anchor and associates the resulting Id with this object.
    /// </summary>
    /// <param name="lastPlacedAnchor">The last placed anchor.</param>
    public void HostLastPlacedAnchor(Component lastPlacedAnchor)
    {
        var anchor = (Anchor)lastPlacedAnchor;

        _ShowAndroidToastMessage("Hosting request sent");
        XPSession.CreateCloudAnchor(anchor).ThenAction(result =>
        {
            if (result.Response != CloudServiceResponse.Success)
            {
                Debug.Log(string.Format("Failed to host Cloud Anchor: {0}", result.Response));
                _ShowAndroidToastMessage("hosting failed");
                m_CloudAnchorsExampleController.OnAnchorHosted(false, result.Response.ToString());
                return;
            }

            Debug.Log(string.Format(
                "Cloud Anchor {0} was created and saved.", result.Anchor.CloudId));
            CmdSetCloudAnchorId(result.Anchor.CloudId);
            _ShowAndroidToastMessage("Hosted");
            m_CloudAnchorsExampleController.OnAnchorHosted(true, result.Response.ToString());
        });
    }

    /// <summary>
    /// Resolves an anchor id and instantiates an Anchor prefab on it.
    /// </summary>
    /// <param name="cloudAnchorId">Cloud anchor id to be resolved.</param>
    private void _ResolveAnchorFromId(string cloudAnchorId)
    {
        //m_CloudAnchorsExampleController.OnAnchorInstantiated(false);

        // If device is not tracking, let's wait to try to resolve the anchor.
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        m_ShouldResolve = false;


        XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction(
            (System.Action<CloudAnchorResult>)(result =>
                {
                    if (result.Response != CloudServiceResponse.Success)
                    {
                        Debug.LogError(string.Format(
                            "Client could not resolve Cloud Anchor {0}: {1}",
                            cloudAnchorId, result.Response));

                            //m_CloudAnchorsExampleController.OnAnchorResolved(
                            //    false, result.Response.ToString());
                            m_ShouldResolve = true;
                        _ShowAndroidToastMessage("resolve failed trying again");
                        return;
                    }

                    Debug.Log(string.Format(
                        "Client successfully resolved Cloud Anchor {0}.",
                        cloudAnchorId));

                        //m_CloudAnchorsExampleController.OnAnchorResolved(
                        //    true, result.Response.ToString());
                        _ShowAndroidToastMessage("resolving success");
                    _OnResolved(result.Anchor.transform);
                }));
    }

    /// <summary>
    /// Callback invoked once the Cloud Anchor is resolved.
    /// </summary>
    /// <param name="anchorTransform">Transform of the resolved Cloud Anchor.</param>
    private void _OnResolved(Transform anchorTransform)
    {
        var cloudAnchorController = GameObject.Find("CloudAnchorsExampleController")
                                              .GetComponent<CloudAnchorsExampleController>();
        cloudAnchorController.SetWorldOrigin(anchorTransform);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_CloudAnchorId);
        }
        else
        {
            m_CloudAnchorId = (string)stream.ReceiveNext();
        }
    }

    /// <summary>
    /// Android toast messages for debugging purposes
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
