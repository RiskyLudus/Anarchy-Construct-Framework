using UnityEngine;

namespace AnarchyConstructFramework.Core.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "Anarchy/Create Settings", order = 1)]
    public class AnarchySettings : ScriptableObject
    {
        public string RootNamespace = "Anarchy";
        public string PathToAnarchyConstructFramework = "Assets/Anarchy";
        public string PathToConstructs = "Assets/_Constructs";
    }
}
