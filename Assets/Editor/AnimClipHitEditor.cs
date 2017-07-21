using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// こっちが本命の攻撃判定制御エディタ
public class AnimClipHitEditor : EditorWindow {
    private Animator animObject;

    [SerializeField]
    private AnimationClip animClip;


    [MenuItem("Window/AnimClipHitEditor")]
    static void Open()
    {
        GetWindow<AnimClipHitEditor>();
    }

    private float animClipTime;
    private float startTime;
    private float endTime;

    // どのパーツの攻撃判定を出すかのフラグ
    private bool toggleLeftHand;
    private bool toggleRightHand;
    private bool toggleLeftLeg;
    private bool toggleRightLeg;

    private AnimationEvent onHitEventStart;
    private AnimationEvent onHitEventEnd;

    private string tmpAnimClipName; // animClipの入れ替え更新判定用…何かいい方法無いかなぁ。

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

        if (animClip && !animClip.name.Equals(tmpAnimClipName))
        {
            tmpAnimClipName = animClip.name;
            onHitEventStart = null;
            onHitEventEnd = null;
            animClipTime = 0;
        }


        GUILayout.BeginHorizontal();
        animClipTime = EditorGUILayout.Slider(new GUIContent("TimeLine"), animClipTime, 0, 1, GUILayout.Width(300));

        GUILayout.EndHorizontal();
        


        GUILayout.BeginVertical();
        EditorGUILayout.MinMaxSlider(new GUIContent("Hit Evt Slider"), ref startTime, ref endTime, 0.0f, 1.0f);
        EditorGUILayout.LabelField("On Hit Evt Start = ", startTime.ToString());
        EditorGUILayout.LabelField("On Hit Evt End = ", endTime.ToString());

        GUILayout.EndVertical();
        EditorGUILayout.Space();

        //--------------------------------------------------------------------
        // 攻撃判定種別を選べる
        GUILayout.BeginVertical();
        GUILayout.Label("Hit Collider Type");

        GUILayout.BeginHorizontal();
        toggleLeftHand = EditorGUILayout.Toggle("Left Hand", toggleLeftHand);
        toggleRightHand = EditorGUILayout.Toggle("Right Hand", toggleRightHand);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        toggleLeftLeg = EditorGUILayout.Toggle("Left Leg", toggleLeftLeg);
        toggleRightLeg = EditorGUILayout.Toggle("Right Leg", toggleRightLeg);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        //--------------------------------------------------------------------
        // イベントを付与
        if (GUILayout.Button("Add Hit Event"))
        {
            var serialied = new SerializedObject(animClip);
            serialied.Update();

            var events = new List<AnimationEvent>();

            onHitEventStart = new AnimationEvent();
            onHitEventStart.time = startTime;
            onHitEventStart.functionName = "OnHitEventStart";
            onHitEventStart.intParameter = CreateHitEvent();
            events.Add(onHitEventStart);

            onHitEventEnd = new AnimationEvent();
            onHitEventEnd.time = endTime;
            onHitEventEnd.functionName = "OnHitEventEnd";
            events.Add(onHitEventEnd);

            AnimationUtility.SetAnimationEvents((AnimationClip)serialied.targetObject , events.ToArray());
            EditorUtility.SetDirty(serialied.targetObject);

            serialied.ApplyModifiedProperties();
        }
        EditorGUILayout.Space();


        //--------------------------------------------------------------------
        // 情報をなんとなくリスト表示してみる
        
        GUILayout.BeginVertical();
        if (animClip != null)
        {
            foreach (var aes in animClip.events)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Time = " + aes.time.ToString() , GUILayout.Width(200));
                GUILayout.Label("Func Name = " + aes.functionName, GUILayout.Width(200));
                GUILayout.Label("Hit = " + System.Convert.ToString(aes.intParameter , 2) , GUILayout.Width(200));


                GUILayout.EndHorizontal();

                if (aes.functionName.Equals("OnHitEventStart") && onHitEventStart == null)
                {
                    onHitEventStart = aes;
                    startTime = onHitEventStart.time;
                    SetHitEvent(aes.intParameter);

                    Repaint();
                }
                if (aes.functionName.Equals("OnHitEventEnd") && onHitEventEnd == null)
                {
                    onHitEventEnd = aes;
                    endTime = onHitEventEnd.time;

                    Repaint();
                }
            }
        }
        else
        {
            GUILayout.Label("No Hit Event. Please Press [Add Hit Event] Button.");
        }
        GUILayout.EndVertical();

        if (animObject == null || animClip == null)
        {
            EditorGUI.EndDisabledGroup();
        }

        //--------------------------------------------------------------------

        if (animObject != null && animClip != null)
        {
            animClip.SampleAnimation(animObject.gameObject, animClipTime);
        }

        if (onHitEventStart != null && onHitEventEnd != null && animObject != null)
        {
            // 範囲に入っていたら当たり判定を出す
            if (startTime <= animClipTime && endTime >= animClipTime)
            {
                var fighter = animObject.GetComponent<Fighter>();
                if (fighter != null)
                {
                    fighter.OnHitEventStart(onHitEventStart.intParameter);
                }
                
            }
            else
            {
                // 範囲から外れたら消す
                var fighter = animObject.GetComponent<Fighter>();
                if (fighter != null)
                {
                    fighter.OnHitEventEnd();
                }
            }
        }

        
    }

    //-------------------
    // private
    private int CreateHitEvent()
    {
        int result = 0;

        if (toggleLeftHand) result = (result | (int)HitType.LEFT_HAND);
        if (toggleRightHand) result = (result | (int)HitType.RIGHT_HAND);
        if (toggleLeftLeg) result = (result | (int)HitType.LEFT_LEG);
        if (toggleRightLeg) result = (result | (int)HitType.RIGHT_LEG);

        return result;
    }

    private void SetHitEvent(int hitEvt)
    {
        toggleLeftHand = false;
        toggleLeftLeg = false;
        toggleRightHand = false;
        toggleRightLeg = false;

        if ((hitEvt & (int)HitType.LEFT_HAND) != 0) toggleLeftHand = true;
        if ((hitEvt & (int)HitType.RIGHT_HAND) != 0) toggleRightHand = true;
        if ((hitEvt & (int)HitType.LEFT_LEG) != 0) toggleLeftLeg = true;
        if ((hitEvt & (int)HitType.RIGHT_LEG) != 0) toggleRightLeg = true;
    }

}
