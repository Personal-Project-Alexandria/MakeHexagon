using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public enum CellType
{
    NONE,
    UP,
    DOWN = -1
}
[System.Serializable]
public enum ColorType
{
    NONE = -1,
    COLOR_1,
    COLOR_2,
    COLOR_3,
    COLOR_4,
    COLOR_5,
    COLOR_6,
    SPECIAL_COLOR
}
[System.Serializable]
public struct PosMap
{
    public int Row;

    public int Col;

    public PosMap(int nRow, int nCol)
    {
        this.Row = nRow;
        this.Col = nCol;
    }
}


public class HexagonGame : MonoBehaviour {

	public int _Row;

    public int _Col;

    public int _startX;

    public int _startY;

    public List<Color> _listColorUI;

    private List<ColorType> _listColorUse = new List<ColorType>(6);

    private List<ColorType> _listSumColor = new List<ColorType>(6);

    private CellModel[,] _mapData;

    private TriangelScript[,] _mapUI;

    public AlgorithmCheckHexa _algorithmCheckHexa;

    private int _nChoseRow;

    private int _nChoseCol;

    private List<CellModel> _listMapping = new List<CellModel>();

    private List<CellModel> _listHint = new List<CellModel>();

    public Transform _specialPatternManager;

    private bool _bAllowAddPattern;

    private bool _bCreateSpecialPattern;

    private bool _bActiveHammer;

    private int _nScore;

