using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildPostprocessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 1; // 실행 순서, 1로 설정

    public void OnPostprocessBuild(BuildReport report)
    {
        string buildPath = Path.GetDirectoryName(report.summary.outputPath);
        string steamAppIdPath = Path.Combine(buildPath, "steam_appid.txt");

        // Steam 앱 ID
        string steamAppId = "3421480";

        try
        {
            // steam_appid.txt 파일 생성
            File.WriteAllText(steamAppIdPath, steamAppId);
            UnityEngine.Debug.Log($"steam_appid.txt 생성 완료: {steamAppIdPath}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"steam_appid.txt 생성 실패: {e.Message}");
        }
    }
}