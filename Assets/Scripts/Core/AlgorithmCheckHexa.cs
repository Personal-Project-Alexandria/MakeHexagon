using System;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmCheckHexa : MonoBehaviour
{
    private List<PosMap> _listPosMap = new List<PosMap>(6);

    private int _nRow;

    private int _nCol;

    private CellModel[,] _mapData;

    private void Awake()
    {
        this.InitListPosMap();
    }

    public void UpdateRowCol(int nRow, int nCol)
    {
        this._nRow = nRow;
        this._nCol = nCol;
    }

    private void InitListPosMap()
    {
        for (int i = 0; i < 3; i++)
        {
            this._listPosMap.Add(new PosMap(1, i));
        }
        for (int j = 2; j >= 0; j--)
        {
            this._listPosMap.Add(new PosMap(0, j));
        }
    }

    public List<PosMap> CheckHexa(List<CellModel> listHint, CellModel[,] mapData, PatternScript.Type eType)
    {
        List<PosMap> list = new List<PosMap>();
        int count = listHint.Count;
        if (count == 0 || eType == PatternScript.Type.SPECIAL)
        {
            return list;
        }
        this._mapData = mapData;
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = listHint[i].PosInMap;
            CellType type = listHint[i].Type;
            if (type != CellType.UP)
            {
                if (type == CellType.DOWN)
                {
                    List<PosMap> listTemp = this.CheckCellDown_1(posInMap, listHint[i].Color);
                    this.SumaryListCheckHexa(list, listTemp);
                    listTemp = this.CheckCellDown_2(posInMap, listHint[i].Color);
                    this.SumaryListCheckHexa(list, listTemp);
                    listTemp = this.CheckCellDown_3(posInMap, listHint[i].Color);
                    this.SumaryListCheckHexa(list, listTemp);
                }
            }
            else
            {
                List<PosMap> listTemp = this.CheckCellUp_1(posInMap, listHint[i].Color);
                this.SumaryListCheckHexa(list, listTemp);
                listTemp = this.CheckCellUp_2(posInMap, listHint[i].Color);
                this.SumaryListCheckHexa(list, listTemp);
                listTemp = this.CheckCellUp_3(posInMap, listHint[i].Color);
                this.SumaryListCheckHexa(list, listTemp);
            }
        }
        return list;
    }

    private void SumaryListCheckHexa(List<PosMap> listCheckHexa, List<PosMap> listTemp)
    {
        int count = listTemp.Count;
        if (count == 0)
        {
            return;
        }
        List<PosMap> list = new List<PosMap>();
        for (int i = 0; i < count; i++)
        {
            PosMap posMap = listTemp[i];
            if (!this.IsPosMapExist(listCheckHexa, posMap))
            {
                list.Add(posMap);
            }
        }
        listCheckHexa.AddRange(list);
        list.Clear();
    }

    private bool IsPosMapExist(List<PosMap> listCheckHexa, PosMap checkPosMap)
    {
        int count = listCheckHexa.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posMap = listCheckHexa[i];
            if (posMap.Row == checkPosMap.Row && posMap.Col == checkPosMap.Col)
            {
                return true;
            }
        }
        return false;
    }

    private List<PosMap> CheckCellUp_1(PosMap pos, ColorType eColorType)
    {
        int row = pos.Row;
        int nConvertCol = pos.Col - 1;
        return this.CheckCellGerenal(row, nConvertCol, eColorType);
    }

    private List<PosMap> CheckCellUp_2(PosMap pos, ColorType eColorType)
    {
        int nConvertRow = pos.Row - 1;
        int col = pos.Col;
        return this.CheckCellGerenal(nConvertRow, col, eColorType);
    }

    private List<PosMap> CheckCellUp_3(PosMap pos, ColorType eColorType)
    {
        int nConvertRow = pos.Row - 1;
        int nConvertCol = pos.Col - 2;
        return this.CheckCellGerenal(nConvertRow, nConvertCol, eColorType);
    }

    private List<PosMap> CheckCellDown_1(PosMap pos, ColorType eColorType)
    {
        int nConvertRow = pos.Row - 1;
        int nConvertCol = pos.Col - 1;
        return this.CheckCellGerenal(nConvertRow, nConvertCol, eColorType);
    }

    private List<PosMap> CheckCellDown_2(PosMap pos, ColorType eColorType)
    {
        int row = pos.Row;
        int nConvertCol = pos.Col - 2;
        return this.CheckCellGerenal(row, nConvertCol, eColorType);
    }

    private List<PosMap> CheckCellDown_3(PosMap pos, ColorType eColorType)
    {
        int row = pos.Row;
        int col = pos.Col;
        return this.CheckCellGerenal(row, col, eColorType);
    }

    private List<PosMap> CheckCellGerenal(int nConvertRow, int nConvertCol, ColorType eColorType)
    {
        List<PosMap> list = new List<PosMap>();
        int count = this._listPosMap.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posMap = this._listPosMap[i];
            int num = nConvertRow + posMap.Row;
            int num2 = nConvertCol + posMap.Col;
            if (!this.IsCellMatchColor(num, num2, eColorType))
            {
                list.Clear();
                break;
            }
            list.Add(new PosMap(num, num2));
        }
        return list;
    }

    private bool IsCellMatchColor(int nRowIndex, int nColIndex, ColorType eColorType)
    {
        return nRowIndex >= 0 && nColIndex >= 0 && nRowIndex < this._nRow && nColIndex < this._nCol && this._mapData[nRowIndex, nColIndex].Type != CellType.NONE && (this._mapData[nRowIndex, nColIndex].Color == eColorType || eColorType == ColorType.SPECIAL_COLOR);
    }
}
