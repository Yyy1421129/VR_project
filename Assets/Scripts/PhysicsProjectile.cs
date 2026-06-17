using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsProjectile : Projectile
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] string[] ignoreTags = { "Player" };

    Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void Init(Weapon weapon)
    {
        base.Init(weapon);
        Destroy(gameObject, lifeTime);
    }

    public override void Launch()
    {
        base.Launch();
        rigidBody.AddRelativeForce(Vector3.forward * weapon.GetShootingForce(), ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        if (ShouldIgnore(other))
        {
            return;
        }

        ITakeDamage[] damageTakers = other.GetComponentsInParent<ITakeDamage>();
        Vector3 contactPoint = transform.position;

        foreach (ITakeDamage taker in damageTakers)
        {
            taker.TakeDamage(weapon, this, contactPoint);
        }

        Destroy(gameObject);
    }

    bool ShouldIgnore(Collider other)
    {
        if (ignoreTags == null)
        {
            return false;
        }

        foreach (string tag in ignoreTags)
        {
            if (!string.IsNullOrEmpty(tag) && other.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
    }
}
