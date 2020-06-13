using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

namespace AmoaebaUtils.GoogleExporter
{

[Serializable]
public static class GSTUSheetUtils
{   
    private const int GSTU_START_ROW_INDEX = 1;
    private const string GSTU_START_COLUMN_KEY = "A";
    private const int ROW_DATA_OFFSET = 2;

    public static string GetSheetName(GstuSpreadSheet sheet) 
    {
        return sheet.rows.GetValueFromPrimary(GSTU_START_ROW_INDEX)[0].value;
    }
    public static List<GSTU_Cell> TypesRow(GstuSpreadSheet sheet) 
    {
        return GetRowWithoutFirstColumn(sheet, GSTU_START_ROW_INDEX+1);
    } 
    public static List<GSTU_Cell> ColumnNamesRow(GstuSpreadSheet sheet) 
    {
         return GetRowWithoutFirstColumn(sheet, GSTU_START_ROW_INDEX);
    } 

    public static List<GSTU_Cell> GetRowWithoutFirstColumn(GstuSpreadSheet sheet, int row)
    {
        List<GSTU_Cell> rowData = sheet.rows.GetValueFromPrimary(row);
        return rowData.GetRange(1, rowData.Count-1);
    }

    public static List<GSTU_Cell> GetSheetRowKeys(GstuSpreadSheet sheet)
    {
        List<GSTU_Cell> cellsData = sheet.columns.GetValueFromPrimary(GSTU_START_COLUMN_KEY);
        return cellsData.GetRange(ROW_DATA_OFFSET, cellsData.Count-ROW_DATA_OFFSET);
    }

}
}