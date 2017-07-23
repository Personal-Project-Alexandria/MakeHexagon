using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class SavePatterModel
{
    public PatternScript.Type TypePattern;

    public ColorType Color;

    public int IndexInPanel;

    public int CountClick;
}

public class PatternScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IEventSystemHandler
{
    [System.Serializable]
    public enum Type
    {
        ONE_TRIANGLE,
        TWO_TRIANGLE,
        THREE_TRIANGLE,
        FOUR_TRIANGLE_1,
        FOUR_TRIANGLE_2,
        FIVE_TRIANGLE,
        SPECIAL
    }

    private const int ROW = 2;

    private const int COL = 3;

    private PatternScript.Type _eTypePattern;

    private ColorType _eColor = ColorType.NONE;

    private const string PATH_PATTERN_TRIANGLE = "Prefabs/PatternTriangle";

    private int _nNumberTriangle;

    private int _nIndexInPanel;

    private int _nWidthPattern;

    private int _nHeightPattern;

    private bool _bOneLine = true;

    private CellType[,] _map = new CellType[2, 3];

    private bool _bAllowClick = true;

    private bool _bBeginDrag;

    private List<Transform> _listPattern = new List<Transform>();

    private List<CellType> _listRotate = new List<CellType>(6);

    private List<PosMap> _listPosMap = new List<PosMap>(6);

    private int _nFinalIndex;

    private int _nCountRotateTwoAngle;

    private int _nCountClick;

    private int _nIndexPositionCenter;

    private CellType[,] _saveMap = new CellType[2, 3];

    private int _nSavePosCenter;

    private int _nSaveFinalIndex;

    private int _nSaveCountClick;

    private int _nSaveCountRotateTwoAngel;

    public PatternScript.Type TypePattern
    {
        get
        {
            return this._eTypePattern;
        }
    }

    public ColorType Color
    {
        get
        {
            return this._eColor;
        }
    }

    public int NumberTriangle
    {
        get
        {
            return this._nNumberTriangle;
        }
    }

    public int IndexInPanel
    {
        get
        {
            return this._nIndexInPanel;
        }
        set
        {
            this._nIndexInPanel = value;
        }
    }

    private void Awake()
    {
        this.InitMap();
        this.InitListPosMap();
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

    private void CreateListRotate()
    {
        int count = this._listPosMap.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posMap = this._listPosMap[i];
            this._listRotate.Add(this._map[posMap.Row, posMap.Col]);
        }
    }

    private void OnDestroy()
    {
        base.StopAllCoroutines();
    }

