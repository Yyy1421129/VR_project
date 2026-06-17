using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthCoin : MonoBehaviour
{
    const string CoinNamePrefix = "SM_PolygonPrototype_Icon_Coin_01";
    const string CollectSoundResourcePath = "Audio/coincollect";

    static AudioClip sharedCollectSound;

    [SerializeField] float pickupRadius = 0.85f;
    [SerializeField] AudioClip collectSound;
    [SerializeField] float collectVolume = 1f;
    [SerializeField] bool destroyOnCollect = true;

    Player player;
    bool collected;

    void Start()
    {
        player = FindObjectOfType<Player>();
        collectSound = ResolveCollectSound();
    }

    void Update()
    {
        if (collected || player == null || player.IsDead)
        {
            return;
        }

        Vector3 playerPosition = player.GetHeadPosition();
        if (Vector3.Distance(transform.position, playerPosition) <= pickupRadius)
        {
            Collect();
        }
    }

    void Collect()
    {
        collected = true;
        player.ResetHealth();

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);
        }

        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    AudioClip ResolveCollectSound()
    {
        if (collectSound != null)
        {
            return collectSound;
        }

        if (sharedCollectSound == null)
        {
            sharedCollectSound = Resources.Load<AudioClip>(CollectSoundResourcePath);
        }

        return sharedCollectSound;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.35f);
        Gizmos.DrawSphere(transform.position, pickupRadius);
    }
}

public static class HealthCoinBootstrap
{
    const string CoinNamePrefix = "SM_PolygonPrototype_Icon_Coin_01";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachHealthCoins()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            return;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            AttachToTransform(roots[i].transform);
        }
    }

    static void AttachToTransform(Transform current)
    {
        if (current.name.StartsWith(CoinNamePrefix) && current.GetComponent<HealthCoin>() == null)
        {
            current.gameObject.AddComponent<HealthCoin>();
        }

        for (int i = 0; i < current.childCount; i++)
        {
            AttachToTransform(current.GetChild(i));
        }
    }
}
