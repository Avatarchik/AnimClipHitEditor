using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 単純なサンプルエディタ
public class AnimClipSampleEditor : EditorWindow
{

    // アニメーションクリップを再生させるオブジェクト
    private Animator animObject;
    // 再生したいアニメーションクリップ
    private AnimationClip animClip;

    [MenuItem("Window/AnimClipSampleEditor")]
    static void Open()
    {
        GetWindow<AnimClipSampleEditor>();
    }

    private float value;

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("character : ", GUILayout.Width(110));
        animObject = (Animator)EditorGUILayout.ObjectField(animObject, typeof(UnityEngine.Animator), true);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("animation clip : ", GUILayout.Width(110));
        animClip = (AnimationClip)EditorGUILayout.ObjectField(animClip, typeof(UnityEngine.AnimationClip), true);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (animObject == null || animClip == null)
        {
            GUILayout.Label("Please Setting character and animation clip");
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(true);

        }

        GUILayout.BeginHorizontal();
        value = EditorGUILayout.Slider(new GUIContent("TimeLine"), value, 0, 1, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        if (animObject == null || animClip == null)
        {
            EditorGUI.EndDisabledGroup();
        }

        //--------------------------------------------------------------------

        if (animObject != null && animClip != null)
        {
            animClip.SampleAnimation(animObject.gameObject, value);
        }

        Repaint();
    }

}