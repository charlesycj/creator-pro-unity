using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildPostprocessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 1; // ���� ����, 1�� ����

    public void OnPostprocessBuild(BuildReport report)
    {
        string buildPath = Path.GetDirectoryName(report.summary.outputPath);
        string steamAppIdPath = Path.Combine(buildPath, "steam_appid.txt");

        // Steam �� ID
        string steamAppId = "3421480";

        try
        {
            // steam_appid.txt ���� ����
            File.WriteAllText(steamAppIdPath, steamAppId);
            UnityEngine.Debug.Log($"steam_appid.txt ���� �Ϸ�: {steamAppIdPath}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"steam_appid.txt ���� ����: {e.Message}");
        }
    }
}