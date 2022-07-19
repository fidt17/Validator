namespace fidt17.UnityValidationModule.Editor
{
    public enum ValidationScopeEnum
    {
        /// <summary>
        /// Validate all objects on current scene
        /// </summary>
        ActiveScene = 0,
        
        /// <summary>
        /// Validate all objects in every scene that goes to build 
        /// </summary>
        InBuildScenes = 1,
        
        /// <summary>
        /// Validate all Prefabs and ScriptableObjects in project
        /// </summary>
        ProjectAssets = 2,
        
        /// <summary>
        /// Validate all build scenes, all prefabs and all ScriptableObjects
        /// </summary>
        Everything = 3,
    }
}