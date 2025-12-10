using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Sirenix.Utilities;

namespace FMODExtensions
{
    public class FMODCollisionEmitter : MonoBehaviour
    {
        [SerializeField] private EventReference ImpactOneShot;
        [SerializeField] private EventReference SlideSound;

        [SerializeField] private float _MinMagnitude = 0.5f;
        [SerializeField] private float _repeatTime = 0.33f;

        private float _nextPlayTime = 0f;

        Vector3 CollisionPoint;

        EventInstance slidingSound;
        bool SlidingSoundCreated = false;

        private Rigidbody _rb;
        private bool _hasRigidBody = true;

        [SerializeField] public string CollisionMagnitudeFMODParmName = "CollisionMagnitude";

        [SerializeField] public bool PrintCollisionEnterInfo = false;
        [SerializeField] public bool PrintCollisionStayInfo = false;

        private Vector3 OldAngularVelocity;

        private void Start()
        {
            _nextPlayTime = Time.time + _repeatTime;
            OldAngularVelocity.x = 0f;
            OldAngularVelocity.y = 0f;
            OldAngularVelocity.z = 0f;

            if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                _rb = rb;
            }
            else
            {
                _hasRigidBody = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            float collisionMagnitude = collision.relativeVelocity.magnitude;
            PlaySound(ImpactOneShot, collisionMagnitude);
        }

        private void OnCollisionEnter(Collision collision)
        {
            float collisionMagnitude = collision.relativeVelocity.magnitude;
            if (PrintCollisionEnterInfo)
                Debug.Log("CollisionEnter magnitude = " + collisionMagnitude + " otherObject = " + collision.gameObject.name);

            CollisionPoint = collision.GetContact(0).point;
            PlaySound(ImpactOneShot, collisionMagnitude);
        }


        private void PlaySound(EventReference eventReference, float magnitude)
        {
            
            if (Time.time <= _nextPlayTime || eventReference.IsNull) return;
            _nextPlayTime = Time.time + _repeatTime;

            if (magnitude > _MinMagnitude)
            {
                EventInstance instance = RuntimeManager.CreateInstance(eventReference);
                instance.setParameterByName("CollisionMagnitude", magnitude);
                instance.set3DAttributes(CollisionPoint.To3DAttributes());
                instance.start();
                instance.release();
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            //create sliding sound event if it doesn't exist
            //figure out the best sliding sound to play given the surfaces of both objects
            //Send the speed of the objects relative to each other
            //Determine what kind of collider is involved
            //If is is a box collider, trigger a sound when the box's angular rotation suddenly drops (it lands on it's side)
            if (_hasRigidBody)
            {
                if (PrintCollisionStayInfo)
                {
                    Debug.Log("AngularVelocity = " + _rb.angularVelocity + " otherObject = " + collision.gameObject.name);
                }

                if (_rb.angularVelocity.x - OldAngularVelocity.x > 0.70f && OldAngularVelocity.x != 0f)
                {
                    CollisionPoint = collision.GetContact(0).point;
                    PlaySound(ImpactOneShot, 5f);
                }

                else if (_rb.angularVelocity.y - OldAngularVelocity.y > 0.70f && OldAngularVelocity.y != 0f)
                {
                    CollisionPoint = collision.GetContact(0).point;
                    PlaySound(ImpactOneShot, 5f);
                }
                else if (_rb.angularVelocity.z - OldAngularVelocity.z > 0.70f && OldAngularVelocity.z != 0f)
                {
                    CollisionPoint = collision.GetContact(0).point;
                    PlaySound(ImpactOneShot, 5f);
                }

                OldAngularVelocity = _rb.angularVelocity;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            //destroy the sliding sound instance
        }

        private void SlidingSound()
        {
            if (!SlideSound.IsNull)
            {
                if (!SlidingSoundCreated)
                {
                    slidingSound = RuntimeManager.CreateInstance(SlideSound);
                    slidingSound.start();
                    SlidingSoundCreated = true;
                }
                slidingSound.set3DAttributes(CollisionPoint.To3DAttributes());
            }
        }
    }
} 