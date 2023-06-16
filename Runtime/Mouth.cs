using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class Mouth : MonoBehaviour
{
    class EventCallbackInfo
    {
        public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
        public bool SpeechPlaying = false;
    }

    [SerializeField] FMODUnity.EventReference FMOD_MainDXEvent;

    FMOD.Studio.EventInstance mouth;
    FMOD.Studio.PARAMETER_ID enemySpeechTypeID;
    FMOD.Studio.EVENT_CALLBACK beatCallback;

    EventCallbackInfo eventCallbackInfo;
    GCHandle timelineHandle;


    void Start()
    {
        eventCallbackInfo = new EventCallbackInfo();
        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        mouth = FMODUnity.RuntimeManager.CreateInstance(FMOD_MainDXEvent);

        FMOD.Studio.EventDescription mainDXEventDescription;
        mouth.getDescription(out mainDXEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION enemySpeechTypeParameterDescription;
        mainDXEventDescription.getParameterDescriptionByName("EnemySpeechType", out enemySpeechTypeParameterDescription);
        enemySpeechTypeID = enemySpeechTypeParameterDescription.id;

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(mouth, GetComponent<Transform>(), GetComponent<Rigidbody>());

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(eventCallbackInfo);

        // Pass the object through the userdata of the instance
        mouth.setUserData(GCHandle.ToIntPtr(timelineHandle));

        mouth.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.SOUND_STOPPED);
        mouth.start();
        
    }
    private void Update()
    {
        if (!eventCallbackInfo.SpeechPlaying)
            ShutMouth();
    }

    public void ReactSpeech()
    {
        eventCallbackInfo.SpeechPlaying = true;
        mouth.setParameterByID(enemySpeechTypeID, 2f);
    }

    public void DeathSpeech()
    {
        eventCallbackInfo.SpeechPlaying = true;
        mouth.setParameterByID(enemySpeechTypeID, 1f);
    }
    public void AttackSpeech()
    {
        eventCallbackInfo.SpeechPlaying = true;
        mouth.setParameterByID(enemySpeechTypeID, 0f);
    }
    public void EffortSpeech()
    {
        eventCallbackInfo.SpeechPlaying = true;
        mouth.setParameterByID(enemySpeechTypeID, 4f);
    }
    private void OnDestroy()
    {
        mouth.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        mouth.release();
    }

    public void ShutMouth()
    {
        mouth.setParameterByID(enemySpeechTypeID, 3f);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            EventCallbackInfo eventCallbackInfo = (EventCallbackInfo)timelineHandle.Target;

            switch (type)
            {

                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        eventCallbackInfo.LastMarker = parameter.name;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        eventCallbackInfo.SpeechPlaying = false;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        timelineHandle.Free();
                        break;
                    }
            }
        }
        return FMOD.RESULT.OK;
    }


}
