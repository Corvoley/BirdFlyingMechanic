using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI velocity;
    [SerializeField] private TextMeshProUGUI flying;
    [SerializeField] private CharacterController character;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity.text = $"Velocity: {(int)character.Velocity}";
        flying.text = $"Flying:{(character.IsFlying ? true : false)}"; 
    }
}
