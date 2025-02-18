﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used for coins, health, inventory items, and even ammo if you want to create a gun shooting mechanic!*/

public class Collectable : MonoBehaviour
{

    public enum ItemType { InventoryItem, Coin, Health, Ammo, bed, dragon, enemy, coffee_bean, ddl, diode1, diode2, cup, boxing, FPGA }; //Creates an ItemType category
    [SerializeField] ItemType itemType; //Allows us to select what type of item the gameObject is in the inspector
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip[] collectSounds;
    [SerializeField] private int itemAmount;
    [SerializeField] private string itemName; //If an inventory item, what is its name?
    [SerializeField] private Sprite UIImage; //What image will be displayed if we collect an inventory item?
    public Vector3 distanceFromPlayer;
    public Vector3 speed;
    public Vector3 speedEased;
    public float easing = 20;
    public float speedMultiplier = 10;
    public bool flyToPlayer; //Fly to player after .5 seconds
    private Rigidbody2D rigidbody;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody = transform.parent.GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == NewPlayer.Instance.gameObject)
        {
            Collect();
        }

        //Collect me if I trigger with an object tagged "Death Zone", aka an area the player can fall to certain death
        if (col.gameObject.layer == 14)
        {
            Collect();
        }
    }

    public void Update()
    {
        StartCoroutine(FlyToPlayer());
    }

    public void Collect()
    {
        if (itemType == ItemType.InventoryItem)
        {
            if (itemName != "")
            {
                GameManager.Instance.GetInventoryItem(itemName, UIImage);
            }
        }
        else if (itemType == ItemType.Coin)
        {
            NewPlayer.Instance.coins += itemAmount;
            if(NewPlayer.Instance.coins > NewPlayer.Instance.max_coins)
            {
                NewPlayer.Instance.coins = NewPlayer.Instance.max_coins;
            }
        }
        else if (itemType == ItemType.Health)
        {
            if (NewPlayer.Instance.health < NewPlayer.Instance.maxHealth)
            {
                GameManager.Instance.hud.HealthBarHurt();
                NewPlayer.Instance.health += itemAmount;
            }
        }
        else if (itemType == ItemType.Ammo)
        {
            if (NewPlayer.Instance.ammo < NewPlayer.Instance.maxAmmo)
            {
                GameManager.Instance.hud.HealthBarHurt();
                NewPlayer.Instance.ammo += itemAmount;
            }
        }
        else if (itemType == ItemType.bed)
        {
            if (!NewPlayer.Instance.super_armor)
            {
                GameManager.Instance.hud.HealthBarHurt();
                NewPlayer.Instance.health -= itemAmount;
                if(NewPlayer.Instance.coins > 0)
                {
                    NewPlayer.Instance.coins -= 1;
                }
            }
        }
        else if (itemType == ItemType.dragon)
        {   
            if (!NewPlayer.Instance.super_armor)
            {
                GameManager.Instance.hud.HealthBarHurt();
                NewPlayer.Instance.health -= itemAmount*3;
            }
        }
        else if (itemType == ItemType.enemy)
        {
            if (!NewPlayer.Instance.super_armor)
            {
                GameManager.Instance.hud.HealthBarHurt();
                NewPlayer.Instance.health -= itemAmount*2;
            }
        }
        else if (itemType == ItemType.coffee_bean)
        {
            NewPlayer.Instance.coins += itemAmount*2;
            if(NewPlayer.Instance.coins > NewPlayer.Instance.max_coins)
            {
                NewPlayer.Instance.coins = NewPlayer.Instance.max_coins;
            }
        }
        else if (itemType == ItemType.ddl)
        {
            NewPlayer.Instance.coins += itemAmount*3;
            if(NewPlayer.Instance.coins > NewPlayer.Instance.max_coins)
            {
                NewPlayer.Instance.coins = NewPlayer.Instance.max_coins;
            }
        }
        else if (itemType == ItemType.diode1)
        {
            NewPlayer.Instance.runRightSpeed = 2f;
            NewPlayer.Instance.StopEffect(ItemType.diode1, 3f);
        }
        else if (itemType == ItemType.diode2)
        {
            NewPlayer.Instance.runRightSpeed = 0.5f;
            NewPlayer.Instance.StopEffect(ItemType.diode2, 3f);
        }
        else if (itemType == ItemType.cup)
        {
            if (NewPlayer.Instance.health < NewPlayer.Instance.maxHealth)
            {
                GameManager.Instance.hud.HealthBarHurt();
                NewPlayer.Instance.health += itemAmount;
            }
            NewPlayer.Instance.runRightSpeed = 1.5f;
            NewPlayer.Instance.StopEffect(ItemType.cup, 5f);
        }
        else if (itemType == ItemType.boxing)
        {
            NewPlayer.Instance.super_armor = true;
            NewPlayer.Instance.StopEffect(ItemType.boxing, 5f);
            //todo:无敌效果图
        }
        else if (itemType == ItemType.FPGA)
        {
            NewPlayer.Instance.runRightSpeed = 0;
            NewPlayer.Instance.StopEffect(ItemType.FPGA, 2f);
        }

        //If my parent has an Ejector script, it means that my parent is actually what needs to be destroyed, along with me, once collected
        if (transform.parent.GetComponent<Ejector>() != null)
        {
            //Destroy(transform.parent.gameObject);
            transform.parent.gameObject.SetActive(false);
        }
        else
        {   
            //Destroy(gameObject);
            transform.gameObject.SetActive(false);
        }
        
        GameManager.Instance.audioSource.PlayOneShot(collectSounds[Random.Range(0, collectSounds.Length)], Random.Range(.6f, 1f));

        NewPlayer.Instance.FlashEffect();

    }

    private IEnumerator FlyToPlayer()
    {
        if (flyToPlayer)
        {
            yield return new WaitForSeconds(1);
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            distanceFromPlayer.x = (NewPlayer.Instance.transform.position.x) - transform.parent.transform.position.x;
            distanceFromPlayer.y = (NewPlayer.Instance.transform.position.y + 1) - transform.parent.transform.position.y;
            speed.x = (Mathf.Abs(distanceFromPlayer.x) / distanceFromPlayer.x) * speedMultiplier;
            speed.y = (Mathf.Abs(distanceFromPlayer.y) / distanceFromPlayer.y) * speedMultiplier;
            speedEased += (speed - speedEased) * Time.deltaTime * easing;
            transform.parent.transform.position += speedEased * Time.deltaTime;
            Debug.Log("flyToPlayer!");
        }
    }
}
