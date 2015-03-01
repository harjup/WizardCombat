using UnityEditor;


public class BuildTestScript
{
    public static void PerformBuild()
    {
        string[] scenes = {"Assets/Scenes/Start.unity"};
        BuildPipeline.BuildPlayer(scenes, "TestFolder/", BuildTarget.WebPlayer, BuildOptions.Development);
    }

    
    


}
