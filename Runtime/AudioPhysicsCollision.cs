/* This is a script to support object collisions in the VFS Viking Village project. It's origins lie in the script written by someone who worked on the VFS
Kinetic project. It has been customized to work with Fmod. */

using UnityEngine;
using System.Collections;

public class AudioPhysicsCollision : MonoBehaviour 
{

	[SerializeField] FMODUnity.EventReference ObjectCollision;


	public float collisionEnterMinValue = 0.5f;
	public float collisionEnterMaxValue = 15f;
	public float collisionStayMinValue = 0.5f;
	public float collisionStayMaxValue = 15f;
	public bool debugLogCollisionInfo;
	
	private float nextCollisionTime = 0.0f;
	private float impactVelocity = 0.0f;
	private float previousImpactVelocity = 0.0f;
	
	private static float MIN_VELOCITY = 0.0f;
	private static float MAX_VELOCITY = 1.0f;
	private static float COLLISION_WINDOW = 0.33f;

	private Rigidbody Rigidbody;


    void OnCollisionEnter(Collision collision) 
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player")) return;
		
		float magnitude = collision.relativeVelocity.magnitude;
		
		#if UNITY_EDITOR		
		if (debugLogCollisionInfo)
			Debug.Log("OnCollisionEnter, magnitude: " + magnitude, gameObject);
		#endif
		
		if (magnitude > collisionEnterMinValue) 
			Collide(magnitude);	
	}
	
	void OnCollisionStay(Collision collision) 
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player")) return;

		float magnitude = collision.relativeVelocity.magnitude;
		
		#if UNITY_EDITOR		
		if (debugLogCollisionInfo)	
			Debug.Log("OnCollisionStay, magnitude: " + magnitude, gameObject);
		#endif
		
		if (magnitude > collisionStayMinValue)
			Collide(magnitude);
	}


    void Collide(float magnitude)
	{
		impactVelocity = ConvertVelocityRange(magnitude);
		if ( (Time.time > nextCollisionTime) || ( (impactVelocity - previousImpactVelocity) > ((MAX_VELOCITY - MIN_VELOCITY) * 0.33) ) )
		{
			nextCollisionTime = Time.time + COLLISION_WINDOW;

			//create Fmod event instance, set the impact velocity etc.
			if (!ObjectCollision.IsNull)
			{
				FMOD.Studio.EventInstance collision = FMODUnity.RuntimeManager.CreateInstance(ObjectCollision);
				collision.setParameterByName("CollisionVelocity", impactVelocity);
				collision.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
				collision.start();
				collision.release();
			}


			previousImpactVelocity = impactVelocity;
		}
	}
	
	float ConvertVelocityRange(float inputValue)
	{
		if (inputValue > collisionEnterMaxValue)
			inputValue = collisionEnterMaxValue;
		if (inputValue < collisionEnterMinValue)
			inputValue = collisionEnterMinValue;
		
		if (collisionEnterMinValue > collisionEnterMaxValue)
			Debug.LogWarning("AudioPhysicsCollision: Object's audio collision range Min > Max!", gameObject);
		
		float inputRange = (collisionEnterMaxValue - collisionEnterMinValue);
		float outputRange = (MAX_VELOCITY - MIN_VELOCITY);
		float outputValue = (((inputValue - collisionEnterMinValue) * outputRange) / inputRange) + MIN_VELOCITY;
		
		if (outputValue > MAX_VELOCITY)
			outputValue = MAX_VELOCITY;
		if (outputValue < MIN_VELOCITY)
			outputValue = MIN_VELOCITY;
		
		#if UNITY_EDITOR		
		if (debugLogCollisionInfo)
			Debug.Log("Converted velocity: " + outputValue, gameObject);
		#endif		
		
		return outputValue;	
	}
}
