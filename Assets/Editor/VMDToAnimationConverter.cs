using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class VMDToAnimationConverter : EditorWindow
{// 複数VMDファイルのパスを保持
    List<string> vmdFilePaths = new List<string>();

    // 変換先フォルダ（パス）
    string saveFolderPath = "Assets/ConvertedAnimations";

    GameObject targetModel;

    [MenuItem("Tools/VMD → AnimationClip Converter")]
    static void OpenWindow()
    {
        GetWindow<VMDToAnimationConverter>("VMD Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("VMD to AnimationClip Converter", EditorStyles.boldLabel);

        targetModel = (GameObject)EditorGUILayout.ObjectField("Target Model (Humanoid Rig)", targetModel, typeof(GameObject), true);

        if (GUILayout.Button("Add VMD File"))
        {
            string path = EditorUtility.OpenFilePanel("Select VMD file", "", "vmd");
            if (!string.IsNullOrEmpty(path))
            {
                if (!vmdFilePaths.Contains(path))
                    vmdFilePaths.Add(path);
            }
        }

        GUILayout.Label("Selected VMD files:");
        for (int i = 0; i < vmdFilePaths.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Path.GetFileName(vmdFilePaths[i]), GUILayout.Width(300));
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                vmdFilePaths.RemoveAt(i);
                i--;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        // 保存先フォルダ指定
        GUILayout.BeginHorizontal();
        GUILayout.Label("Save Folder", GUILayout.Width(70));
        GUILayout.Label(saveFolderPath, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Save Folder", "Assets", "");
            if (!string.IsNullOrEmpty(folder))
            {
                // フォルダがAssetsフォルダ内にあるかチェック（相対パスに変換）
                if (folder.StartsWith(Application.dataPath))
                {
                    saveFolderPath = "Assets" + folder.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Save folder must be inside the project's Assets folder.", "OK");
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("Convert All VMD to AnimationClips"))
        {
            if (targetModel == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select target model.", "OK");
                return;
            }
            if (vmdFilePaths.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "Please add VMD files.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(saveFolderPath))
            {
                EditorUtility.DisplayDialog("Error", "Please select save folder.", "OK");
                return;
            }

            foreach (var vmdPath in vmdFilePaths)
            {
                string animName = Path.GetFileNameWithoutExtension(vmdPath) + ".anim";
                string savePath = Path.Combine(saveFolderPath, animName);

                ConvertVMDToAnimationClip(vmdPath, savePath, targetModel);
            }

            EditorUtility.DisplayDialog("Success", "All AnimationClips created.", "OK");
            AssetDatabase.Refresh();
        }
    }

    void ConvertVMDToAnimationClip(string vmdPath, string savePath, GameObject model)
    {
        // ここは既存の簡易処理のまま
        byte[] vmdData = File.ReadAllBytes(vmdPath);
        List<VMDKeyframe> keyframes = ParseVMDKeyframes(vmdData);
        if (keyframes == null || keyframes.Count == 0)
        {
            Debug.LogError($"Failed to parse VMD: {vmdPath}");
            return;
        }

        AnimationClip clip = new AnimationClip();
        clip.frameRate = 30f;

        var curveRotY = new AnimationCurve();

        foreach (var kf in keyframes)
        {
            if (kf.boneName == "センター")
            {
                float time = kf.frameNumber / 30f;
                float yRotation = kf.rotationEuler.y;
                curveRotY.AddKey(time, yRotation);
            }
        }

        clip.SetCurve("センター", typeof(Transform), "localEulerAngles.y", curveRotY);

        // フォルダパスが相対パスなのでAssetDatabaseで扱える
        AssetDatabase.CreateAsset(clip, savePath);
        AssetDatabase.SaveAssets();

        Debug.Log($"Created animation clip: {savePath}");
    }

    class VMDKeyframe
    {
        public string boneName;
        public int frameNumber;
        public Vector3 position;
        public Vector3 rotationEuler;
    }

    List<VMDKeyframe> ParseVMDKeyframes(byte[] data)
    {
        List<VMDKeyframe> dummy = new List<VMDKeyframe>();
        dummy.Add(new VMDKeyframe() { boneName = "センター", frameNumber = 0, rotationEuler = new Vector3(0, 0, 0) });
        dummy.Add(new VMDKeyframe() { boneName = "センター", frameNumber = 30, rotationEuler = new Vector3(0, 90, 0) });
        return dummy;
    }
}