using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SignalListener : MonoBehaviour
{
    public SignalSender SignalSender;
    public UnityEvent signalEvent;

  public void OnSignalRaised()
  {
      signalEvent.Invoke();
  }

  private void OnEnable()
  {
      SignalSender.RegisterListener(this);
  }

  private void OnDisable()
  {
      SignalSender.DeRegisterListener(this);
  }
}
