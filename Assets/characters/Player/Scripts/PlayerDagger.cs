using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDagger : MonoBehaviour
{
  
    public float daggerDamage;
    public float daggerCombo;
    public float daggerCooldown;
    private float daggerTimer = 0;
    private float daggerCounter = 0;
    private bool daggerOverloaded = false;

    public float knockbackForce;
    public float knockbackDuration;
    public float freezeDuration;


    public GameObject aimCircle;

    public GameObject circle;
    private bool circleVisible = false;
    public List<GameObject> segments;
    private List<Segment> segments2;
    public Color defaultColor;
    public Color highlightColor;

    private Animator anim;
   

    void Start()
    {
        anim = transform.parent.GetComponent<Animator>();

        circle.SetActive(false);
        segments2 = new List<Segment>();


        foreach (var segment in segments)
        { 
            segment.AddComponent<DaggerSegment>();

            Segment s = new Segment();
            s.sprite = segment.GetComponent<SpriteRenderer>();
            s.collider = segment.GetComponent<Collider2D>();
            s.sprite.color = defaultColor;
            segments2.Add(s);
        }
    }

    void Update()
    {


        if (!daggerOverloaded)
        {
            if (Input.GetMouseButton(1))
            {
                if (!circleVisible)
                {
                    circle.SetActive(true);
                    aimCircle.SetActive(false);
                    circleVisible = true;
                }

                HighlightSegment();
            }
            else
            {
                if (circleVisible)
                {
                    circle.SetActive(false);
                    aimCircle.SetActive(true);
                    circleVisible = false;
                }
            }
        }
        else
        {
            if (daggerTimer > 0)
            {
                daggerTimer -= Time.deltaTime;
            }
            else
            {
                daggerCounter = 0;
                daggerOverloaded = false;
            }
        }
    }


    void HighlightSegment()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePosition.x - circle.transform.position.x, mousePosition.y - circle.transform.position.y);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360;   //robimy to ¿eby mieæ zakres 0-360
        }

        int segmentIndex = Mathf.FloorToInt(angle / 45.0f);     //obliczamy który segment ma siê podœwietliæ
         
        foreach (var segment in segments2)
        {
            segment.sprite.color = defaultColor;  //resetowanie pozosta³ych segmentów
            segment.collider.enabled = false;
            
        }

        segments2[segmentIndex].sprite.color = highlightColor;     //podœwietlenie odpowiedniego segmentu
        segments2[segmentIndex].collider.enabled = true;
    }


    public void TriggerEnter(Collider2D collision)
    {
        var target = ComponentHelper.GetInterfaceComponent<Enemy>(collision.gameObject);
        if (target != null && !target.IsFreezed())
        {  
            daggerCounter++;

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                anim.SetTrigger("Attack");

                if (daggerCounter != daggerCombo)
                {
                    target.Damage(daggerDamage);
                    target.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration, freezeDuration);
                }
                else
                {
                    target.Damage(daggerDamage * 2);
                    target.ApplyKnockback(knockbackDirection, knockbackForce * 3, knockbackDuration, freezeDuration);
                    daggerTimer = daggerCooldown;
                    daggerOverloaded = true;
                    aimCircle.SetActive(true);
                }
                
            }

            circle.SetActive(false);
 
        }
    }


}

[System.Serializable]
public class Segment
{
    public SpriteRenderer sprite;
    public Collider2D collider;
}
