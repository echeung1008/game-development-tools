using UnityEngine;

public static class TransformExtensions
{
    public static void SetLayerRecursively(this Transform transform, string layer)
    {
        int layerIndex = LayerMask.NameToLayer(layer);
        if (layerIndex != -1) transform.SetLayerRecursively(layerIndex);
        else Debug.LogWarning("Attempted to set the layer to a nonexistent layer: " + layer);
    }

    public static void SetLayerRecursively(this Transform transform, int layer)
    {
        if (transform == null) return;

        transform.gameObject.layer = layer;

        foreach (Transform child in transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child, layer);
            }
        }
    }
}
