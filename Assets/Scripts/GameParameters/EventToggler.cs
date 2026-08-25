using UnityEngine;

public class EventToggler : MonoBehaviour
{
    public enum ComparisonMode
    {
        Equals,
        NotEquals
    }

    [Header("Settings")]
    [SerializeField] private BoolValue scriptableObjectValue;
    [SerializeField] private bool desiredValue;
    [SerializeField] private ComparisonMode comparisonMode;

    private void Awake()
    {
        EvaluateState();
    }

    public void EvaluateState()
    {
        if (scriptableObjectValue == null)
        {
            Debug.LogWarning($"[{nameof(EventToggler)}] Falta asignar el ScriptableObject 'scriptableObjectValue' en {gameObject.name}.", this);
            return;
        }

        bool isValueMatching = scriptableObjectValue.value == desiredValue;

        bool shouldBeActive = comparisonMode switch
        {
            ComparisonMode.Equals => isValueMatching,
            ComparisonMode.NotEquals => !isValueMatching,
            _ => false
        };

        gameObject.SetActive(shouldBeActive);
        Debug.Log($"[{nameof(EventToggler)}] Estado actualizado en {gameObject.name}: {(shouldBeActive ? "Activo" : "Inactivo")}");
    }
}