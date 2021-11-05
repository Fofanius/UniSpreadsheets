﻿using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace UniSpreadsheets.Editor
{
    [CustomEditor(typeof(XlsxAssetImporter))]
    public class XlsxAssetImporterEditor : ScriptedImporterEditor
    {
        private string[] _encodings;
        private SerializedProperty _encodingIndex;
        private SerializedProperty _spreadsheetDataReceivers;

        public override bool showImportedObject => false;

        public override void OnEnable()
        {
            base.OnEnable();

            _encodings = Encoding.GetEncodings().Select(x => x.Name).ToArray();
            _encodingIndex = serializedObject.FindProperty("_encodingIndex");
            _spreadsheetDataReceivers = serializedObject.FindProperty("_spreadsheetDataReceivers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                _encodingIndex.intValue = EditorGUILayout.Popup("Encoding", _encodingIndex.intValue, _encodings);
                EditorGUILayout.PropertyField(_spreadsheetDataReceivers);
            }
            serializedObject.ApplyModifiedProperties();

            ApplyRevertGUI();
        }
    }
}
