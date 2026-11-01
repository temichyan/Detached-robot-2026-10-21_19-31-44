using UnityEngine;

public class MagnetArm : Limb
{
    [SerializeField] private float MagnetRadius;
    [SerializeField] private float MagnetPower;
    private bool IsOn = false;
    public override void UseAbility()
    {
        IsOn = !IsOn;
    }
    private void Update()
    {
        ToMagnit();
    }
    private void ToMagnit()
    {
        if (IsOn)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, MagnetRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.GetComponentInParent<Magnitible>())
                {
                    Magnitible target = collider.GetComponentInParent<Magnitible>();
                    if (target)
                    {
                        Rigidbody targetRb = collider.GetComponentInParent<Rigidbody>();
                        if (targetRb)
                        {
                            Vector3 direction = transform.position - targetRb.position;
                            float distance = direction.magnitude;

                            if (distance > 0.1f)
                            {
                                direction.Normalize();

                                float distancePercent = distance / MagnetRadius;

                                float forceMultiplier = 1f - distancePercent;

                                float currentForce = MagnetPower * forceMultiplier;

                                targetRb.AddForce(
                                    direction * currentForce,
                                    ForceMode.Force);
                            }
                        }
                    }
                }
            }
        }
    }
}
