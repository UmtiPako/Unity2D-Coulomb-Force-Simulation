using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.InputSystem;
using TMPro;

public class ChargeValues : MonoBehaviour
{
    public int chargeID { get; set; }

    public float charge;

    public SpriteRenderer SR;

    public ChargeManager _chargeManager;

    public GameObject arrow;
    public TMP_Text chargeText; 

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        arrow = transform.GetChild(0).gameObject;
        chargeText = transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>();
        addChargeToList();
    }

    private void Update()
    {
        chargeText.text = charge + " µC";
    }
    public void addChargeToList()
    {
        chargeID = setChargeID();
        _chargeManager.charges.Add(this);
    }

    public int setChargeID()
    {
        int id = _chargeManager.charges
            .Select(c => c.GetComponent<ChargeValues>().chargeID)
            .DefaultIfEmpty(0).Max();
        return id + 1;
    }

    int clicked = 0;
    float clickTime = 0;
    float clickDelay = 0.5f;

    private void OnMouseDown()
    {
        if (this.clicked > 1 && Time.time - this.clickTime < clickDelay)
        {
            _chargeManager.openPanel(this);
            this.clickTime = 0;
            this.clicked = 0;
        }
        else
            this.clickTime = Time.time;
            this.clicked++;
    }

    private void OnMouseDrag()
    {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint
                (Input.mousePosition) - transform.position;
            transform.Translate(new Vector3(
                mousePos.x,
                mousePos.y,
                transform.position.z));
            transform.position = new Vector3(transform.position.x,
                transform.position.y, transform.position.z);
    }

}
