using System;
using SWFServer.Data;
using SWFServer.Data.Entities;
using UnityEngine;
using VContainer;


public class UnitController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    //[SerializeField] private Text userName;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject pollution;


    private Vector3 targetPos;

    private string curAnim = String.Empty;
    public uint Id { get; private set; }
    private bool isNameUpdate = false;

    private GameObject userNameObj;
    
    private UnitSkin unitSkin;

    private UnitAvatar unit;

    public Vector2w CellPos => new Vector2w((int)(unit.Pos.x / GameConst.cellSize), (int)(unit.Pos.y / GameConst.cellSize));
    public UnitState State => unit.State;

    private bool isPlayer => unitSystem.PlayerController == this;

    private GameObject hangObj = null;
    private int handItemId = -1;

    private UIGame uiGame;
    private UnitSystem unitSystem;
    private MapSystem mapSystem;
    private GameContent content;

    [Inject]
    public void Construct(UIGame uiGame, UnitSystem unitSystem, MapSystem mapSystem, GameContent content)
    {
        this.uiGame = uiGame;
        this.unitSystem = unitSystem;
        this.mapSystem = mapSystem;
        this.content = content;
    }

    void Awake()
    {
        unitSkin = GetComponent<UnitSkin>();

       
    }

    void Start()
    {
        targetPos = transform.position;
        pollution.SetActive(false);
    }

    
    void Update()
    {
        var newPos = unit.Pos + unit.Velocity * Time.deltaTime;
        unit.Pos = newPos;

        
        targetPos = Util.ToVector3(unit.Pos);

        if (unit.State == UnitState.stand)
        {
            if ((transform.position - targetPos).sqrMagnitude > 0.001f)
            {
                Vector3 p = targetPos;
                p.y = transform.position.y;
                transform.LookAt(p);
            }

            if (unit.Velocity.sqrMagnitude > 0)
            {
                SetAnim("run");
            }
            else
            {
                SetAnim("idle");
            }
        }

        if (unit.Velocity.sqrMagnitude > 0)
        {
            UIPanel<PanelAction>.Get().Show(false);
        }

        Vector3 upOffset = Vector3.zero;


        if (unit.State == UnitState.lie)
        {
            var block = mapSystem.Map.GetBlock(CellPos);
            var rotate = block.Item1.Rotate.Value;
            if (rotate == 0 || rotate == 3)
                upOffset = Quaternion.Euler(0, rotate * 90, 0) * new Vector3(-0.5f, 0.7f, 0f);
            else
                upOffset = Quaternion.Euler(0, rotate * 90, 0) * new Vector3(-1.5f, 0.7f, 0f);


            transform.rotation = Quaternion.Euler(0, rotate * 90, 0)*Quaternion.Euler(-90, 90, 180);
            SetAnim("idle");
        }

        transform.position = Vector3.Lerp(transform.position, targetPos + upOffset, Time.deltaTime * 10f);

        if (!isNameUpdate)
        {
            string n = Data.Instance.GetUserName(unit.UserId);
            isNameUpdate = !string.IsNullOrEmpty(n);

            if (isNameUpdate)
                userNameObj = uiGame.CreateUserName(transform, n, Lang.Get("levels", unit.Level.ToString()));
        }

        //bool isPollution = unit.Pollution > 50f;
        //if (pollution.activeSelf != isPollution)
        //    pollution.SetActive(isPollution);
    }

    private void SetAnim(string anim)
    {
        if (curAnim == anim)
            return;
        curAnim = anim;
        if (animator != null)
            animator?.SetTrigger(anim);
    }

    public void Init(UnitAvatar unit)
    {
        this.unit = unit;

        //Debug.Log("unit state = " + unit.State);
        if (unit.State == UnitState.stand && isPlayer)
            UIPanel<PanelUnitAction>.Get().Show(false);
        if (unit.State == UnitState.sit)
            SetAnim("sit");
        if (unit.State == UnitState.shower)
            SetAnim("dance");
        if (unit.State == UnitState.mining || unit.State == UnitState.miningWater)
        {
            SetAnim("mining");
            Vector3 p = Util.ToVector3(SWFServer.Data.Util.ToVector2F(unit.ActionPos));
            p.y = transform.position.y;
            transform.LookAt(p);
        }
        if (unit.State == UnitState.craft)
        {
            SetAnim("mining");
            if (CellPos != unit.ActionPos)
            {
                Vector3 p = Util.ToVector3(SWFServer.Data.Util.ToVector2F(unit.ActionPos));
                p.y = transform.position.y;
                transform.LookAt(p);
            }
        }

        if (unit.itemHand == -1)
        {
            if (hangObj != null)
                Destroy(hangObj);
            handItemId = unit.itemHand;
        }
        else
        {
            if (handItemId != unit.itemHand)
            {
                if (hangObj != null)
                    Destroy(hangObj);

                handItemId = unit.itemHand;
                if (Info.EntityInfo[(ushort)handItemId].layer == EntityMapLayer.item)
                    hangObj = CreateItem((ushort)unit.itemHand, rightHand);
            }
        }
    }

    private GameObject CreateItem(ushort itemId, Transform root)
    {

        GameObject item = Instantiate(content.Block(itemId), root);
        item.transform.localPosition = Vector3.zero;

        return item;
    }

    public void SetPos(Vector2f pos, Vector2f velocity)
    {
        unit.Pos = pos;
        unit.Velocity = velocity;
    }

    private void OnDestroy()
    {
        if (userNameObj != null)
            Destroy(userNameObj);
    }

    public bool IsSleepPos()
    {
        if (Data.Instance.LocationOwner != Data.Instance.UserId)
            return false;

        var list = mapSystem.Map.GetBlocks(CellPos, 1);
        var block = list.Find(f => f != null && Info.EntityInfo[f.Id].isSleep);
        if (block == null)
            return false;

        return true;
    }

    public bool IsDoor()
    {
        var list = mapSystem.Map.GetBlocks(CellPos, 1);
        var block = list.Find(f => f != null && Info.EntityInfo[f.Id].isExit);
        if (block == null)
            return false;

        var pos = mapSystem.Map.GetBlockPos(CellPos, 1, block);

        var size = SWFServer.Data.Util.ToVector2F(Info.EntityInfo[block.Id].size) / 2;
        var pCenter = SWFServer.Data.Util.ToVector2F(pos) + size;
        var offset = SWFServer.Data.Util.ToVector2F(Info.EntityInfo[block.Id].exit) - size 
                     + new Vector2f(GameConst.cellSize, GameConst.cellSize) / 2;
        offset.Rotate(-90f * block.Rotate.Value);
        var posExit = SWFServer.Data.Util.ToVector2W(pCenter + offset);

        if (posExit.GetR(CellPos) < 2)
            return true;

        return false;
    }

    public WorkbenchType IsWorkbench(Vector2w pos)
    {
        if (CellPos.GetR(pos) > 1) return WorkbenchType.not;

        var block = mapSystem.Map.GetBlock(pos);
        return block.Item1 != null ? Info.EntityInfo[block.Item1.Id].workbenchType : WorkbenchType.not;
    }

    public Vector2w GetDoor()
    {
        for (int i = 0; i < 8; i++)
        {
            var p = CellPos + SWFServer.Data.Util.offset8[i];
            if (mapSystem.IsMapPos(p))
            {
                var cell = mapSystem.Map[p];
                if (cell != null && cell.Block != null && Info.EntityInfo[cell.Block.Id].isSwitch)
                    return p;
            }
        }

        return Vector2w.Empty;
    }

    public bool IsMove()
    {
        return unit.Velocity.sqrMagnitude > 0;
    }
}
