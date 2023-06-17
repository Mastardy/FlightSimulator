using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    private Planet planet;
    private Editor settingsEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed) planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.settings, planet.OnSettingsUpdated, ref planet.settingsFoldout);
        
        if (GUILayout.Button("Generate Planet")) planet.GeneratePlanet();
    }

    private void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout)
    {
        if (settings == null) return;
        
        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings); 
        if (!foldout) return;
        
        using var check = new EditorGUI.ChangeCheckScope();
        
        CreateCachedEditor(settings, null, ref settingsEditor);
        settingsEditor.OnInspectorGUI();

        if (check.changed && onSettingsUpdated != null) onSettingsUpdated();
    }

    private void OnEnable()
    {
        planet = target as Planet;
    }
}