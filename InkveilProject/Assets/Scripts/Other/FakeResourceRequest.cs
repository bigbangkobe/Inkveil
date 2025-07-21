using UnityEngine;

public class FakeResourceRequest : ResourceRequest
{
    public void SetAsset(Object asset)
    {
        typeof(ResourceRequest).GetField("m_Asset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(this, asset);

        typeof(AsyncOperation).GetField("m_IsDone", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(this, true);
    }
}
