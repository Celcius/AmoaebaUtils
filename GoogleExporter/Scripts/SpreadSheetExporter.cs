using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using AmoaebaUtils;
using System.IO;
using GoogleSheetsToUnity;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmoaebaUtils.GoogleExporter
{

public class SpreadSheetExporter : ScriptableObject
{
    private const string ENTRY_CLASS_POSTFIX = "Entry";
    private const string AGGREGATE_CLASS_POSTFIX = "Entries";
    private const string AGGREGATE_CLASS_FIELD = "entries";
    private const string AGGREGATE_CLASS_FILL_METHOD = "Populate";
    private const string COLUMN_POSTFIX = "Val";

    [SerializeField]
    private string googleKey="";
        
    [SerializeField]
    private bool isPrivateSheet = false;
    
    [SerializeField]
    private string[] SheetsToExport = null;

    [Space()]
        
    [SerializeField]
    private string outputNamespace = "Config";

    [SerializeField]
    private string outputPath= "Assets/Generated/";
    
    private string folderPath => outputPath + (outputPath.EndsWith("/") ? this.name : "/" + this.name);

    private enum ParseType
    {
        Int,
        Float,
        String
    }

    [Serializable]
    private struct ColumnExporter
    {
        public string Name => sheetName + COLUMN_POSTFIX;
        public string sheetName;
        public ParseType Type;
        
        public object ParseTypeForValue(string value)
        {
            switch (Type)
            {
                case ParseType.Int:
                    return int.Parse(value);
                case ParseType.Float:
                    value = value.Replace('.',',');
                    return float.Parse(value);
                case ParseType.String:
                    return value;        
            }
            
            Debug.LogError("Parsing type fell through switch case for " + value);
            return value;
        }
    }
    public void ExportClasses()
    {
        Export(false);
    }

    public void ExportInstances()
    {
        Export(true);
    }

    private void Export(bool exportInstances)
    {
        if(!CanExport())
        {
            Debug.LogError("Could not export " + this.name + " due to missing parameters");
            return;
        }

        FileUtils.GenerateFoldersForPath(folderPath);
        
        foreach(string sheet in SheetsToExport)
        {
            if(exportInstances)
            {
                ReadInstancesSheet(googleKey, sheet, isPrivateSheet);
            }
            
            else
            {
                ReadClassesSheet(googleKey, sheet, isPrivateSheet);
            }
        }

        AssetDatabase.Refresh();
    }

    public bool CanExport()
    {
        return SheetsToExport != null 
                && SheetsToExport.Length > 0
                && googleKey != null
                && googleKey != string.Empty;
    }

        public void ReadClassesSheet(string googleKey, string sheet, bool isPrivate)
        {
            if (isPrivate)
            {
                SpreadsheetManager.Read(new GSTU_Search(googleKey, sheet), onSheetClassesRead);
            }
            else
            {
                SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(googleKey, sheet), onSheetClassesRead);
            }           
        }

        public void ReadInstancesSheet(string googleKey, string sheet, bool isPrivate)
        {
            if (isPrivate)
            {
                SpreadsheetManager.Read(new GSTU_Search(googleKey, sheet), onSheetInstancesRead);
            }
            else
            {
                SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(googleKey, sheet), onSheetInstancesRead);
            }           
        }

        internal void onSheetClassesRead(GstuSpreadSheet sheet)
        {
            onSheetRead(sheet, false);
        }

        internal void onSheetInstancesRead(GstuSpreadSheet sheet)
        {
            onSheetRead(sheet, true);
        }

        internal void onSheetRead(GstuSpreadSheet sheet, bool generateInstances)
        {   
            List<ColumnExporter> exporters = ReadExporters(sheet);
            
            string sheetName = GSTUSheetUtils.GetSheetName(sheet);
          
            GenerateClasses(sheetName, exporters);
            
            if(generateInstances)
            {
                GenerateInstances(sheet, sheetName, exporters);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private List<ColumnExporter> ReadExporters(GstuSpreadSheet sheet)
        {
            List<GSTU_Cell> columnNames = GSTUSheetUtils.ColumnNamesRow(sheet);
            List<GSTU_Cell> columnTypes = GSTUSheetUtils.TypesRow(sheet);
            List<ColumnExporter> retVal = new List<ColumnExporter>();

            int max = Mathf.Min(columnNames.Count, columnTypes.Count);
            for(int i = 0; i < max; i++)
            {
                ColumnExporter exporter;
                exporter.sheetName = columnNames[i].value;
                exporter.Type = StringToParseType(columnTypes[i].value);
                retVal.Add(exporter);
            }

            return retVal;
        }

        private ParseType StringToParseType(string type)
        {
            type = type.ToLower();
            switch (type)
            {
                case "int":
                return ParseType.Int;
                case "float":
                return ParseType.Float;
                case "string":
                return ParseType.String;
            }

            Debug.LogError("ParseType fell through switch-case in " + this.name);
            return ParseType.String;
        }

        private void GenerateClasses(string spreadsheetName, List<ColumnExporter> exporters)
        {
            Action<StreamWriter> writeEntryBody = (StreamWriter outfile) => {
                                    foreach(ColumnExporter exporter in exporters)
                                    {
                                        WriteField(outfile, exporter);
                                    }
                                };

            FileUtils.GenerateClass(GetEntryClassName(spreadsheetName),
                                    outputNamespace,
                                    folderPath,
                                    writeEntryBody);
            
            FileUtils.GenerateClass(GetAggregateClassName(spreadsheetName),
            outputNamespace,
            folderPath,
            (StreamWriter outfile) => WriteAggregateBody(outfile, spreadsheetName));
        }

        private string GetEntryClassName(string sheetName)
        {
            return sheetName + ENTRY_CLASS_POSTFIX;
        }

        private string GetAggregateClassName(string sheetName)
        {
            return sheetName + AGGREGATE_CLASS_POSTFIX;
        }

        private void WriteAggregateBody(StreamWriter outfile, string sheetName)
        {
            string entryTypeName = GetEntryClassName(sheetName);

            outfile.WriteLine("");
            outfile.WriteLine(" public " + entryTypeName+ "[] " + AGGREGATE_CLASS_FIELD + ";");
            outfile.WriteLine("");
            outfile.WriteLine(" private void " + AGGREGATE_CLASS_FILL_METHOD + "(Array array) {");
            outfile.WriteLine("  "+ AGGREGATE_CLASS_FIELD + "= new "+ entryTypeName + "[array.Length];");
            outfile.WriteLine("  for(int i = 0; i < array.Length;i++)");
            outfile.WriteLine("  {");
            outfile.WriteLine("   " + AGGREGATE_CLASS_FIELD + "[i] = ("+entryTypeName+")array.GetValue(i);");
            outfile.WriteLine("  }");
            outfile.WriteLine(" }");
            outfile.WriteLine("");
        }

        private void WriteField(StreamWriter outfile, ColumnExporter exporter)
        {
            outfile.WriteLine("");
            outfile.Write("public ");
            
            switch (exporter.Type)
            {
                case ParseType.Int:
                outfile.Write("int ");
                break;
                
                case ParseType.Float:
                outfile.Write("float ");
                break;

                case ParseType.String:
                outfile.Write("string ");
                break;
            }

            outfile.Write(""+ exporter.Name + ";\n");
            outfile.WriteLine("");
        }

        private void GenerateInstances(GstuSpreadSheet sheet, 
                                       string spreadsheetName, 
                                       List<ColumnExporter> exporters)
        {
            List<GSTU_Cell> keys = GSTUSheetUtils.GetSheetRowKeys(sheet);
            string instancesFolder = folderPath + "/" + spreadsheetName + "/";
            FileUtils.GenerateFoldersForPath(instancesFolder);

            string className = GetAggregateClassName(spreadsheetName);
            Type aggregateType = AmoaebaUtils.ReflectionHelpers.GetTypeFromName(""+ outputNamespace + "." + className);
            ScriptableObject aggregateInstance = ScriptableObject.CreateInstance(aggregateType);

            className = GetEntryClassName(spreadsheetName);
            Type classType = AmoaebaUtils.ReflectionHelpers.GetTypeFromName(""+ outputNamespace + "." + className);
            Type arrayType = classType.MakeArrayType();

            Array instances = Array.CreateInstance(arrayType.GetElementType(), keys.Count); 
            
            for(int i = 0; i < keys.Count; i++)
            {
                GSTU_Cell key = keys[i];
                ScriptableObject instance = ScriptableObject.CreateInstance(classType);
                instances.SetValue(instance,i);

                foreach(ColumnExporter exporter in exporters)
                {
                    GSTU_Cell entry = sheet[key.value, exporter.sheetName];
                    FieldInfo columnField = classType.GetField(exporter.Name,
                                                                BindingFlags.Public 
                                                                | BindingFlags.Instance);
                    columnField.SetValue(instance, exporter.ParseTypeForValue(entry.value));
                }

                AssetDatabase.CreateAsset(instance, instancesFolder + key.value + ".asset");
            }
            AssetDatabase.CreateAsset(aggregateInstance, instancesFolder + spreadsheetName + "Entries.asset");

            MethodInfo aggregateMethod = aggregateType.GetMethod(AGGREGATE_CLASS_FILL_METHOD,
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.Instance);
            object[] arguments = {instances};
            aggregateMethod.Invoke(aggregateInstance, arguments);
        }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpreadSheetExporter))]
public class SpreadSheetExporterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        SpreadSheetExporter exporter = (SpreadSheetExporter)target;
        if(exporter == null)
        {
            return;
        }

        bool compiling = BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling;
        bool CanExport = exporter.CanExport() && !compiling;

        EditorGUI.BeginDisabledGroup(CanExport == false);
        if(GUILayout.Button("Generate Classes"))
        {
            exporter.ExportClasses();
        }
        
        if(GUILayout.Button("Generate Instances"))
        {
            exporter.ExportInstances();
        }
        EditorGUI.EndDisabledGroup();

        if(compiling)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = Color.red;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wait for classes to compile", style);
            EditorGUILayout.Space();
        }

    }
}
#endif
}
