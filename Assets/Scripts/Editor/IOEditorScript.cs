using UnityEditor;

[CustomEditor(typeof(IOSystem), true)]
public class IOEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        IOSystem _ioSystem = (IOSystem)target;
        if(target.GetType() == typeof(EventOnlyIOSystem))
        {
            EditorGUILayout.HelpBox("EventOnlyInteract can ONLY use UnityEvents.", MessageType.Info);
            if(_ioSystem.GetComponent<InteractionEvent>() == null)
            {
                _ioSystem.useEvents = true;
                _ioSystem.gameObject.AddComponent<InteractionEvent>();
            }
        }
        else
        {
            base.OnInspectorGUI();
            if (_ioSystem.useEvents)
            {
                //we are using the component, add component
                if (_ioSystem.GetComponent<InteractionEvent>())
                    _ioSystem.gameObject.AddComponent<InteractionEvent>();
            }
            else
            {
                //we are NOT using the component, remove component
                if (_ioSystem.GetComponent<InteractionEvent>() != null)
                    DestroyImmediate(_ioSystem.GetComponent<InteractionEvent>());
            }
        }

    }
}
