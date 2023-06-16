using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSimpleOneshot : MonoBehaviour
{   
    [SerializeField] FMODUnity.EventReference m_FmodEvent;

    public void PlayOneshot()
    {
        FMODUnity.RuntimeManager.PlayOneShot(m_FmodEvent, transform.position);
    }
}
