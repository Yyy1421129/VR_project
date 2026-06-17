using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour
{
    const string GunShotResourcePath = "Audio/GUN_FIRE-GoodSoundForYou-820112263";

    [SerializeField] protected float shootingForce;
    [SerializeField] protected Transform bulletSpawn;
    [SerializeField] float recoilForce;
    [SerializeField] float damage;
    [SerializeField] AudioClip gunShotClip;

    Rigidbody rigidBody;
    XRGrabInteractable interactableWeapon;
    AudioSource audioSource;

    protected virtual void Awake()
    {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        SetupInteractableWeaponEvents();
    }

    protected virtual void Start()
    {
        ApplyGunShotClip();
    }

    void ApplyGunShotClip()
    {
        if (audioSource == null)
        {
            return;
        }

        AudioClip clip = gunShotClip != null ? gunShotClip : audioSource.clip;
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>(GunShotResourcePath);
        }

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    void SetupInteractableWeaponEvents()
    {
        interactableWeapon.onSelectEntered.AddListener(PickUpWeapon);
        interactableWeapon.onSelectExited.AddListener(DropWeapon);
        interactableWeapon.onActivate.AddListener(StartShooting);
        interactableWeapon.onDeactivate.AddListener(StopShooting);
    }

    void PickUpWeapon(XRBaseInteractor interactor)
    {
        var hider = interactor.GetComponent<MeshHidder>();
        if (hider != null)
        {
            hider.Hide();
        }
    }

    void DropWeapon(XRBaseInteractor interactor)
    {
        var hider = interactor.GetComponent<MeshHidder>();
        if (hider != null)
        {
            hider.Show();
        }
    }

    protected virtual void StartShooting(XRBaseInteractor interactor)
    {
    }

    protected virtual void StopShooting(XRBaseInteractor interactor)
    {
    }

    protected virtual void Shoot()
    {
        ApplyRecoil();
        PlayShotSound();
    }

    void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce()
    {
        return shootingForce;
    }

    public float GetDamage()
    {
        return damage;
    }

    void PlayShotSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
