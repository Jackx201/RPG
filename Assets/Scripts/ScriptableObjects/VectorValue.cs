using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class VectorValue : ScriptableObject
{
    [Header("Valor en ejecucion")]
    public Vector2 initialValue;
    [Header("Valor inicial")]
    public Vector2 defaultValue;
    internal Vector3 value;
}
