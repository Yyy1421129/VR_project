using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsDamage : MonoBehaviour, ITakeDamage
{
    [SerializeField] int scoreValue;
    [SerializeField] float forceMultiplier = 1f;

    Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint)
    {
        rigidBody.AddForce(projectile.transform.forward * weapon.GetShootingForce() * forceMultiplier, ForceMode.Impulse);

        if (scoreValue > 0 && ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
        }
    }
}
