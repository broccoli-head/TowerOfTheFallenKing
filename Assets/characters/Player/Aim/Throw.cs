using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public float force;
    public static bool CanThrow = true;
    float offset;
    GameObject potion;
    GameObject Aim;
    Rigidbody2D rb;
    Vector2 direction;
    BeamPotionEffect BPE;
    Inventory inventory;

    void Start()
    {
        Aim = FindObjectOfType<Aim>().gameObject;
        inventory = FindObjectOfType<Inventory>();
        offset = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>().bounds.size.y * 0.8f;
    }

    void Update()
    {
        //Przesuwa pozycje z ktorej wyrzucana jest potka tak zeby nie wchodzila w kolizje z graczem
        transform.localPosition = new Vector3(offset,0,0);

        if(inventory.SelectedPotion != null) {


            potion = inventory.SelectedPotion.potion == null ?
                inventory.SelectedPotion.potionEffect.gameObject : inventory.SelectedPotion.potion.gameObject;

            if (Input.GetKeyDown(KeyCode.Mouse0) && CanThrow && Time.timeScale > 0)
            {
                GameObject newPotion = Instantiate(potion, transform.position, Quaternion.identity);
                newPotion.GetComponent<Collider2D>().excludeLayers = 6;
                direction = (Aim.transform.position - transform.parent.position).normalized;
                if (newPotion.TryGetComponent<PotionObject>(out PotionObject potionObj)) 
                {
                    rb = newPotion.AddComponent<Rigidbody2D>();
                    potionObj.direction = direction;
                    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                    rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                    rb.AddForce(direction * force, ForceMode2D.Impulse); 
                }
                else if(newPotion.TryGetComponent<BeamPotionEffect>(out BPE))
                {
                    BPE.direction = direction;
                }    
                newPotion.transform.parent = null;
                inventory.RemovePlayerItem(inventory.SelectedPotion.Name);
                inventory.ValidatePotions();
            }
        }
    }
}