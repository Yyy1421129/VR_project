using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Rifle : Weapon
{
    [SerializeField] float fireRate = 3f;

    Projectile projectile;
    WaitForSeconds wait;
    Coroutine shootingCoroutine;

    protected override void Awake()
    {
        base.Awake();
        projectile = GetComponentInChildren<Projectile>();
    }

    void Start()
    {
        wait = new WaitForSeconds(1f / fireRate);
        if (projectile != null)
        {
            projectile.Init(this);
        }
    }

    protected override void StartShooting(XRBaseInteractor interactor)
    {
        base.StartShooting(interactor);
        shootingCoroutine = StartCoroutine(ShootingCO());
    }

    IEnumerator ShootingCO()
    {
        while (true)
        {
            Shoot();
            yield return wait;
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        if (projectile != null)
        {
            projectile.Launch();
        }
    }

    protected override void StopShooting(XRBaseInteractor interactor)
    {
        base.StopShooting(interactor);

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }
}
