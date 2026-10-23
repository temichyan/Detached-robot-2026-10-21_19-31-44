using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LimbSystem : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction[] slotActions = new InputAction[4];
    private InputAction[] Actions = new InputAction[3];
    [SerializeField] private LimbSlot[] LimbSlots;
    System.Action[] LimbMethods;
    private LimbSlot chosedSlot;

    private void Awake()
    {
        LimbMethods = new System.Action[]
        {
            DropLimb,
            Interact,
            Ability
        };

        SetupActions();
    }
    private void Update()
    {
        for (int i = 0;i < slotActions.Length;i++)
        {
            if (slotActions[i].WasPressedThisFrame() && LimbSlots[i].GetComponentInChildren<Limb>())
            {
                chosedSlot = LimbSlots[i];
                Debug.Log(LimbSlots[i].name);
            }
        }
        for (int j = 0; j < Actions.Length;j++)
        {
            if (Actions[j].WasPressedThisFrame())
            {
                Debug.Log(Actions[j].name);
                LimbMethods[j]?.Invoke();
            }
        }
    }
    private void SetupActions ()
    {
        playerInput = GetComponentInParent<PlayerInput>();

        for (int i = 0; i < slotActions.Length; i++)
        {
            slotActions[i] = playerInput.actions["Slot" + (i + 1)];
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
        chosedSlot.GetComponentInChildren<Limb>().AddComponent<Rigidbody>();
        chosedSlot.GetComponentInChildren<Limb>().transform.SetParent(null);
    }
    private void Interact()
    {
        Debug.Log("Конечность одета");

    }
    private void Ability()
    {
        Debug.Log("Конечность использована");

    }
}