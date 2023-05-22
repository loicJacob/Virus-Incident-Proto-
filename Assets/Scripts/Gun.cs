
using System;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject fireParticles;
    [SerializeField] private Transform gunOutput;
    [SerializeField] private int maxAmoInMagazine;
    [SerializeField] private int magazineNum;
    [SerializeField] private float fireRate = 0.1f;

    private List<int> magazines = new List<int>();
    private int currentMagazinIndex = 0;
    private float fireElapsedTime = 0;

    private void Awake()
    {
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
            fireElapsedTime = 0;
            if (magazines[currentMagazinIndex] > 0)
            {
                Instantiate(fireParticles, gunOutput).GetComponent<ParticleSystem>().Play();
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
}
