using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Gun)), CanEditMultipleObjects]
public class BulletSpreadVisualizer : Editor
{
	private SerializedProperty _vectors;

	public override void OnInspectorGUI()
    {
		Gun gunScript = (Gun) target;

        GUIContent dropdownName = new GUIContent("Gun Type");
        gunScript.gunIndex = EditorGUILayout.Popup(dropdownName, gunScript.gunIndex, gunScript.gunTypeArray);

		EditorUtility.SetDirty(target);

        base.OnInspectorGUI();
    }

	private void OnEnable()
	{
		_vectors = serializedObject.FindProperty("bulletSpread");
	}

	// Helper method to get a square rectangle of the correct aspect ratio
	private Rect GetCenteredRect(Rect rect, float aspect = 1f)
	{
		Vector2 size = rect.size;
		size.x = Mathf.Min(size.x, rect.size.y * aspect);
		size.y = Mathf.Min(size.y, rect.size.x / aspect);

		Vector2 pos = rect.min + (rect.size - size) * 0.5f;
		return new Rect(pos, size);
	}

	public override bool HasPreviewGUI()
	{
		return true;
	}

	public override GUIContent GetPreviewTitle()
	{
		return new GUIContent("Bullet Spread Pattern");
	}

	public override void DrawPreview(Rect rect)
	{
		rect = GetCenteredRect(rect);

		// Draw background of the rect we plot points in
		EditorGUI.DrawRect(rect, Color.black);

		float dotSize = 5; // size in pixels of the point we draw
		float halfDotSize = dotSize * 0.5f;

        //How big of a dimension the graph shows (4 means it'll show an axis fromm -2 to 2)
		float viewportSize = 4; 

		for (int i = 0; i < _vectors.arraySize; i++)
		{
			SerializedProperty vectorProperty = _vectors.GetArrayElementAtIndex(i);

			Vector2 vector = vectorProperty.vector2Value;

			Vector2 normalizedPosition = vector / new Vector2(viewportSize, -viewportSize);

			if (Mathf.Abs(normalizedPosition.x) > 0.5f || Mathf.Abs(normalizedPosition.y) > 0.5f)
			{
				// don't draw points outside our viewport
				continue;
			}

			Vector2 pixelPosition = rect.center + rect.size * normalizedPosition;

			EditorGUI.DrawRect(new Rect(pixelPosition.x - halfDotSize, pixelPosition.y - halfDotSize, dotSize, dotSize), Color.red);
		}
	}
}
