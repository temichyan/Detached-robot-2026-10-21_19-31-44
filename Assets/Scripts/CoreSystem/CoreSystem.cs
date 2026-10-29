using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoreSystem : MonoBehaviour
{
    private MainControlSystem main;
    [SerializeField] private Transform controlledObject;
    
    [SerializeField] private Core core;
    [SerializeField] Camera camera;
    private bool wantChangeObject = false;

    private void Awake()
    {
        main = GetComponent<MainControlSystem>();
    }
    private void Update()
    {

    }
    public void TakeCore()
    {
        if (main.currentCoreSlot)
        {
            wantChangeObject = true;
            Debug.Log("Taked Core");
        }
    }
    public void PullCore()
    {
        if (wantChangeObject && main.chosedSlot)
        {
            main.currentCoreSlot = main.chosedSlot;
            main.chosedSlot = null;
            wantChangeObject = false;
            core.transform.SetParent(main.currentCoreSlot.transform);
            core.transform.localPosition = Vector3.zero;
            Debug.Log("Pulled core");

        }
    }
    public void DetachCore()
    {
        controlledObject.GetChild(0).gameObject.transform.SetParent(null);
        main.currentCoreSlot.transform.SetParent(controlledObject.transform);
        controlledObject.GetComponentInChildren<Limb>().transform.localPosition = Vector3.zero;
        main.currentCoreSlot.transform.localPosition = Vector3.zero;
        main.chosedSlot = null;
        Debug.Log("Detached Core");
        transform.localPosition += new Vector3(0,0,1f);

    }
    public void CoreConnect()
    {
        RaycastHit hit;
        ControllableObject controllableObject;
        if (Physics.Raycast(
            camera.transform.position,
            camera.transform.forward,
            out hit,
            20f
            ))
        {
            SlotAnchor Anchor = hit.collider.GetComponentInChildren<SlotAnchor>();

            if (Anchor && !Anchor.GetComponentInParent<DevicePort>().GetComponentInChildren<Limb>() && !core.GetComponentInParent<ControllableObject>())
            {
                controllableObject = Anchor.GetComponentInParent<ControllableObject>();
                controllableObject.transform.SetParent(controlledObject.transform);
                main.currentCoreSlot.transform.SetParent(Anchor.transform);
                main.currentCoreSlot.transform.localPosition = Vector3.zero;
                main.currentCoreSlot.transform.localRotation = Quaternion.identity;
                controllableObject.transform.localPosition = Vector3.zero;
                controllableObject.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
