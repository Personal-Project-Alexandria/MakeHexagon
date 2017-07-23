using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellModel {

    private CellType _eType;

    private ColorType _eColor;

    private PosMap _position;

    public CellType Type
    {
        get
        {
            return this._eType;
        }
        set
        {
            this._eType = value;
        }
    }

    public ColorType Color
    {
        get
        {
            return this._eColor;
        }
        set
        {
            this._eColor = value;
        }
    }

    public PosMap PosInMap
    {
        get
        {
            return this._position;
        }
    }

    public CellModel(CellType eType, ColorType eColor, PosMap posInMap)
    {
        this._eType = eType;
        this._eColor = eColor;
        this._position = posInMap;
    }

    public void UpdateData(CellType eType, ColorType eColor, PosMap posInMap)
    {
        this._eType = eType;
        this._eColor = eColor;
        this._position = posInMap;
    }
}
