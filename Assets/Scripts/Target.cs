using UnityEngine;

public class Target : MonoBehaviour, ITakeDamage
{
    [SerializeField] int scoreValue = 10;
    [SerializeField] float health = 1f;
    [SerializeField] bool destroyOnHit = true;
    [SerializeField] ParticleSystem hitEffect;

    public void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint)
    {
        health -= weapon.GetDamage();
        SpawnHitEffect(contactPoint, weapon.transform.position);

        if (health <= 0f)
        {
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(scoreValue);
            }

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }

    void SpawnHitEffect(Vector3 contactPoint, Vector3 weaponPosition)
    {
        if (hitEffect == null)
        {
            return;
        }

        ParticleSystem effect = Instantiate(
            hitEffect,
            contactPoint,
            Quaternion.LookRotation(weaponPosition - contactPoint));
        effect.Stop();
        effect.Play();
    }
}
