using UnityEditor;

namespace AnarchyConstructFramework.Editor
{
    public class ConstructBindingFunctions
    {
        [MenuItem("Anarchy/Update Bindings")]
        static void UpdateBindings()
        {
            var settings = AnarchyConstructFrameworkEditorFunctions.GetSettings();
            string constructFolderPath = settings.PathToConstructs;
        }
    }
}
