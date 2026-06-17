using UnityEngine;

public class MeshHidder : MonoBehaviour
{
    Renderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    public void Show()
    {
        SetRenderersEnabled(true);
    }

    public void Hide()
    {
        SetRenderersEnabled(false);
    }

    void SetRenderersEnabled(bool enabled)
    {
        if (renderers == null)
        {
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = enabled;
            }
        }
    }
}
