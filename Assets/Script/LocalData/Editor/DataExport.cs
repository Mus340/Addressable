using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class DataExport
{
    [MenuItem("Data/Export Excel to JSON")]
    public static void ExportExcelToJson()
    {
        ReadExcelFiles();
    }


    private static void ReadExcelFiles()
    {
        string projectPath = Application.dataPath.Replace("/Assets", "");
        string excelPath = Path.Combine(projectPath, "Excel");
        string tempOutputPath = Path.Combine(projectPath, "Excel/Temp");
        string outputPath = Path.Combine(Application.dataPath, "Resources/Data");
        string jsonDataPath = Path.Combine(Application.dataPath, "Script/LocalData/JsonData");

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        if (!Directory.Exists(tempOutputPath))
        {
            Directory.CreateDirectory(tempOutputPath);
        }

        if (!Directory.Exists(jsonDataPath))
        {
            Directory.CreateDirectory(jsonDataPath);
        }
        else
        {
            string[] csFiles = Directory.GetFiles(jsonDataPath, "*.cs");

            foreach (string csFile in csFiles)
            {
                File.Delete(csFile);
            }
        }
        foreach (string excelFile in Directory.GetFiles(excelPath, "*.xls*"))
        {
            if ((File.GetAttributes(excelFile) & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                continue;
            }
            string tempExcelFilePath = Path.Combine(tempOutputPath, Path.GetFileName(excelFile));

            // 임시 파일로 복사
            File.Copy(excelFile, tempExcelFilePath, true);

            using (FileStream stream = File.Open(tempExcelFilePath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);


                using (excelReader)
                {
                    DataSet dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = false
                        }
                    });

                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        string structName = dataTable.TableName;
                        if (structName[0] == '_')
                        {
                            continue;
                        }

                        // Read data types from the second row
                        List<(string,string)> dataTypes = new List<(string,string)>();
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            // Ignore columns starting with the '#' character
                            if (dataTable.Rows[0][i].ToString().StartsWith("#"))
                            {
                                continue;
                            }

                            var filedName = dataTable.Rows[0][i].ToString();
                            var datatype = dataTable.Rows[1][i].ToString();

                            dataTypes.Add((filedName,datatype));
                        }
                        
                        // Create the struct
                        StringBuilder structBuilder = new StringBuilder();
                        structBuilder.AppendLine("public struct " + structName);
                        structBuilder.AppendLine("{");

                        for (int i = 0; i < dataTypes.Count; i++)
                        {
                            structBuilder.AppendLine($"    public {dataTypes[i].Item2} {dataTypes[i].Item1};");
                        }

                        structBuilder.AppendLine("}");

                        // Save the struct to a .cs file
                        string structFilePath = Path.Combine(jsonDataPath, $"{structName}.cs");
                        File.WriteAllText(structFilePath, structBuilder.ToString());

                        // Create the JSON data
                        List<Dictionary<string, object>> jsonDict = new List<Dictionary<string, object>>();

                        for (int rowIndex = 2; rowIndex < dataTable.Rows.Count; rowIndex++)
                        {
                            DataRow row = dataTable.Rows[rowIndex];
                            Dictionary<string, object> rowDict = new Dictionary<string, object>();

                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                string columnName = dataTable.Rows[0][i].ToString();
                                object cellValue = row[i];
                                Debug.Log($"{columnName}:{cellValue}");

                                // Check if the value can be parsed as an integer
                                if (int.TryParse(cellValue.ToString(), out int intValue))
                                {
                                    rowDict.Add(columnName,intValue);
                                }
                                else
                                {
                                    rowDict.Add(columnName,cellValue);
                                }
                            }
                            jsonDict.Add(rowDict);
                        }
                        string json = JsonConvert.SerializeObject(jsonDict.ToArray(), Formatting.Indented);
                        string encryptedJson = CryptoHelper.Encrypt(json);
                        string outputFileName = $"{dataTable.TableName}.json";
                        string jsonPath = Path.Combine(outputPath, outputFileName);
                        string tempJsonPath = Path.Combine(tempOutputPath, "."+outputFileName);
                        
                        File.WriteAllText(tempJsonPath, json);
                        File.WriteAllText(jsonPath, encryptedJson);
                    }
                }
                File.Delete(tempExcelFilePath);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Excel to JSON export completed.");
    }
}