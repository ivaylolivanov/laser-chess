using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EndPlayerTurn : MonoBehaviour {
    public static UnityAction OnEndPlayerTurnButtonPressed;

    private Button button;

    private void OnEnable() {
        button = GetComponent<Button>();

        button.onClick.AddListener(InvokeEndPlayerTurn);
    }

    private void OnDisable() {
        button.onClick.RemoveListener(InvokeEndPlayerTurn);
    }

    private void InvokeEndPlayerTurn()
        => OnEndPlayerTurnButtonPressed?.Invoke();
}
