using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using System.Runtime.InteropServices;
public class ChargeManager : MonoBehaviour
{
    [HideInInspector] public List<ChargeValues> charges = new List<ChargeValues>();

    [SerializeField] private GameObject chargePrefab;
    [SerializeField] private ChargePanel chargePanel;
    [SerializeField] private Canvas worldCanvas;
    public TMP_Text forceText;

    public bool electricForcePanel = true;
    public void SpawnCharge()
    {
        var newCharge = Instantiate(chargePrefab);
        var script = newCharge.GetComponent<ChargeValues>();

        newCharge.transform.position = new Vector3(
            newCharge.transform.position.x, UnityEngine.Random.Range(1f, 5f),
            newCharge.transform.position.z);
        script._chargeManager = this;
    }

    public void ClearScreen()
    {
        if (charges.Count > 0)
        {
            foreach (ChargeValues charge in charges)
            {
                Destroy(charge.gameObject);
            }
            charges.Clear();
            if (chargePanel.isActiveAndEnabled) chargePanel.
                                               gameObject.SetActive(false);
        }
    }

    #region Panel Stuff
    public void openPanel(ChargeValues charge)
    {
        if (!chargePanel.isActiveAndEnabled)
        {
            chargePanel.gameObject.SetActive(true);
            charge.SR.color = Color.yellow;
            chargePanel.activeCharge = charge;
            chargePanel.labelText.text = $"charge of c{charge.chargeID}";
        }
    }

    public void closePanel()
    {
        chargePanel.chargeInput.text = string.Empty;
        chargePanel.activeCharge.SR.color = Color.white;
        chargePanel.activeCharge.arrow.transform.rotation = Quaternion.Euler(0,0,90);
        chargePanel.activeCharge.arrow.SetActive(false);
        chargePanel.activeCharge = null;
        chargePanel.gameObject.SetActive(false);
        electricForcePanel = false;
        forceText.gameObject.SetActive(false);
    }

    public void simulateForce()
    {
        electricForcePanel = true;
        chargePanel.activeCharge.arrow.SetActive(true);
        forceText.gameObject.SetActive(true);
    }

    public void submitChargeValue()
    {
        string input = (!String.IsNullOrEmpty(chargePanel.chargeInput.text))
            ? chargePanel.chargeInputText.text : "0";
        input = RemoveZeroWidthSpaces(input).Trim();

        if (float.TryParse(input,
            NumberStyles.Float, CultureInfo.InvariantCulture,
            out float parsedCharge))
        {
            chargePanel.activeCharge.charge = parsedCharge;
            chargePanel.chargeInput.text = "";
        }
        else
        {
            Debug.LogError("Geçersiz sayý formatý!");
        }
    }

    public void deleteCharge()
    {
        charges.RemoveAt(charges.IndexOf
            (chargePanel.activeCharge));
        Destroy(chargePanel.activeCharge.gameObject);
        closePanel();
    }

    #endregion

    #region Distance Lines

    [Header("Lines Between Charges")]
    public Transform objectA;
    public Transform objectB;
    public TMP_Text textPrefab; // Mesafeyi gösterecek text prefabý
    private LineRenderer lineRenderer;
    private TMP_Text distanceText;

    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); 

        if (textPrefab != null)
        {
            distanceText = Instantiate(textPrefab, transform);
            distanceText.transform.SetParent(worldCanvas.transform);
        }
    }

    void Update()
    {
        if (objectA != null && objectB != null)
        {
            // Çizgi uç noktalarýný belirle
            lineRenderer.SetPosition(0, objectA.position);
            lineRenderer.SetPosition(1, objectB.position);

            // Mesafeyi hesapla
            float distance = Vector3.Distance(objectA.position, objectB.position);

            // Çizgi kalýnlýðýný mesafeye baðlý olarak artýr
            float thickness = Mathf.Clamp(distance * 0.005f, 0.002f, 0.02f); // Min: 0.02, Max: 0.2
            lineRenderer.startWidth = thickness;
            lineRenderer.endWidth = thickness;

            // Ortadaki noktayý hesapla ve mesafeyi göster
            Vector3 midPoint = (objectA.position + objectB.position) / 2;

            if (distanceText != null)
            {
                distanceText.transform.position = midPoint;
                distanceText.text = distance.ToString("F2") + "m";
            }
        }
    }

    #endregion

        #region Utility
    public static string RemoveZeroWidthSpaces(string input)
    {
        return new string(input.Where(c => c != '\u200B' && !char.IsControl(c)).ToArray());
    }

    #endregion
}
