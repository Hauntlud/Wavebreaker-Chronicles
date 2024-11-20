using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

[ExecuteAlways]
public class RaycastBlockDisabler : MonoBehaviour
{
    // Function to get all child objects with raycast blocking and ensure they are not blocking raycasts
    [Button("Disable Raycast Blocking")]
    public void DisableRaycastBlocking()
    {
        // Get all components that can block raycasts in children
        Image[] images = GetComponentsInChildren<Image>(true);
        TextMeshProUGUI[] textMeshes = GetComponentsInChildren<TextMeshProUGUI>(true);
        CanvasGroup[] canvasGroups = GetComponentsInChildren<CanvasGroup>(true);
        Slider[] sliders = GetComponentsInChildren<Slider>(true);

        // Disable raycast blocking for each component
        foreach (var image in images)
        {
            if (image.raycastTarget)
            {
                image.raycastTarget = false;
                MarkAsDirty(image);
            }
        }

        foreach (var textMesh in textMeshes)
        {
            if (textMesh.raycastTarget)
            {
                textMesh.raycastTarget = false;
                MarkAsDirty(textMesh);
            }
        }

        foreach (var canvasGroup in canvasGroups)
        {
            if (canvasGroup.blocksRaycasts)
            {
                canvasGroup.blocksRaycasts = false;
                MarkAsDirty(canvasGroup);
            }
        }

        foreach (var slider in sliders)
        {
            if (slider.interactable)
            {
                slider.interactable = false;
                MarkAsDirty(slider);
            }
        }

        Debug.Log("Raycast blocking disabled and sliders set to non-interactable for all relevant child objects.");
    }

    // Helper function to mark objects as dirty for editor saving
    private void MarkAsDirty(Object obj)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(obj);
        }
#endif
    }
}