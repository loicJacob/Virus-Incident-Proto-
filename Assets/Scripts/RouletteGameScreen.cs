
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteGameScreen : Screen
{
    [SerializeField] private Button testBarrelBTN;
    [SerializeField] private Button testFireBTN;
    [SerializeField] private RectTransform gunBarrel;
    [SerializeField] private TextMeshProUGUI rewardText;

    [SerializeField] private float rollPressingMaxTime = 3;
    [SerializeField] private float rollMinAcceleration = 500;
    [SerializeField] private float rollMaxAcceleration = 2000;
    [SerializeField] private float barrelRotationFriction = 0.1f;
    [SerializeField] private float barrelRotationFrictionPerChamber = 1;

    private List<Transform> chambers = new List<Transform>();

    private bool isPressingBarrelRoll = false;
    private bool isBarrelRolling = false;

    private float rollPower = 0;
    private float barrelAngularVelocity = 0;
    private float barrelElapsedRotation = 0;
    private float barrelZRotation = 0;

    private int numChamber = 0;
    private float angleBewteenChamber = 0;

    private AudioSource clickSound;

    override protected void Awake()
    {
        base.Awake();


        numChamber = gunBarrel.childCount;
        angleBewteenChamber = 360 / numChamber;

        for (int i = 0; i < numChamber; i++)
        {
            int fixedIndex = i;
            chambers.Add(gunBarrel.GetChild(i));
            chambers[i].GetComponent<Button>().onClick.AddListener(() => AddBullet(chambers[fixedIndex]));
        }

        testBarrelBTN.onClick.AddListener(RollBarrel);
        testFireBTN.onClick.AddListener(RollBarrel);

        clickSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        animator.SetBool("IsOpen", true);
        UpdateRewardText();
    }

    private void Update()
    {
        if (isPressingBarrelRoll)
            rollPower = Mathf.Clamp(rollPower + 1 * Time.deltaTime, 0, rollPressingMaxTime);

        if (isBarrelRolling)
        {
            barrelAngularVelocity *= barrelRotationFriction;

            if (barrelZRotation >= barrelElapsedRotation)
            {
                int rest = Mathf.FloorToInt(barrelZRotation / angleBewteenChamber);

                barrelElapsedRotation = angleBewteenChamber * (rest + 1);
                barrelAngularVelocity -= barrelRotationFrictionPerChamber;

                if (barrelAngularVelocity <= barrelRotationFrictionPerChamber + barrelRotationFrictionPerChamber / 2)
                {
                    barrelAngularVelocity = 0;
                    barrelZRotation = Mathf.RoundToInt(barrelZRotation / angleBewteenChamber) * angleBewteenChamber;
                    isBarrelRolling = false;
                    animator.SetBool("IsOpen", false);
                }

                clickSound.Play();
            }

            barrelZRotation += barrelAngularVelocity * Time.deltaTime;
            gunBarrel.rotation = Quaternion.Euler(0, 0, barrelZRotation);
        }
    }

    private void RollBarrel()
    {
        isPressingBarrelRoll = !isPressingBarrelRoll;
        isBarrelRolling = !isPressingBarrelRoll;

        if (isPressingBarrelRoll)
        {
            animator.SetTrigger("CloseReloader");
            rollPower = 0;
        }
        else
        {
            barrelAngularVelocity = 0;
            barrelZRotation = gunBarrel.rotation.eulerAngles.z;
            barrelElapsedRotation = (Mathf.FloorToInt(barrelZRotation / angleBewteenChamber) + 1) * angleBewteenChamber;
            barrelAngularVelocity = Mathf.Clamp(rollPower / rollPressingMaxTime * rollMaxAcceleration, rollMinAcceleration, rollMaxAcceleration);
        }
    }

    private void AddBullet(Transform chamber)
    {
        chamber.GetChild(0).gameObject.SetActive(!chamber.GetChild(0).gameObject.activeSelf);
        UpdateRewardText();
    }

    private void UpdateRewardText()
    {
        int multiplier = 0;

        for (int i = 0; i < numChamber; i++)
        {
            if (chambers[i].GetChild(0).gameObject.activeSelf)
                multiplier++;
        }

        rewardText.text = "Reward x" + multiplier;
        rewardText.color = Color.Lerp(Color.yellow, Color.red, (float)multiplier / 5);
    }

    private void Fire()
    {

    }
}
