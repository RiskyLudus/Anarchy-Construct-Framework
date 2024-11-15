using UnityEngine;

namespace Anarchy.Core.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "Anarchy/Core/Create Settings", order = 1)]
    public class AnarchySettings : ScriptableObject
    {
        public string RootNamespace = "MyProject";
        public string PathToAnarchyConstructFramework = "Assets/Anarchy";
        public string PathToConstructs = "Assets/_Constructs";
    }
}
