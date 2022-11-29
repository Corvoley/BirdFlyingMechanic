using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI velocity;
    [SerializeField] private TextMeshProUGUI flying;
    [SerializeField] private TextMeshProUGUI flyingUpForce;
    [SerializeField] private TextMeshProUGUI flyingForwardForce;
    [SerializeField] private CharacterController character;
     
    void Update()
    {
        velocity.text = $"Velocity: {(int)character.Velocity}";
        flying.text = $"Flying:{(character.IsFlying ? true : false)}";
        flyingUpForce.text = $"VerticalMultplierForce:{(int)character.CurrentFlyingUpwardForce}";
        flyingForwardForce.text = $"FlyingForwardForce:{(int)character.CurrentFlyingForwardForce}";
    }
}
