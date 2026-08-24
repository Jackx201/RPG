using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DamageOnContact : Damage
{
    [SerializeField] private string otherString;
    [SerializeField] private int damageAmount;
    [SerializeField] private SignalSender hitSignal; // Señal opcional para vibrar pantalla o efectos
    
    [SerializeField] private CinemachineImpulseSource impulseSource; //Si está en un borde, no va funcionar, hay que subir el Damping del Cinemachine confiner en cámaras cuyas habitaciones tengan enemigos.   


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(otherString))
        {
            Health temp = other.gameObject.GetComponent<Health>();
            PlayerHealth tempPlayer = other.GetComponent<PlayerHealth>();
            bool hitSomething = false;

            if (temp)
            {
                ApplyDamage(temp, damageAmount);
                hitSomething = true;
            }
            if (tempPlayer)
            {
                ApplyDamageFloat(tempPlayer, damageAmount);
                hitSomething = true;
            }

            // Si golpeamos algo exitosamente y tenemos una señal asignada, la disparamos
            if (hitSomething && hitSignal != null)
            {
                if(impulseSource != null){
                    impulseSource.GenerateImpulse();
                }
                hitSignal.Raise();
            }
        }
    }
}
