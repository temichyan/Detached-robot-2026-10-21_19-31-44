using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LimbSystem : MonoBehaviour
{
    private MainControlSystem main;
    [SerializeField] private int interactDistance;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform ControlledObject;

    private void Awake()
    {
        main = GetComponent<MainControlSystem>();
    }
    public void DropLimb()
    {
        if (!main.chosedSlot) return;

        if (main.chosedSlot.GetComponentInChildren<Core>())
            return;

        Limb limb = main.chosedSlot.GetComponentInChildren<Limb>();

        if (!limb) return;

        limb.gameObject.AddComponent<Rigidbody>();
        limb.transform.SetParent(null);

        main.chosedSlot = null;
    }
    public void Interact()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            interactDistance))
        {
            Limb limb = hit.collider.GetComponentInChildren<Limb>();

            if (limb && main.chosedSlot)
            {
                Destroy(limb.GetComponent<Rigidbody>());
                limb.transform.SetParent(main.chosedSlot.transform);
                limb.transform.localPosition = new Vector3(0, 0, 0);
                limb.transform.localRotation = Quaternion.Euler(0,0,0);
                main.chosedSlot = null;
            }
        }
    }
    public void Ability()
    {
        if (main.chosedSlot && main.chosedSlot.GetComponentInChildren<Limb>())
        {
            main.chosedSlot.GetComponentInChildren<Limb>().UseAbility();
            main.chosedSlot = null;
        }
    }
}