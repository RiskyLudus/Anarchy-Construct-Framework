using UnityEngine;

namespace Anarchy.Core.Common
{
    public class AnarchyData : ScriptableObject
    {
        [Tooltip("The addressable address for the construct prefab.")]
        public string constructPrefabAddress;
    }
}