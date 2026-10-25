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
    [SerializeField] private int interactDistance;
    [SerializeField] private Camera playerCamera;
    private Limb aimedLimb;

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
            if (slotActions[i].WasPressedThisFrame())
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
        if (chosedSlot)
        {
            chosedSlot.GetComponentInChildren<Limb>().AddComponent<Rigidbody>();
            chosedSlot.GetComponentInChildren<Limb>().transform.SetParent(null);
            chosedSlot = null;
        }
    }
    private void Interact()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            interactDistance))
        {
            Limb limb = hit.collider.GetComponentInChildren<Limb>();

            if (limb && chosedSlot)
            {
                Destroy(limb.GetComponent<Rigidbody>());
                limb.transform.SetParent(chosedSlot.transform);
                limb.transform.localPosition = new Vector3(0, 0, 0);
                limb.transform.localRotation = Quaternion.Euler(0,0,0);
                chosedSlot = null;
            }
        }
    }
    private void Ability()
    {
        if (chosedSlot.GetComponentInChildren<Limb>())
        {
            chosedSlot.GetComponentInChildren<Limb>().UseAbility();
        }
        chosedSlot = null;
    }
}