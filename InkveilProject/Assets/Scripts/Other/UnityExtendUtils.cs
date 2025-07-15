using UnityEngine;
public static class UnityExtendUtils
{
    /// <summary>
    /// 获取子物体组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tran"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindComponent<T>(this Transform tran, string name) where T:Component
    {
        if (tran != null)
            return tran.Find(name).GetComponent<T>();
        else
            Debug.Log("tran is null");
        return null;
    }

    public static RectTransform RectTransform<T>(this T com)where T:Component
    {
        return com.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 获取物体对象（包括被禁用物体）
    /// </summary>
    /// <param name="name">物体名称</param>
    /// <returns>若未找到返回空</returns>
    public static GameObject GetGameObject(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<Transform>();

        for (int i = 0; i < all.Length; ++i)
        {
            //            Debug.Log(all[i].name);
            if (all[i].gameObject.name.Equals(name))
                return all[i].gameObject;
        }

        Debug.Log("can't find " + name + " gameobject");
        return null;
    }
}
