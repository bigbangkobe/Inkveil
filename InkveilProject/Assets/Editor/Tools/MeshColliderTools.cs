using UnityEngine;
using UnityEditor;
/* 
编辑：HML
操作方法：选择需要操作的对象，在编辑器里找到“MeshCollider”菜单，然后执行对应操作！
*/

/// <summary>
/// 创建/编辑MeshCollider
/// </summary>
public class MeshColliderTools : MonoBehaviour
{

    /// <summary>
    /// 创建MeshCollider
    /// </summary>
    [MenuItem("ELEJO/Tools/MeshCollider/Create")]
    static void Create()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders)
        {
            if (!item.gameObject.GetComponent<MeshCollider>())
            {
                Collider collider = item.gameObject.AddComponent<MeshCollider>();
                //打印出添加MeshCollider的对象名称
                Debug.Log("已为" + collider.gameObject.name + "创建MeshCollider！");
            }
            else
            {
                Debug.Log(item.gameObject.name + "已创建MeshCollider，无需再创建！");
            }
        }
    }

    /// <summary>
    /// 删除MeshCollider
    /// </summary>
    [MenuItem("ELEJO/Tools/MeshCollider/Delete")]
    static void Delete()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders)
        {
            if (item.gameObject.GetComponent<MeshCollider>())
            {
                //删除MeshCollider
                DestroyImmediate(item.gameObject.GetComponent<MeshCollider>());
                Debug.Log("删除了" + item.gameObject.name + "的MeshCollider！");
            }
            else
            {
                Debug.Log(item.gameObject.name + "没有MeshCollider，无法执行删除操作！");
            }
        }
    }

    /// <summary>
    /// 禁用MeshCollider
    /// </summary>
    [MenuItem("ELEJO/Tools/MeshCollider/Forbidden")]
    static void Forbidden()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders)
        {
            if (item.gameObject.GetComponent<MeshCollider>())
            {
                //禁用MeshCollider
                Collider collider = item.gameObject.GetComponent<MeshCollider>();
                if (collider.enabled)
                {
                    collider.enabled = false;
                    Debug.Log("禁用了" + collider.gameObject.name + "的MeshCollider！");
                }
                else
                {
                    Debug.Log(item.gameObject.name + "的MeshCollider已被禁用！");
                }
            }
            else
            {
                Debug.Log(item.gameObject.name + "没有MeshCollider！");
            }
        }
    }

    /// <summary>
    /// 启用MeshCollider
    /// </summary>
    [MenuItem("ELEJO/Tools/MeshCollider/StartUsing")]
    static void StartUsing()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders)
        {
            if (item.gameObject.GetComponent<MeshCollider>())
            {
                //启用MeshCollider
                Collider collider = item.gameObject.GetComponent<MeshCollider>();
                if (!collider.enabled)
                {
                    collider.enabled = true;
                    Debug.Log("启用了" + collider.gameObject.name + "的MeshCollider！");
                }
                else
                {
                    Debug.Log(item.gameObject.name + "的MeshCollider已经启用！");
                }
            }
            else
            {
                Debug.Log(item.gameObject.name + "没有MeshCollider！");
            }
        }
    }

    /// <summary>
    /// 启用MeshCollider的Canves
    /// </summary>
    [MenuItem("ELEJO/Tools/MeshCollider/OtherTools/StartUsingCanves")]
    static void StartUsingCanves()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders)
        {
            if (item.gameObject.GetComponent<MeshCollider>())
            {
                //启用MeshCollider
                MeshCollider collider = item.gameObject.GetComponent<MeshCollider>();
                if (!collider.convex)
                {
                    collider.convex = true;
                    Debug.Log("启用了" + collider.gameObject.name + "的MeshCollider的Convex！");
                }
                else
                {
                    Debug.Log(item.gameObject.name + "的MeshCollider的Convex已经启用！");
                }
            }
            else
            {
                Debug.Log(item.gameObject.name + "没有MeshCollider！");
            }
        }
    }

    /// <summary>
    /// 启用MeshCollider的CanvesTrigger
    /// </summary>
    [MenuItem("ELEJO/Tools/MeshCollider/OtherTools/StartUsingCanvesTrigger")]
    static void StartUsingCanvesTrigger()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer item in renders)
        {
            if (item.gameObject.GetComponent<MeshCollider>())
            {
                //启用MeshCollider
                MeshCollider collider = item.gameObject.GetComponent<MeshCollider>();
                if (collider.convex)
                {
                    if (!collider.isTrigger)
                    {
                        collider.isTrigger = true;
                        Debug.Log("开启了" + collider.gameObject.name + "的MeshCollider的触发器！");
                    }
                    else Debug.Log(collider.gameObject.name + "的MeshCollider的触发器已经开启！");
                }
                else
                {
                    collider.convex = true;
                    collider.isTrigger = true;
                    Debug.Log(item.gameObject.name + "的MeshCollider的Convex原没有启用，此次操作启用，并打开了触发器！");
                }
            }
            else
            {
                Debug.Log(item.gameObject.name + "没有MeshCollider！");
            }
        }
    }

}