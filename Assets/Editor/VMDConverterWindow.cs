using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VMDConverterWindow : EditorWindow
{
    private string[] vmdFilePaths = new string[0];

    [MenuItem("Tools/VMD Converter")]
    public static void ShowWindow()
    {
        GetWindow<VMDConverterWindow>("VMD Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("VMD to Unity Animation Converter", EditorStyles.boldLabel);

        if (GUILayout.Button("Select VMD Files"))
        {
            // 複数ファイル選択のため、複数回呼び出すか、
            // EditorUtility.OpenFilePanelは標準で複数選択非対応なので
            // ここでは代替として一度に選択できるダイアログを自作する方法や
            // ユーザーに複数回選択してもらう形を検討してください。
            // 簡易的には複数選択ダイアログをサードパーティ製などで実装可能ですが、
            // 今回は簡単に一回で一つのファイル選択にとどめます。

            // もしくは複数選択対応するUnityのEditorUtilityは存在しないため、
            // 代替として複数回選択したパスをカンマ区切りで受け取るなどのUIを作る方法もあります。

            // ここでは簡易的に1ファイル選択し追加する形にしておきます。
            string path = EditorUtility.OpenFilePanel("Select VMD File", "", "vmd");
            if (!string.IsNullOrEmpty(path))
            {
                List<string> list = new List<string>(vmdFilePaths);
                if (!list.Contains(path)) list.Add(path);
                vmdFilePaths = list.ToArray();
            }
        }

        if (vmdFilePaths.Length > 0)
        {
            GUILayout.Label("Selected Files:");
            foreach (var path in vmdFilePaths)
            {
                GUILayout.Label(Path.GetFileName(path));
            }

            if (GUILayout.Button("Clear Selection"))
            {
                vmdFilePaths = new string[0];
            }

            if (GUILayout.Button("Convert All"))
            {
                foreach (var path in vmdFilePaths)
                {
                    ConvertVMDToAnimation(path);
                }
                EditorUtility.DisplayDialog("完了", "全てのファイルの変換が完了しました。", "OK");
            }
        }
    }

    void ConvertVMDToAnimation(string path)
    {
        try
        {
            List<VMDMotion> motions = ReadVMD(path);
            Dictionary<string, AnimationCurve[]> boneCurves = new Dictionary<string, AnimationCurve[]>();

            foreach (var motion in motions)
            {
                if (!boneCurves.ContainsKey(motion.boneName))
                {
                    boneCurves[motion.boneName] = new AnimationCurve[7]; // posX, posY, posZ, rotX, rotY, rotZ, rotW
                    for (int i = 0; i < 7; i++) boneCurves[motion.boneName][i] = new AnimationCurve();
                }

                float time = motion.frameNo / 30f; // assuming 30fps

                boneCurves[motion.boneName][0].AddKey(time, motion.pos.x);
                boneCurves[motion.boneName][1].AddKey(time, motion.pos.y);
                boneCurves[motion.boneName][2].AddKey(time, motion.pos.z);
                boneCurves[motion.boneName][3].AddKey(time, motion.rot.x);
                boneCurves[motion.boneName][4].AddKey(time, motion.rot.y);
                boneCurves[motion.boneName][5].AddKey(time, motion.rot.z);
                boneCurves[motion.boneName][6].AddKey(time, motion.rot.w);
            }

            AnimationClip clip = new AnimationClip();
            clip.legacy = false;

            foreach (var pair in boneCurves)
            {
                string bonePath = "Armature/" + pair.Key; // 実際のボーンパスに合わせて必要なら修正
                var curves = pair.Value;

                clip.SetCurve(bonePath, typeof(Transform), "localPosition.x", curves[0]);
                clip.SetCurve(bonePath, typeof(Transform), "localPosition.y", curves[1]);
                clip.SetCurve(bonePath, typeof(Transform), "localPosition.z", curves[2]);
                clip.SetCurve(bonePath, typeof(Transform), "localRotation.x", curves[3]);
                clip.SetCurve(bonePath, typeof(Transform), "localRotation.y", curves[4]);
                clip.SetCurve(bonePath, typeof(Transform), "localRotation.z", curves[5]);
                clip.SetCurve(bonePath, typeof(Transform), "localRotation.w", curves[6]);
            }

            string dir = "Assets/Animations";
            Directory.CreateDirectory(dir);

            string fileName = Path.GetFileNameWithoutExtension(path);
            string savePath = $"{dir}/{fileName}_Converted.anim";

            AssetDatabase.CreateAsset(clip, savePath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Converted and saved animation: {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError("変換中にエラーが発生しました: " + e.Message);
            EditorUtility.DisplayDialog("エラー", "変換中にエラーが発生しました。\n" + e.Message, "OK");
        }
    }

    private List<VMDMotion> ReadVMD(string path)
    {
        List<VMDMotion> motions = new List<VMDMotion>();

        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            reader.ReadBytes(30); // header
            reader.ReadBytes(20); // model name

            uint motionCount = reader.ReadUInt32();

            for (int i = 0; i < motionCount; i++)
            {
                string boneName = Encoding.GetEncoding("shift_jis").GetString(reader.ReadBytes(15)).TrimEnd('\0');
                uint frameNo = reader.ReadUInt32();

                float posX = reader.ReadSingle();
                float posY = reader.ReadSingle();
                float posZ = reader.ReadSingle();

                float rotX = reader.ReadSingle();
                float rotY = reader.ReadSingle();
                float rotZ = reader.ReadSingle();
                float rotW = reader.ReadSingle();

                reader.ReadBytes(64); // interpolation

                motions.Add(new VMDMotion
                {
                    boneName = boneName,
                    frameNo = frameNo,
                    pos = new Vector3(posX, posY, posZ),
                    rot = new Quaternion(rotX, rotY, rotZ, rotW)
                });
            }
        }

        return motions;
    }

    private class VMDMotion
    {
        public string boneName;
        public uint frameNo;
        public Vector3 pos;
        public Quaternion rot;
    }
}
