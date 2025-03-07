using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static CalibrationDataLoader;
using static UnityEngine.GraphicsBuffer;

#if UNITY_EDITOR
[CustomEditor(typeof(CalibrationDataLoader))]
public class CalibrationLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var loader = (CalibrationDataLoader)target;

        loader.type = (CalibrationFileType)EditorGUILayout.EnumPopup("Calibration File Type", loader.type);

        if (loader.type == CalibrationFileType.Specific)
        {
            loader.specificCsvPath = EditorGUILayout.TextField("Specific CSV Path", loader.specificCsvPath);
        }

        //if (GUILayout.Button("Load Calibration File"))
        //{
        //    loader.LoadCalibrationFile();
        //}

        EditorUtility.SetDirty(loader);
    }
}
#endif
