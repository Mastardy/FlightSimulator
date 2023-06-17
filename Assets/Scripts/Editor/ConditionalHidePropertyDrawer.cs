using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);        

        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }        
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        
        return -EditorGUIUtility.standardVerticalSpacing;

    }

    public bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        SerializedProperty sourcePropertyValue = null;

        if (!property.isArray)
        {
            string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
            string conditionPath = propertyPath.Replace(property.name, condHAtt.conditionalSourceField); //changes the path to the conditionalsource property path
            sourcePropertyValue = property.serializedObject.FindProperty(conditionPath) ?? property.serializedObject.FindProperty(condHAtt.conditionalSourceField);
        }
        else
        {
            sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.conditionalSourceField);
        }


        if (sourcePropertyValue != null)
        {
            return CheckPropertyType(condHAtt,sourcePropertyValue);         
        }

        return true;
    }

    private bool CheckPropertyType(ConditionalHideAttribute condHAtt, SerializedProperty sourcePropertyValue)
    {
        switch (sourcePropertyValue.propertyType)
        {                
            case SerializedPropertyType.Boolean:
                return sourcePropertyValue.boolValue;                
            case SerializedPropertyType.Enum:
                return sourcePropertyValue.enumValueIndex == condHAtt.enumIndex;
            default:
                Debug.LogError("Data type of the property used for conditional hiding [" + sourcePropertyValue.propertyType + "] is currently not supported");
                return true;
        }
    }
}