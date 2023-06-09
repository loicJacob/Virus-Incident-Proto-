
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Serialized Objects")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private ParticleSystem shotFlashParticles;
    [SerializeField] private ParticleSystem impactParticles_Metal;
    [SerializeField] private ParticleSystem impactParticles_Wood;
    [SerializeField] private ParticleSystem impactParticles_Dirt;

    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioClip noAmoClip;

    [Header ("Settings")]
    [SerializeField] private LayerMask shootingMask;
    [SerializeField] private float precisionLoss = 0.01f;
    [SerializeField] private int maxAmoInMagazine;
    [SerializeField] private int magazineNum;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float knockBackForce = 5f;

    private Animator animator;
    private List<int> magazines = new List<int>();
    private int currentMagazinIndex = 0;
    private float fireElapsedTime = 0;

    private bool hasPlayNoAmoSound = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        for (int i = 0; i < magazineNum; i++)
            magazines.Add(maxAmoInMagazine);

        UIManager.Instance.RequestAmoUpdate += SendAmoUpdate;
    }

    private void OnEnable()
    {
        fireElapsedTime = fireRate;
        UIManager.Instance.OnUpdateAmo?.Invoke(magazines[currentMagazinIndex], maxAmoInMagazine);
        OnReleaseTrigger();
    }

    private void Update()
    {
        fireElapsedTime += Time.deltaTime;
    }

    private void SendAmoUpdate()
    {
        UIManager.Instance.OnUpdateAmo?.Invoke(magazines[currentMagazinIndex], maxAmoInMagazine);
    }

    public void Shoot()
    {
        if (fireElapsedTime > fireRate)
        {
            fireElapsedTime = 0;

            if (magazines[currentMagazinIndex] > 0)
            {
                shotFlashParticles.Play();
                // Play a trail here

                AudioSource.PlayClipAtPoint(shotClip, transform.position);

                Vector3 direction = transform.forward + new Vector3(Random.Range(-precisionLoss, precisionLoss), 0, 0);

                if (Physics.Raycast(bulletSpawnPoint.position, direction.normalized, out RaycastHit hit, float.MaxValue, shootingMask))
                {
                    if (hit.collider.CompareTag("Shotable"))
                    {
                        hit.collider.GetComponentInParent<ShotableObject>().OnHit(hit.point, hit.normal, damage, knockBackForce);
                    }
                    else
                    {
                        //Check surface type
                        //impactParticles_Metal.Play();
                        //impactParticles_Dirt.Play();
                        //impactParticles_Wood.Play();
                    }
                }

                magazines[currentMagazinIndex]--;
                UIManager.Instance.OnUpdateAmo?.Invoke(magazines[currentMagazinIndex], maxAmoInMagazine);
            }
            else
            {
                if (!hasPlayNoAmoSound)
                {
                    hasPlayNoAmoSound = true;
                    AudioSource.PlayClipAtPoint(noAmoClip, transform.position);
                }
            }
        }
    }

    public void Reload()
    {
        currentMagazinIndex++;

        if (currentMagazinIndex == magazineNum - 1)
            currentMagazinIndex = 0;

        AudioSource.PlayClipAtPoint(reloadClip, transform.position);
        UIManager.Instance.OnUpdateAmo?.Invoke(magazines[currentMagazinIndex], maxAmoInMagazine);
    }

    public void OnReleaseTrigger()
    {
        hasPlayNoAmoSound = false;
    }
}
