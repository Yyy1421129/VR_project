using UnityEngine;

public class RaycastProjectile : Projectile
{
    [SerializeField] float maxDistance = 100f;
    [SerializeField] LayerMask hitMask = Physics.DefaultRaycastLayers;

    public override void Launch()
    {
        base.Launch();

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, hitMask))
        {
            ITakeDamage[] damageTakers = hit.collider.GetComponentsInParent<ITakeDamage>();
            foreach (ITakeDamage taker in damageTakers)
            {
                taker.TakeDamage(weapon, this, hit.point);
            }
        }
    }
}
