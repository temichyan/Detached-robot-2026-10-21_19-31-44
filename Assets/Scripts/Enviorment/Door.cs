using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    public List<Plate> requiredPlates = new List<Plate>();

    [Header("Movement Parameters")]
    public Vector3 openRotationAngle = new Vector3(0, 90f, 0);
    public float rotationSpeed = 3f;

    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private Dictionary<Plate, bool> platesState = new Dictionary<Plate, bool>();

    void Start()
    {
        closedRotation = transform.localRotation;
        targetRotation = closedRotation;

        foreach (var plate in requiredPlates)
        {
            platesState[plate] = false;

            plate.onActivated.AddListener(() => SetPlateState(plate, true));
            plate.onDeactivated.AddListener(() => SetPlateState(plate, false));
        }
    }
    void Update()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void SetPlateState(Plate plate, bool state)
    {
        if (platesState.ContainsKey(plate))
        {
            platesState[plate] = state;
            EvaluateDoorState();
        }
    }

    void EvaluateDoorState()
    {
        bool allActive = true;
        foreach (var state in platesState.Values)
        {
            if (!state)
            {
                allActive = false;
                break;
            }
        }

        if (allActive && requiredPlates.Count > 0)
        {
            targetRotation = closedRotation * Quaternion.Euler(openRotationAngle);
            Debug.Log("Door is opening.");
        }
        else
        {
            targetRotation = closedRotation;
            Debug.Log("Door is closing.");
        }
    }
}
