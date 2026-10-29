using Unity.VisualScripting;
using UnityEngine;

public class NormalArm : Limb
{
    [SerializeField] private Transform Pocket;
    [SerializeField] private float TakeDistance;
    [SerializeField] private Camera playerCamera;
    public override void UseAbility()
    {
        RaycastHit hit;
        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            TakeDistance)) 
        {
            Interactible aimedThing = hit.collider.GetComponentInChildren<Interactible>();
            if (!Pocket.GetComponentInChildren<Interactible>() && aimedThing)
            {
                aimedThing.transform.SetParent(Pocket.transform);
                aimedThing.transform.localPosition = Vector3.zero;
                aimedThing.transform.localRotation = Quaternion.Euler(0,0,0);
                Destroy(aimedThing.GetComponent<Rigidbody>());

            } else if (Pocket.GetComponentInChildren<Interactible>())
            {
                Interactible collectedThing = Pocket.GetComponentInChildren<Interactible>();
                collectedThing.transform.SetParent(null);
                collectedThing.AddComponent<Rigidbody>();
            }

        }
    }
}
