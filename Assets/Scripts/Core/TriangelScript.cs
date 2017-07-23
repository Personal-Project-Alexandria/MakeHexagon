using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriangelScript : MonoBehaviour {

    public CellType _cellType;

    public GameObject _hint;

    public GameObject _patternTriangle;

    private int _nRow;

    private int _nCol;

    public int Row
    {
        get
        {
            return this._nRow;
        }
    }

    public int Col
    {
        get
        {
            return this._nCol;
        }
    }

    public CellType Type
    {
        get
        {
            return this._cellType;
        }
    }

    public void SetData(int nRow, int nCol, CellType eCellType)
    {
        this._nRow = nRow;
        this._nCol = nCol;
        this._cellType = eCellType;
        int num = (this._cellType != CellType.UP) ? -180 : 0;
        base.transform.localEulerAngles = new Vector3(0f, 0f, (float)num);
    }

    public void DisplayHint(Color colorUI)
    {
        this._hint.SetActive(true);
        this._hint.GetComponent<Image>().color = new Color(colorUI.r, colorUI.g, colorUI.b, 0.6f);
    }

    public void ClearHint()
    {
        this._hint.SetActive(false);
    }

    public void DisplayPatternTriangle(Color colorUI)
    {
        this._patternTriangle.SetActive(true);
        this._patternTriangle.transform.localScale = Vector3.one;
        this._patternTriangle.GetComponent<Image>().color = colorUI;
    }

    public void ClearPatternTriangle()
    {
        this._patternTriangle.SetActive(false);
    }
}
