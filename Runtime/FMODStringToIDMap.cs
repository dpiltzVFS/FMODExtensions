using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace FMODExtensions
{
    [CreateAssetMenu(menuName = "FMOD/FMODStringToIDMap")]
    public class FMODStringToIDMap : SerializedScriptableObject
    {
        public Dictionary<string, float> map;
    }
}