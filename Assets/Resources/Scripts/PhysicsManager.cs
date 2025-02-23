using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    const double k = 8.99 / 1000; // due to  µC

    [SerializeField] private ChargeManager _chargeManager;
    [SerializeField] private ChargePanel _chargePanel;

    private void FixedUpdate()
    {
        if (_chargeManager.electricForcePanel)
        {
            Calculate_ElectricForce(_chargePanel.activeCharge);
        }
    }

    public void Calculate_ElectricForce(ChargeValues charge)
    {
        /* double forceOnCharge = _chargeManager.charges.Where(
            c => c.chargeID != charge.chargeID).
            Select(s => electricForceFormula(charge,s)).Sum(); --> outdated */ 

        Vector2 forceDirection = vectorSum(_chargeManager.charges.Where(
            c => c.chargeID != charge.chargeID).
            Select(s => findVector(charge, s)));

        double forceOnCharge = forceDirection.magnitude;

        charge.arrow.SetActive(true);
        float angle = Mathf.Atan2(forceDirection.y, forceDirection.x) * Mathf.Rad2Deg;
        charge.arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        string formattedForce = forceOnCharge.ToString("F3");
        charge._chargeManager.forceText.text = formattedForce + " µF";

    }

    private static Vector2 findVector(ChargeValues q1, ChargeValues q2)
    {
        return (float)(k * q1.charge * q2.charge
            / Mathf.Pow(Vector2.Distance(q1.transform.position, q2.transform.position), 2))
            * (q1.transform.position - q2.transform.position).normalized; ;
    }

    private static Vector2 vectorSum(IEnumerable<Vector2> vectors)
    {
        Vector2 vectorSum = new Vector2(0,0);
        foreach (var vector in vectors)
        {
            vectorSum += vector;
        }
        return vectorSum;
        // return vectorSum.magnitude > 0 ? vectorSum.normalized : Vector2.zero;
    }
}
