using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
[System.Serializable]
public class FloatValue : ScriptableObject, ISerializationCallbackReceiver
{
    public float initialValue;
    public float RuntimeValue;
    internal float value;

    public void OnAfterDeserialize()
    {
        RuntimeValue = initialValue;
    }

    public void OnBeforeSerialize() { }
}
