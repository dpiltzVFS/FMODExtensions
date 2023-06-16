using UnityEngine;

public class FMODButtonHandler : MonoBehaviour
{
    public FMODUnity.EventReference buttonPressFmodEvent;
    public FMODUnity.EventReference buttonMouseoverFmodEvent;
    
    public void PlayButtonPressSound()
    { 
        FMODUnity.RuntimeManager.PlayOneShot(buttonPressFmodEvent,transform.position);
    }

    public void PlayButtonMouseoverSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonMouseoverFmodEvent,transform.position);
    }
}
