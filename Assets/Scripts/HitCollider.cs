using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class HitCollider : MonoBehaviour {

    public HitType hitType;
    private CapsuleCollider collider;

    public void Disable()
    {
        if (collider == null)
        {
            collider = GetComponent<CapsuleCollider>();
        }
        collider.enabled = false;
    }

    public void Enable()
    {
        if (collider == null)
        {
            collider = GetComponent<CapsuleCollider>();
        }
        collider.enabled = true;
    }


    // colliderを表示してみる…。多少強引な計算があるので、
    // colliderの正確な位置とサイズを取る方法があれば…。
    void OnDrawGizmos()
    {
        if (collider && collider.enabled)
        {

            Vector3 offset = 
                transform.right * (collider.center.x / collider.height / 2) + 
                transform.up * (collider.center.y / collider.height / 2) + 
                transform.forward * (collider.center.z / collider.height / 2);

            Vector3 size = new Vector3(
                (collider.radius / 0.5f) * collider.transform.lossyScale.x,
                (collider.height / 2) * collider.transform.lossyScale.y, 
                (collider.radius / 0.5f) * collider.transform.lossyScale.z
                );

            Quaternion dir;
            switch (collider.direction)
            {
                case 0:
                    dir = Quaternion.Euler(Vector3.forward * 90);
                    break;
                case 1:
                    dir = Quaternion.Euler(Vector3.up * 90);
                    break;
                case 2:
                    dir = Quaternion.Euler(Vector3.right * 90);
                    break;
                default:
                    dir = Quaternion.Euler(Vector3.zero);
                    break;
            }
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawMesh(GetPrimitiveMesh(PrimitiveType.Capsule), collider.transform.position + offset, transform.rotation * dir, size);

        }
    }

    private Mesh GetPrimitiveMesh(PrimitiveType type)
    {

        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(gameObject);

        return mesh;

    }

}
