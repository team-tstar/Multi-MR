// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEngine;


    [AddComponentMenu("Photon Networking/Photon Transform View")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    [RequireComponent(typeof(PhotonView))]
    public class PhotonTransformView : MonoBehaviour, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;
       // private float m_diff;

        private PhotonView m_PhotonView;

        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;
        //private Vector3 m_StoredScale;

        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = true;

        bool m_firstTake = false;
        //private Vector3 m_NetworkScale;

        public void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            m_StoredPosition = transform.position;
            m_NetworkPosition = Vector3.zero;
            //m_StoredScale = Vector3.one;
            m_NetworkRotation = Quaternion.identity;
            //m_NetworkScale = Vector3.one;
        }

        void OnEnable()
        {
            m_firstTake = true;
        }

        public void Update()
        {
            if (!this.m_PhotonView.IsMine)
            {
                transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
                //transform.localScale = Vector3.Lerp(transform.localScale,this.m_NetworkScale,this.m_diff * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_Direction = transform.position - this.m_StoredPosition;
                    this.m_StoredPosition = transform.position;

                    stream.SendNext(transform.position);
                    stream.SendNext(this.m_Direction);
                }

                if (this.m_SynchronizeRotation)
                {
                    stream.SendNext(transform.rotation);
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(transform.localScale);// - m_StoredScale
                }
            }
            else
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        transform.position = this.m_NetworkPosition;
                        this.m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        this.m_NetworkPosition += this.m_Direction * lag;
                        this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
                    }
                }

                if (this.m_SynchronizeRotation)
                {
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        this.m_Angle = 0f;
                        transform.rotation = this.m_NetworkRotation;
                    }
                    else
                    {
                        this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    this.gameObject.transform.localScale = (Vector3)stream.ReceiveNext();
                    // this.m_NetworkScale = (Vector3)stream.ReceiveNext();
                    //if (m_firstTake)
                    // {
                    //     transform.localScale = this.m_NetworkScale;
                    //     this.m_diff = 0;
                    // }
                    // else
                    // {
                    //     float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    //      this.m_diff = lag*Vector3.Distance(transform.localScale, this.m_NetworkScale);
                    //  }
                }

                if (m_firstTake)
                {
                    m_firstTake = false;
                }
            }
        }
    }
}