    private bool _bCanGetDouble;
    public void Awake()
    {
        this.InitMapData();
        this.InitMapUI();
        this.FillMapData();
        this.DrawMap();
        this.InitLogicListColor();
    }
    public void Start()
    {
        Messenger.AddListener<Vector2, PatternScript>(Constant.MsgBeginDrag, new Callback<Vector2, PatternScript>(this.OnPatternBeginDrag));
        Messenger.AddListener<Vector2, PatternScript>(Constant.MsgDrag, new Callback<Vector2, PatternScript>(this.OnPatternDrag));
        Messenger.AddListener<Vector2, PatternScript>(Constant.MsgEndDrag, new Callback<Vector2, PatternScript>(this.OnPatternEndDrag));
        this.testCreate();
    }
    private void InitMapData()
    {
        this._mapData = new CellModel[this._Row, this._Col];
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                this._mapData[i, j] = new CellModel(CellType.NONE, ColorType.NONE, new PosMap(i, j));
            }
        }
    }
    private void InitMapUI()
    {
        this._mapUI = new TriangelScript[this._Row, this._Col];
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                this._mapUI[i, j] = null;
            }
        }
    }
    private void FillMapData()
    {
        int num = this._Row / 2 - 1;
        for (int i = this._Row / 2 - 1; i >= 0; i--)
        {
            int num2 = num - i;
            int num3 = this._Col - 1 - num2;
            CellType cellType = CellType.DOWN;
            for (int j = num2; j <= num3; j++)
            {
                this._mapData[i, j].Type = cellType;
                //cellType *= CellType.DOWN;
                cellType = (CellType)((int)cellType * (int)CellType.DOWN);
            }
        }
        int num4 = 0;
        for (int k = this._Row / 2; k < this._Row; k++)
        {
            int num5 = k - 1 - num4 * 2;
            num4++;
            for (int l = 0; l < this._Col; l++)
            {
                this._mapData[k, l].Type = (CellType)((int)(this._mapData[num5, l].Type) * (int)CellType.DOWN);
            }
        }
    }

    private void DrawMap()
    {
        int num = 0;
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                CellType type = this._mapData[i, j].Type;
                if (type != CellType.NONE)
                {
                    GameObject gameObject = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Triangle"));
                    gameObject.transform.SetParent(this.transform);// this._layerTriangle);
                    gameObject.transform.localScale = Vector3.one;
                    gameObject.name = num.ToString();
                    this.SetPositionTriangle(gameObject.transform, i, j, type);
                    gameObject.GetComponent<TriangelScript>().SetData(i, j, type);
                    this._mapUI[i, j] = gameObject.GetComponent<TriangelScript>();
                    num++;
                }
            }
        }
    }
    private void SetPositionTriangle(Transform triangel, int nRow, int nCol, CellType _cellType)
    {
        float num = (float)((_cellType != CellType.UP) ? 0 : (-(float)Constant.DELTA));
        int num2 = (nCol != 0) ? (Constant.RANGE_X * nCol) : 0;
        int num3 = (nRow != 0) ? (Constant.RANGE_Y * nRow) : 0;
        triangel.localPosition = new Vector2((float)(this._startX + Constant.CELL_SIZE_HEIGHT / 2 + num2 + Constant.CELL_SIZE_HEIGHT / 2 * nCol), (float)(this._startY + Constant.CELL_SIZE_HEIGHT / 2 + num3 + Constant.CELL_SIZE_HEIGHT * nRow) + num);
    }
    private void InitLogicListColor()
    {
        this._listSumColor.Clear();
        this._listColorUse.Clear();
        for (int i = 0; i < 6; i++)
        {
            ColorType item = (ColorType)i;
            this._listSumColor.Add(item);
        }
    }
    [ContextMenu("test")]
    void testCreate()
    {
        CreatePattern(PatternScript.Type.FIVE_TRIANGLE, 1, ColorType.COLOR_1, Color.blue, false);
    }

    private GameObject CreatePattern(PatternScript.Type eType, int nIndex, ColorType eColor, Color colorUI, bool bIsSpecial)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Pattern"));
        gameObject.transform.SetParent(this.transform); // this._panelPattern);
        gameObject.transform.localScale = Vector3.one;
        if (bIsSpecial)
        {
            gameObject.transform.position = this._specialPatternManager.transform.position;
        }
        else
        {
            gameObject.transform.localPosition = new Vector2(0f, 0f);
        }
        gameObject.GetComponent<PatternScript>().CreatePattern(nIndex, eType, eColor, colorUI);
        return gameObject;
    }
    private void OnPatternBeginDrag(Vector2 position, PatternScript pattern)
    {
        if (this._bActiveHammer)
        {
            return;
        }
        this._nChoseRow = -1;
        this._nChoseCol = -1;
        this._bAllowAddPattern = false;
        this._bCreateSpecialPattern = false;
        //SoundManager.GetInstance().PlaySelectBlock();
    }

    void FixedUpdate()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    PointerEventData pointer = new PointerEventData(EventSystem.current);
        //    pointer.position = Input.mousePosition;
        //    List<RaycastResult> raycastResults = new List<RaycastResult>();
        //    EventSystem.current.RaycastAll(pointer, raycastResults);
            
        //    TriangelScript t = getTriangel(raycastResults);
        //    if ( t != null)
        //    {
        //        Debug.Log("Get Triangel = " + t.gameObject.name);
        //    }
        //}

    }
    private TriangelScript getTriangel(List<RaycastResult> raycastResults)
    {
        foreach(RaycastResult result in raycastResults)
        {
            if(result.gameObject.GetComponent<TriangelScript>() != null)
            {
                return result.gameObject.GetComponent<TriangelScript>();
            }
        }
        return null;
    }
    private void OnPatternDrag(Vector2 position, PatternScript pattern)
    {
        if (this._bActiveHammer)
        {
            return;
        }
        
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y));

        Vector2 position2 = pattern.transform.parent.InverseTransformPoint(pos);
        pattern.UpdatePosDrag(position2);

        Vector2 p = pattern.GetPositionTriangleCenter();

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count != 0)
        {
            Debug.LogError("Length = " + raycastResults.Count);
            TriangelScript component = getTriangel(raycastResults);// array[0].collider.GetComponent<TriangelScript>();
            if (component != null)
            {
                int row = component.Row;
                int col = component.Col;
                if (this._nChoseRow == row && this._nChoseCol == col)
                {
                    return;
                }
                this._bAllowAddPattern = false;
                this._nChoseRow = row;
                this._nChoseCol = col;
                this._listMapping = pattern.GetListMapping(this._nChoseRow, this._nChoseCol);
                this.DebugLisMapping(this._listMapping);
                this._bAllowAddPattern = this.CheckAllowAddPattern(this._listMapping, pattern.Color);
                if (this._bAllowAddPattern)
                {
                    this.ClearHint();
                    this.GetListHint(this._listMapping);
                    this.ShowHint(pattern.Color);
                }
                else
                {
                    this.ClearHint();
                }
            }
        }
        else
        {
            this._nChoseRow = -1;
            this._nChoseCol = -1;
            this.ClearHint();
        }
    }

    private void OnPatternEndDrag(Vector2 position, PatternScript pattern)
    {
        if (this._bActiveHammer)
        {
            return;
        }
        position = pattern.GetPositionTriangleCenter();
        RaycastHit[] array = Physics.RaycastAll(new Vector3(position.x, position.y, -1f), Vector3.forward, 1000f, LayerMask.GetMask(new string[]
        {
            "Triangle"
        }));

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        this.ClearHint();
        if (raycastResults.Count != 0)
        {
            TriangelScript component = getTriangel(raycastResults);// array[0].collider.GetComponent<TriangelScript>();
            if (component != null)
            {
                this._nChoseRow = component.Row;
                this._nChoseCol = component.Col;
                this._listMapping = pattern.GetListMapping(this._nChoseRow, this._nChoseCol);
                this.DebugLisMapping(this._listMapping);
                this._bAllowAddPattern = this.CheckAllowAddPattern(this._listMapping, pattern.Color);
                if (this._bAllowAddPattern)
                {
                    this.GetListHint(this._listMapping);
                    this.ShowPatternTriangle(pattern.Color, this._listHint);
                    List<PosMap> list = this._algorithmCheckHexa.CheckHexa(this._listHint, this._mapData, pattern.TypePattern);
                    int num = this.CaculateScore(pattern.NumberTriangle, list.Count, pattern.Color, ref this._bCanGetDouble);
                    if (this._bCanGetDouble)
                    {
                        //this._bCreateSpecialPattern = this._specialPatternManager.CanCreateSpecial();
                    }
                    //this.PlayEffectGetScore(num, pattern.transform.position, pattern.Color);
                    //base.StartCoroutine("ClearPatternTriangle", list);
                   // this.CheckTransferColor();
                    pattern.DestroyPattern();
                    //base.StartCoroutine("DelayCheckOutOfMove");
                    //Messenger.Broadcast<bool>(Constant.MsgCanUseHammer, this.CanUseHammer());
                    this.testCreate();
                    //this.AddNewPattern(pattern.IndexInPanel, this._bCreateSpecialPattern);
                    //if (num >= 60)
                    //{
                    //    SoundManager.GetInstance().PlayAfterScore();
                    //}
                }
                else
                {
                    //this.ResetPosPattern(pattern);
                }
            }
        }
        else
        {
            //this.ResetPosPattern(pattern);
        }
    }
    private void AddNewPattern(int nIndex, bool bIsSpecial = false)
    {
        //this._listPattern[nIndex] = null;
        //if (!bIsSpecial)
        //{
        //    this.ArrangePattern();
        //}
        nIndex = ((!bIsSpecial) ? 2 : nIndex);
        GameObject gameObject = this.InitDataPattern(nIndex, bIsSpecial);
        //this._listPattern[nIndex] = gameObject.GetComponent<PatternScript>();
        if (bIsSpecial)
        {
            gameObject.GetComponent<PatternScript>().UpdateColorSpecial(this._listColorUI);
            //gameObject.transform.DORotate(new Vector3(0f, 0f, -720f), 1f, RotateMode.FastBeyond360);
            //Sequence s = DOTween.Sequence();
            //s.Append(gameObject.transform.DOScale(new Vector3(1.3f, 1.3f, 1f), 0.5f));
            //s.Append(gameObject.transform.DOScale(Vector3.one, 0.5f));
            //float y = gameObject.transform.localPosition.y;
            //gameObject.transform.DOLocalMoveY(y + 70f, 0.3f, false);
            //gameObject.transform.DOMove(this._listPosition[nIndex].position, 1f, false).SetDelay(0.3f);
            //SoundManager.GetInstance().PlayGetScore();
            //base.StartCoroutine("PreventTouch", 1.3f);
        }
        else
        {
            //SoundManager.GetInstance().PlayAddBlock();
            int nCountClick = UnityEngine.Random.Range(0, 6);
            //this._listPattern[2].RotateWithCountClick(nCountClick);
        }
        if (!bIsSpecial)
        {
            //this.PlayEffectRunPattern();
        }
    }
    private GameObject InitDataPattern(int nIndex, bool bIsSpecial)
    {
        ColorType colorType = (!bIsSpecial) ? this.GetRandomColor() : ColorType.SPECIAL_COLOR;
        PatternScript.Type eType = (PatternScript.Type)((!bIsSpecial) ? UnityEngine.Random.Range(0, 6) : 6);
        Color colorUI = (!bIsSpecial) ? this._listColorUI[(int)colorType] : Color.white;
        GameObject gameObject = this.CreatePattern(eType, nIndex, colorType, colorUI, bIsSpecial);
        //gameObject.name = "Pattern " + this._nCountPattern;
        //this._nCountPattern++;
        return gameObject;
    }
    private ColorType GetRandomColor()
    {
        int index = UnityEngine.Random.Range(0, this._listColorUse.Count);
        return this._listColorUse[index];
    }
    private void DebugLisMapping(List<CellModel> listMapping)
    {
        int count = listMapping.Count;
        string text = string.Empty;
        string text2 = string.Empty;
        for (int i = 0; i < count / 2; i++)
        {
            text = text + listMapping[i].Type + " - ";
        }
        text += "\n";
        for (int j = count - 1; j >= count / 2; j--)
        {
            text = text + listMapping[j].Type + " - ";
        }
        for (int k = 0; k < count / 2; k++)
        {
            PosMap posInMap = listMapping[k].PosInMap;
            string text3 = text2;
            text2 = string.Concat(new object[]
            {
                text3,
                "( ",
                posInMap.Row,
                " , ",
                posInMap.Col,
                " )  - "
            });
        }
        text2 += "\n";
        for (int l = count - 1; l >= count / 2; l--)
        {
            PosMap posInMap2 = listMapping[l].PosInMap;
            string text3 = text2;
            text2 = string.Concat(new object[]
            {
                text3,
                "( ",
                posInMap2.Row,
                " , ",
                posInMap2.Col,
                " )  - "
            });
        }
    }

    private bool CheckAllowAddPattern(List<CellModel> listMapping, ColorType eColorType)
    {
        int count = listMapping.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = listMapping[i].PosInMap;
            CellType type = listMapping[i].Type;
            int row = posInMap.Row;
            int col = posInMap.Col;
            if (type != CellType.NONE)
            {
                if (row < 0 || col < 0 || row >= this._Row || col >= this._Col)
                {
                    return false;
                }
                if (eColorType != ColorType.SPECIAL_COLOR)
                {
                    if (this._mapData[row, col].Color != ColorType.NONE)
                    {
                        return false;
                    }
                    if (type != this._mapData[row, col].Type)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private void ShowPatternTriangle(ColorType eColorType, List<CellModel> listHint)
    {
        int count = listHint.Count;
        if (count == 0)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = listHint[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            if (eColorType != ColorType.SPECIAL_COLOR)
            {
                this._mapUI[row, col].DisplayPatternTriangle(this._listColorUI[(int)eColorType]);
            }
            else
            {
                this._mapUI[row, col].ClearPatternTriangle();
            }
            this._mapData[row, col].Color = ((eColorType != ColorType.SPECIAL_COLOR) ? eColorType : ColorType.NONE);
        }
    }

    private void ClearHint()
    {
        int count = this._listHint.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = this._listHint[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            this._mapUI[row, col].ClearHint();
        }
        this._listHint.Clear();
    }
    private void GetListHint(List<CellModel> listMapping)
    {
        this._listHint.Clear();
        int count = listMapping.Count;
        if (count == 0)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = listMapping[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            CellType type = listMapping[i].Type;
            if (row >= 0 && col >= 0)
            {
                if (type != CellType.NONE)
                {
                    if (type == this._mapData[row, col].Type)
                    {
                        this._listHint.Add(listMapping[i]);
                    }
                }
            }
        }
    }

    private void ShowHint(ColorType eColorType)
    {
        int count = this._listHint.Count;
        if (count == 0)
        {
            return;
        }
        Color colorUI = (eColorType != ColorType.SPECIAL_COLOR) ? this._listColorUI[(int)eColorType] : Color.white;
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = this._listHint[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            this._mapUI[row, col].DisplayHint(colorUI);
        }
    }

    private int CaculateScore(int nNumberPatternTriangle, int nCountListCheckHexa, ColorType ePatternColor, ref bool bCanGetDouble)
    {
        int num = 0;
        int nScore = this._nScore;
        if (ePatternColor == ColorType.SPECIAL_COLOR)
        {
            this._nScore += 60;
            num += 60;
            bCanGetDouble = false;
        }
        else
        {
            this._nScore += nNumberPatternTriangle;
            num = nNumberPatternTriangle;
            bCanGetDouble = false;
            if (nCountListCheckHexa == 6)
            {
                this._nScore += 60;
                num = 60;
            }
            if (nCountListCheckHexa >= 10)
            {
                this._nScore += 240;
                num = 240;
                bCanGetDouble = true;
            }
        }
        //Messenger.Broadcast<int, int, int>(Constant.MsgNotifyScore, nScore, num, this._nScore);
        return num;
    }
    
}
