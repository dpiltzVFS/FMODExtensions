using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODExtensions
{

    public class FMODGameObjectInfo : MonoBehaviour
    {
        [SerializeField] private FMODObjectSize _objectSize;

        public FMODObjectSize ObjectSize => _objectSize;

        private void OnDrawGizmosSelected()
        {
            if (ObjectSize != null)
            {
#if UNITY_EDITOR
                Texture2D background = new Texture2D(1, 1, TextureFormat.RGB24, false);
                background.SetPixel(0, 0, Color.black);
                background.Apply();
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.cyan;
                style.normal.background = background;
                UnityEditor.Handles.Label(transform.position, $"FMODGameObjectSize " +
                    $": {ObjectSize.name}", style);
#endif

            }
        }
    }
}