    public void CreatePattern(int nIndexInPanel, PatternScript.Type eTypePattern, ColorType eColor, Color colorUI)
    {
        this._nIndexInPanel = nIndexInPanel;
        this._eTypePattern = eTypePattern;
        this._eColor = eColor;
        int num = 0;
        int num2 = 0;
        this._nIndexPositionCenter = 0;
        switch (this._eTypePattern)
        {
            case PatternScript.Type.ONE_TRIANGLE:
                num = 1;
                num2 = 1;
                this._nNumberTriangle = 1;
                this._map[1, 0] = CellType.UP;
                this._bOneLine = true;
                this._nFinalIndex = 0;
                break;
            case PatternScript.Type.TWO_TRIANGLE:
                num = 1;
                num2 = 2;
                this._nNumberTriangle = 2;
                this._map[1, 0] = CellType.DOWN;
                this._map[1, 1] = CellType.UP;
                this._bOneLine = true;
                this._nFinalIndex = 1;
                break;
            case PatternScript.Type.THREE_TRIANGLE:
                num = 1;
                num2 = 3;
                this._nNumberTriangle = 3;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._bOneLine = true;
                this._nFinalIndex = 2;
                break;
            case PatternScript.Type.FOUR_TRIANGLE_1:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 4;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._map[0, 2] = CellType.DOWN;
                this._bOneLine = false;
                this._nFinalIndex = 3;
                break;
            case PatternScript.Type.FOUR_TRIANGLE_2:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 4;
                this._map[0, 0] = CellType.UP;
                this._map[0, 1] = CellType.DOWN;
                this._map[0, 2] = CellType.UP;
                this._map[1, 1] = CellType.UP;
                this._bOneLine = false;
                this._nFinalIndex = 3;
                this._nIndexPositionCenter = 1;
                break;
            case PatternScript.Type.FIVE_TRIANGLE:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 5;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._map[0, 2] = CellType.DOWN;
                this._map[0, 1] = CellType.UP;
                this._bOneLine = false;
                this._nFinalIndex = 4;
                break;
            case PatternScript.Type.SPECIAL:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 6;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._map[0, 2] = CellType.DOWN;
                this._map[0, 1] = CellType.UP;
                this._map[0, 0] = CellType.DOWN;
                this._bOneLine = false;
                this._nFinalIndex = 5;
                break;
        }
        this._nWidthPattern = Constant.CELL_SIZE_WIDTH + (Constant.RANGE_X + Constant.CELL_SIZE_WIDTH / 2) * (num2 - 1);
        this._nHeightPattern = Constant.CELL_SIZE_HEIGHT * num;
        this._nHeightPattern = ((num != 1) ? (this._nHeightPattern + Constant.RANGE_Y) : this._nHeightPattern);
        base.GetComponent<RectTransform>().sizeDelta = new Vector2((float)this._nWidthPattern, (float)this._nHeightPattern);
        this.DrawMap(colorUI);
        base.GetComponent<RectTransform>().sizeDelta = new Vector2((float)(this._nWidthPattern + 50), (float)(this._nHeightPattern + 50));
        if (this._eTypePattern == PatternScript.Type.ONE_TRIANGLE || this._eTypePattern == PatternScript.Type.FOUR_TRIANGLE_2)
        {
            base.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.3f);
        }
        base.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
    }

    public void UpdateColorSpecial(List<Color> listColorUI)
    {
        int count = this._listPattern.Count;
        for (int i = 0; i < count; i++)
        {
            this._listPattern[i].GetChild(0).GetComponent<Image>().color = listColorUI[i];
        }
        base.StartCoroutine("SpecialEffect");
    }

    //[System.DebuggerHidden]
    //private IEnumerator SpecialEffect()
    //{
    //    PatternScript.< SpecialEffect > c__Iterator0 < SpecialEffect > c__Iterator = new PatternScript.< SpecialEffect > c__Iterator0();

    //    < SpecialEffect > c__Iterator.$this = this;
    //    return < SpecialEffect > c__Iterator;
    //}

    private void InitMap()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                this._map[i, j] = CellType.NONE;
            }
        }
    }

    private void DrawMap(Color colorUI)
    {
        int count = this._listPosMap.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posMap = this._listPosMap[i];
            CellType cellType = this._map[posMap.Row, posMap.Col];
            if (cellType != CellType.NONE)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PatternTriangle"));
                gameObject.transform.SetParent(base.transform);
                gameObject.transform.localScale = Vector3.one;
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PatternTriangle"));
                gameObject2.transform.SetParent(gameObject.transform);
                gameObject2.transform.localScale = new Vector2(0.85f, 0.9f);
                gameObject2.transform.localPosition = Vector2.zero;
                gameObject2.GetComponent<Image>().color = ((this._eTypePattern != PatternScript.Type.SPECIAL) ? colorUI : gameObject.GetComponent<Image>().color);
                int num = (cellType != CellType.UP) ? 180 : 0;
                gameObject.transform.localEulerAngles = new Vector3(0f, 0f, (float)num);
                float num2 = (float)((cellType != CellType.UP) ? 0 : (-(float)Constant.DELTA));
                float num3 = (float)((posMap.Col != 0) ? ((Constant.RANGE_X - 2) * posMap.Col) : 0);
                float num4 = (float)((posMap.Row != 0) ? Constant.RANGE_Y : 0);
                float y = (!this._bOneLine) ? ((float)(-(float)this._nHeightPattern / 2 + Constant.CELL_SIZE_HEIGHT / 2 + Constant.CELL_SIZE_HEIGHT * posMap.Row) + num4 + num2) : ((float)(-(float)this._nHeightPattern / 2 + Constant.CELL_SIZE_HEIGHT / 2) + num2);
                gameObject.transform.localPosition = new Vector2((float)(-(float)this._nWidthPattern / 2 + Constant.CELL_SIZE_WIDTH / 2 + Constant.CELL_SIZE_WIDTH / 2 * posMap.Col) + num3, y);
                this._listPattern.Add(gameObject.transform);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        base.StopAllCoroutines();
        this._bBeginDrag = true;
        Messenger.Broadcast<Vector2, PatternScript>(Constant.MsgBeginDrag, eventData.position, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Messenger.Broadcast<Vector2, PatternScript>(Constant.MsgDrag, eventData.position, this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (this._eColor == ColorType.SPECIAL_COLOR)
        //{
        //    base.StartCoroutine("SpecialEffect");
        //}
        this._bBeginDrag = false;
        Messenger.Broadcast<Vector2, PatternScript>(Constant.MsgEndDrag, eventData.position, this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this._bBeginDrag || !this._bAllowClick || this._eTypePattern == PatternScript.Type.SPECIAL)
        {
            return;
        }
        //SoundManager.GetInstance().PlaySelectBlock();
        this.Rotate();
    }

    private void Rotate()
    {
        this._bAllowClick = false;
        float z = base.transform.localEulerAngles.z;
        //base.transform.DORotate(new Vector3(0f, 0f, z - 60f), 0.15f, RotateMode.Fast).OnComplete(delegate
        //{
        //    this._bAllowClick = true;
        //});
        base.transform.localEulerAngles = new Vector3(0f, 0f, z - 60f);
        this._nCountClick++;

        this._bAllowClick = true;
        this._nCountClick = ((this._nCountClick <= 6) ? this._nCountClick : 1);
        this.ProcessRotate();
    }

    public void ProcessRotate()
    {
        switch (this._eTypePattern)
        {
            case PatternScript.Type.ONE_TRIANGLE:
                this.RotateOneTriangle();
                break;
            case PatternScript.Type.TWO_TRIANGLE:
                this.RotateTwoTriangle();
                break;
            case PatternScript.Type.THREE_TRIANGLE:
            case PatternScript.Type.FOUR_TRIANGLE_1:
            case PatternScript.Type.FIVE_TRIANGLE:
                this.RotateNormal();
                break;
            case PatternScript.Type.FOUR_TRIANGLE_2:
                this.RotateFourTriangle2();
                break;
        }
    }

    private void RotateFourTriangle2()
    {
        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                CellType cellType = this._map[i, j];
                this._map[i, j] = (CellType)((int)this._map[i + 1, j] * (int)CellType.DOWN);
                this._map[i + 1, j] = (CellType)((int)cellType * (int)CellType.DOWN);
            }
        }
        this._nIndexPositionCenter++;
        this._nIndexPositionCenter = ((this._nIndexPositionCenter <= this._listPosMap.Count - 1) ? this._nIndexPositionCenter : 0);
    }

    private void RotateTwoTriangle()
    {
        this.InitMap();
        this._nCountRotateTwoAngle++;
        this._nCountRotateTwoAngle = ((this._nCountRotateTwoAngle <= 3) ? this._nCountRotateTwoAngle : 1);
        int nCountRotateTwoAngle = this._nCountRotateTwoAngle;
        if (nCountRotateTwoAngle != 1)
        {
            if (nCountRotateTwoAngle != 2)
            {
                if (nCountRotateTwoAngle == 3)
                {
                    this._map[1, 0] = CellType.DOWN;
                    this._map[1, 1] = CellType.UP;
                }
            }
            else
            {
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
            }
        }
        else
        {
            this._map[1, 0] = CellType.UP;
            this._map[0, 0] = CellType.DOWN;
        }
        switch (this._nCountClick)
        {
            case 1:
                this._nIndexPositionCenter = 0;
                break;
            case 2:
                this._nIndexPositionCenter = 1;
                break;
            case 3:
                this._nIndexPositionCenter = 1;
                break;
            case 4:
                this._nIndexPositionCenter = 5;
                break;
            case 5:
                this._nIndexPositionCenter = 0;
                break;
            case 6:
                this._nIndexPositionCenter = 0;
                break;
        }
    }

    private void RotateOneTriangle()
    {
        this._map[1, 0] = (CellType)((int)this._map[1, 0] * (int)CellType.DOWN);
        this._nIndexPositionCenter = 0;
    }

    private void RotateNormal()
    {
        this._listRotate.Clear();
        this.CreateListRotate();
        int count = this._listPosMap.Count;
        int num = this._nNumberTriangle;
        int num2 = this._nFinalIndex;
        int num3 = num2 + 1 - this._nNumberTriangle;
        num3 = ((num3 >= 0) ? num3 : (count + num3));
        while (num != 0)
        {
            int num4 = num2 + 1;
            num4 = ((num4 < count) ? num4 : 0);
            this._listRotate[num4] = (CellType)((int)this._listRotate[num2] * (int)CellType.DOWN);
            num2--;
            num2 = ((num2 != -1) ? num2 : (count - 1));
            num--;
        }
        this._listRotate[num3] = CellType.NONE;
        this._nFinalIndex++;
        this._nFinalIndex = ((this._nFinalIndex < count) ? this._nFinalIndex : 0);
        this._nIndexPositionCenter++;
        this._nIndexPositionCenter = ((this._nIndexPositionCenter < count) ? this._nIndexPositionCenter : 0);
        for (int i = 0; i < count; i++)
        {
            PosMap posMap = this._listPosMap[i];
            this._map[posMap.Row, posMap.Col] = this._listRotate[i];
        }
    }

    private void LogMap()
    {
        string text = string.Empty;
        for (int i = 1; i >= 0; i--)
        {
            for (int j = 0; j < 3; j++)
            {
                text = text + this._map[i, j] + "   -   ";
            }
            text += "\n";
        }
        UnityEngine.Debug.Log(text);
    }

    public Vector2 GetPositionTriangleCenter()
    {
        return this._listPattern[0].position;
    }

    public List<CellModel> GetListMapping(int nRow, int nCol)
    {
        List<CellModel> list = new List<CellModel>(6);
        PosMap posMap = this._listPosMap[this._nIndexPositionCenter];
        int num = nRow - posMap.Row;
        int num2 = nCol - posMap.Col;
        int count = this._listPosMap.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posMap2 = this._listPosMap[i];
            int nRow2 = num + posMap2.Row;
            int nCol2 = num2 + posMap2.Col;
            CellType eType = this._map[posMap2.Row, posMap2.Col];
            CellModel item = new CellModel(eType, this._eColor, new PosMap(nRow2, nCol2));
            list.Add(item);
        }
        return list;
    }

    public void DestroyPattern()
    {
        UnityEngine.Object.Destroy(base.gameObject);
    }

    public void SaveMap()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                this._saveMap[i, j] = this._map[i, j];
            }
        }
        this._nSavePosCenter = this._nIndexPositionCenter;
        this._nSaveFinalIndex = this._nFinalIndex;
        this._nSaveCountClick = this._nCountClick;
        this._nSaveCountRotateTwoAngel = this._nCountRotateTwoAngle;
    }

    public void RestoreMap()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                this._map[i, j] = this._saveMap[i, j];
            }
        }
        this._nIndexPositionCenter = this._nSavePosCenter;
        this._nFinalIndex = this._nSaveFinalIndex;
        this._nCountClick = this._nSaveCountClick;
        this._nCountRotateTwoAngle = this._nSaveCountRotateTwoAngel;
    }

    public void UpdatePosDrag(Vector2 position)
    {
        base.transform.localPosition = new Vector2(position.x, position.y);// + (float)(this._nHeightPattern / 2) + 60f);
    }

    public SavePatterModel Export()
    {
        SavePatterModel savePatterModel = new SavePatterModel();
        CellType[,] array = new CellType[2, 3];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                array[i, j] = this._map[i, j];
            }
        }
        savePatterModel.TypePattern = this._eTypePattern;
        savePatterModel.Color = this._eColor;
        savePatterModel.IndexInPanel = this._nIndexInPanel;
        savePatterModel.CountClick = this._nCountClick;
        return savePatterModel;
    }

    public void Import(SavePatterModel savePattern, List<Color> listColorUI)
    {
        this._eTypePattern = savePattern.TypePattern;
        this._eColor = savePattern.Color;
        this._nIndexInPanel = savePattern.IndexInPanel;
        this._nCountClick = savePattern.CountClick;
        int num = 0;
        int num2 = 0;
        this._nIndexPositionCenter = 0;
        switch (this._eTypePattern)
        {
            case PatternScript.Type.ONE_TRIANGLE:
                num = 1;
                num2 = 1;
                this._nNumberTriangle = 1;
                this._map[1, 0] = CellType.UP;
                this._bOneLine = true;
                this._nFinalIndex = 0;
                break;
            case PatternScript.Type.TWO_TRIANGLE:
                num = 1;
                num2 = 2;
                this._nNumberTriangle = 2;
                this._map[1, 0] = CellType.DOWN;
                this._map[1, 1] = CellType.UP;
                this._bOneLine = true;
                this._nFinalIndex = 1;
                break;
            case PatternScript.Type.THREE_TRIANGLE:
                num = 1;
                num2 = 3;
                this._nNumberTriangle = 3;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._bOneLine = true;
                this._nFinalIndex = 2;
                break;
            case PatternScript.Type.FOUR_TRIANGLE_1:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 4;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._map[0, 2] = CellType.DOWN;
                this._bOneLine = false;
                this._nFinalIndex = 3;
                break;
            case PatternScript.Type.FOUR_TRIANGLE_2:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 4;
                this._map[0, 0] = CellType.UP;
                this._map[0, 1] = CellType.DOWN;
                this._map[0, 2] = CellType.UP;
                this._map[1, 1] = CellType.UP;
                this._bOneLine = false;
                this._nFinalIndex = 3;
                this._nIndexPositionCenter = 1;
                break;
            case PatternScript.Type.FIVE_TRIANGLE:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 5;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._map[0, 2] = CellType.DOWN;
                this._map[0, 1] = CellType.UP;
                this._bOneLine = false;
                this._nFinalIndex = 4;
                break;
            case PatternScript.Type.SPECIAL:
                num = 2;
                num2 = 3;
                this._nNumberTriangle = 6;
                this._map[1, 0] = CellType.UP;
                this._map[1, 1] = CellType.DOWN;
                this._map[1, 2] = CellType.UP;
                this._map[0, 2] = CellType.DOWN;
                this._map[0, 1] = CellType.UP;
                this._map[0, 0] = CellType.DOWN;
                this._bOneLine = false;
                this._nFinalIndex = 5;
                break;
        }
        this._nWidthPattern = Constant.CELL_SIZE_WIDTH + (Constant.RANGE_X + Constant.CELL_SIZE_WIDTH / 2) * (num2 - 1);
        this._nHeightPattern = Constant.CELL_SIZE_HEIGHT * num;
        this._nHeightPattern = ((num != 1) ? (this._nHeightPattern + Constant.RANGE_Y) : this._nHeightPattern);
        base.GetComponent<RectTransform>().sizeDelta = new Vector2((float)this._nWidthPattern, (float)this._nHeightPattern);
        Color colorUI = (this._eColor != ColorType.SPECIAL_COLOR) ? listColorUI[(int)this._eColor] : UnityEngine.Color.white;
        this.DrawMap(colorUI);
        base.GetComponent<RectTransform>().sizeDelta = new Vector2((float)(this._nWidthPattern + 50), (float)(this._nHeightPattern + 50));
        if (this._eColor == ColorType.SPECIAL_COLOR)
        {
            this.UpdateColorSpecial(listColorUI);
        }
        if (this._eTypePattern == PatternScript.Type.ONE_TRIANGLE || this._eTypePattern == PatternScript.Type.FOUR_TRIANGLE_2)
        {
            base.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.3f);
        }
        base.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
        this.RotateWithCountClick(this._nCountClick);
    }

    public void RotateWithCountClick(int nCountClick)
    {
        this._nCountClick = nCountClick;
        for (int i = 0; i < this._nCountClick; i++)
        {
            this.ProcessRotate();
        }
        base.transform.localEulerAngles = new Vector3(0f, 0f, (float)(-60 * this._nCountClick));
    }
}