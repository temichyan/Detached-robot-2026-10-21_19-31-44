using UnityEngine;
using UnityEngine.InputSystem;

public class MainControlSystem : MonoBehaviour
{
    [SerializeField] Transform controlledObject;

    public ModuleSlot[] slots;
    private CoreSystem coreSystem;
    private LimbSystem limbSystem;

    private PlayerInput playerInput;
    private InputAction[] chooseSlot;

    System.Action[] LimbMethods;
    private InputAction[] Actions = new InputAction[7];

    public ModuleSlot chosedSlot;
    public ModuleSlot currentCoreSlot;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        coreSystem = GetComponent<CoreSystem>();
        limbSystem = GetComponent<LimbSystem>();

        UpdateSlots();
        SetupActions();

        LimbsConfig();
        LimbMethods = new System.Action[]
        {
            DropLimb,
            Interact,
            Ability,
            TakeCore,
            PullCore,
            DetachCore,
            CoreConnect
        };
        currentCoreSlot = GetComponentInChildren<Core>().GetComponentInParent<ModuleSlot>();
    }

    private void Update()
    {
        for (int i = 0; i < chooseSlot.Length; i++)
        {
            if (chooseSlot[i].WasPressedThisFrame())
            {
                Debug.Log("Main Control System is working!!");
                chosedSlot = slots[i];
            }
        }
        for (int i = 0;i < Actions.Length;i++)
        {
            if (Actions[i].WasPressedThisFrame())
            {
                LimbMethods[i]();
            }
        }
    }

    private void UpdateSlots()
    {
        slots = controlledObject.GetComponentsInChildren<ModuleSlot>();
    }
    
    private void SetupActions()
    {
        chooseSlot = new InputAction[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            chooseSlot[i] = playerInput.actions["Slot" + (1 + i)];
        }

        foreach (InputAction Action in playerInput.actions)
        {
            if (!char.IsDigit(Action.name[0]))
            {
                continue;
            }
            int index = int.Parse(Action.name[0].ToString());
            Actions[index] = Action;
        }
    }

    private void DropLimb()
    {
        limbSystem.DropLimb();
    }
    private void Interact()
    {
        limbSystem.Interact();
    }

    private void Ability()
    {
        limbSystem.Ability();
    }
    private void TakeCore()
    {
        coreSystem.TakeCore();
    }

    private void PullCore()
    {
        coreSystem.PullCore();
    }
    private void DetachCore()
    {
        coreSystem.DetachCore();
        UpdateSlots();
    }
    private void CoreConnect()
    {
        coreSystem.CoreConnect();
        UpdateSlots();
    }
    private void LimbsConfig()
    {
        foreach (ModuleSlot slot in slots)
        {
            if (slot.GetComponentInChildren<Rigidbody>())
            {
                Destroy(slot.GetComponentInChildren<Rigidbody>());
            }
        }
    }
}
