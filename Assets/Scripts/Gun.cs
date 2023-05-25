
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Serialized Objects")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem impactParticles_Metal;
    [SerializeField] private ParticleSystem impactParticles_Wood;
    [SerializeField] private ParticleSystem impactParticles_Dirt;

    [Header ("Settings")]
    [SerializeField] private LayerMask shootingMask;
    [SerializeField] private float precisionLoss = 0.01f;
    [SerializeField] private int maxAmoInMagazine;
    [SerializeField] private int magazineNum;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float damage = 5f;

    private Animator animator;
    private List<int> magazines = new List<int>();
    private int currentMagazinIndex = 0;
    private float fireElapsedTime = 0;

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
            if (magazines[currentMagazinIndex] > 0)
            {
                fireElapsedTime = 0;
                fireParticles.Play();

                Vector3 direction = GetDirection();

                if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, shootingMask))
                {
                    if (hit.collider.CompareTag("Shotable"))
                    {
                        hit.collider.GetComponentInParent<ShotableObject>().OnHit(hit.point, hit.normal, damage);
                    }
                    else
                    {
                        //Check surface type
                        //impactParticles_Metal.Play();
                        //impactParticles_Dirt.Play();
                        //impactParticles_Wood.Play();
                    }

                    //TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
                    //StartCoroutine(SpawnTrail(trail, hit));
                }

                magazines[currentMagazinIndex]--;
                UIManager.Instance.OnUpdateAmo?.Invoke(magazines[currentMagazinIndex], maxAmoInMagazine);
            }
        }
    }

    public void Reload()
    {
        currentMagazinIndex++;

        if (currentMagazinIndex == magazineNum - 1)
            currentMagazinIndex = 0;

        UIManager.Instance.OnUpdateAmo?.Invoke(magazines[currentMagazinIndex], maxAmoInMagazine);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward + new Vector3(Random.Range(-precisionLoss, precisionLoss), 0, 0);
        return direction.normalized;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        Destroy(trail.gameObject, trail.time);
    }
}
