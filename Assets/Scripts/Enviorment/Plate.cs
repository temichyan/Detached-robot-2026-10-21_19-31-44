using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    [Header("Settings")]
    public float requiredMass = 10f;

    [Header("Events")]
    public UnityEvent onActivated;
    public UnityEvent onDeactivated;

    private float currentMass = 0f;
    private bool isActive = false;

    private HashSet<Rigidbody> objectsOnPlate = new HashSet<Rigidbody>();

    void RecalculateMass()
    {
        Debug.Log("Recalculating mass...");
        currentMass = 0f;
        foreach (Rigidbody rb in objectsOnPlate)
        {
            if (rb != null)
            {
                currentMass += rb.mass;
            }
        }

        CheckActivation();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (objectsOnPlate.Add(rb))
            {
                RecalculateMass();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (objectsOnPlate.Remove(rb))
            {
                RecalculateMass();
            }
        }
    }

    void CheckActivation()
    {
        bool shouldBeActivated = currentMass >= requiredMass;

        if (shouldBeActivated != isActive)
        {
            isActive = shouldBeActivated;
            if (isActive)
            {
                Debug.Log("Plate activated");
                onActivated?.Invoke();
            }
            else
            {
                Debug.Log("Plate deactivated");
                onDeactivated?.Invoke();
            }
        }
    }
}
