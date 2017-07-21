using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Characterに付与
public class Fighter : MonoBehaviour {

    private bool isHitColliderON = true;

    private HitCollider[] hitColliders;

    void Start()
    {
        InitHitColliders();
    }

    private void InitHitColliders()
    {
        isHitColliderON = false;
        hitColliders = GetComponentsInChildren<HitCollider>();
    }

    public void OnHitEventStart(int hitEvtValue)
    {
        if (isHitColliderON)
        {
            return;
        }

        isHitColliderON = true;

        if (hitColliders == null || hitColliders.Length < 1)
        {
            InitHitColliders();
        }

        // 該当するcolliderをON
        foreach (var hit in hitColliders)
        {
            if (((int)hit.hitType & hitEvtValue) != 0)
            {
                hit.Enable();
            }
        }

    }

    public void OnHitEventEnd()
    {
        if (!isHitColliderON)
        {
            return;
        }

        isHitColliderON = false;
        if (hitColliders == null || hitColliders.Length < 1)
        {
            InitHitColliders();
        }

        foreach (var hit in hitColliders)
        {
            hit.Disable();
        }
    }
}
