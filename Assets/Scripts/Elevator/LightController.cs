using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LightController : MonoBehaviour
{
    [SerializeField] private string lightNoiseSfx;
    [SerializeField] private AudioSource audioSource;
    
    
    
    private void Awake()
    {
       
    }
}
