
using UnityEditor;
using UnityEngine;

public class FigmaToRectTool : EditorWindow
{
    private float figmaWidth = 1080f;
    private float figmaHeight = 1920f;

    private float left = 0f;
    private float top = 0f;
    private float elementWidth = 100f;
    private float elementHeight = 100f;

    private Vector2 pivot = new Vector2(0.5f, 0.5f);
    private AnchorPreset anchorPreset = AnchorPreset.MiddleCenter;

    private Vector2 result;

    [MenuItem("Tools/Figma → RectTransform")]
    public static void ShowWindow()
    {
        GetWindow<FigmaToRectTool>("Figma → RectTransform");
    }

    void OnGUI()
    {
        GUILayout.Label("📐 Figma Frame Settings", EditorStyles.boldLabel);
        figmaWidth = EditorGUILayout.FloatField("Frame Width", figmaWidth);
        figmaHeight = EditorGUILayout.FloatField("Frame Height", figmaHeight);

        GUILayout.Space(5);
        GUILayout.Label("🎯 Element Settings", EditorStyles.boldLabel);
        left = EditorGUILayout.FloatField("Left", left);
        top = EditorGUILayout.FloatField("Top", top);
        elementWidth = EditorGUILayout.FloatField("Element Width", elementWidth);
        elementHeight = EditorGUILayout.FloatField("Element Height", elementHeight);

        GUILayout.Space(5);
        GUILayout.Label("📍 Layout Options", EditorStyles.boldLabel);
        anchorPreset = (AnchorPreset)EditorGUILayout.EnumPopup("Anchor Preset", anchorPreset);
        pivot = EditorGUILayout.Vector2Field("Pivot (0-1)", pivot);

        GUILayout.Space(10);
        if (GUILayout.Button("Convert to Unity Position"))
        {
            result = ConvertFigmaToUnityPosition();
        }

        EditorGUILayout.HelpBox($"🎯 Unity Anchored Position:\nX: {result.x}, Y: {result.y}", MessageType.Info);

        if (GUILayout.Button("Apply to Selected UI Element"))
        {
            ApplyToSelectedUIElement(result);
        }
    }

    private Vector2 ConvertFigmaToUnityPosition()
    {
        Vector2 anchor = GetAnchor(anchorPreset);

        float anchorX = Mathf.Lerp(0, figmaWidth, anchor.x);
        float anchorY = Mathf.Lerp(0, figmaHeight, anchor.y);

        float elementCenterX = left + elementWidth * pivot.x;
        float elementCenterY = top + elementHeight * pivot.y;

        float unityX = elementCenterX - anchorX;
        float unityY = -(elementCenterY - anchorY); // Flip Y

        return new Vector2(Mathf.RoundToInt(unityX), Mathf.RoundToInt(unityY));
    }

    private void ApplyToSelectedUIElement(Vector2 position)
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("⚠️ Please select a UI GameObject in the Hierarchy.");
            return;
        }

        RectTransform rt = Selection.activeGameObject.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogWarning("⚠️ Selected GameObject does not have a RectTransform.");
            return;
        }

        Vector2 anchor = GetAnchor(anchorPreset);
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = position;

        // 🖼 Set native size if Image component exists
        UnityEngine.UI.Image image = Selection.activeGameObject.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.SetNativeSize();
            Debug.Log("🖼 Native size applied to image.");
        }
        else
        {
            Debug.Log("ℹ️ No Image component found; skipping SetNativeSize.");
        }
    }

    private Vector2 GetAnchor(AnchorPreset preset)
    {
        return preset switch
        {
            AnchorPreset.TopLeft => new Vector2(0, 1),
            AnchorPreset.TopCenter => new Vector2(0.5f, 1),
            AnchorPreset.TopRight => new Vector2(1, 1),
            AnchorPreset.MiddleLeft => new Vector2(0, 0.5f),
            AnchorPreset.MiddleCenter => new Vector2(0.5f, 0.5f),
            AnchorPreset.MiddleRight => new Vector2(1, 0.5f),
            AnchorPreset.BottomLeft => new Vector2(0, 0),
            AnchorPreset.BottomCenter => new Vector2(0.5f, 0),
            AnchorPreset.BottomRight => new Vector2(1, 0),
            _ => new Vector2(0.5f, 0.5f)
        };
    }

    public enum AnchorPreset
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}
