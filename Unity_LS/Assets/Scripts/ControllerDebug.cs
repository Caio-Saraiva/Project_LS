using UnityEngine;
using System.Collections.Generic;

public class ControllerDebug : MonoBehaviour
{
    [Header("Deadzone")]
    [Tooltip("Magnitude mínima para um eixo ser considerado ativo.")]
    public float axisDeadzone = 0.2f;

    [Header("Axes to Monitor")]
    [Tooltip("Nomes dos axes no InputManager para ler joysticks/triggers.")]
    public List<string> axisNames = new List<string>
    {
        "Horizontal",  // stick esquerdo X
        "Vertical",    // stick esquerdo Y (inverter se precisar)
        "RT",          // trigger direito (configure no InputManager)
        "LT"           // trigger esquerdo (configure no InputManager)
    };

    [Header("Buttons to Monitor")]
    [Tooltip("Todos os botões de joystick que você quer ver: 0 = A/X, 1 = B/Circle, etc.")]
    public List<KeyCode> buttonKeys = new List<KeyCode>
    {
        KeyCode.JoystickButton0,
        KeyCode.JoystickButton1,
        KeyCode.JoystickButton2,
        KeyCode.JoystickButton3,
        KeyCode.JoystickButton4,
        KeyCode.JoystickButton5,
        KeyCode.JoystickButton6,
        KeyCode.JoystickButton7,
        KeyCode.JoystickButton8,
        KeyCode.JoystickButton9,
        KeyCode.JoystickButton10,
        KeyCode.JoystickButton11,
        KeyCode.JoystickButton12,
        KeyCode.JoystickButton13,
        KeyCode.JoystickButton14,
        KeyCode.JoystickButton15,
        KeyCode.JoystickButton16,
        KeyCode.JoystickButton17,
        KeyCode.JoystickButton18,
        KeyCode.JoystickButton19
    };

    string[] joystickNames;

    void Start()
    {
        // Lista quais controles estão conectados
        joystickNames = Input.GetJoystickNames();
        for (int i = 0; i < joystickNames.Length; i++)
        {
            Debug.Log($"Joystick {i + 1}: '{joystickNames[i]}'");
        }
    }

    void Update()
    {
        // Eixos (sticks, triggers)
        foreach (var axis in axisNames)
        {
            float v = Input.GetAxisRaw(axis);
            if (Mathf.Abs(v) > axisDeadzone)
                Debug.Log($"[Axis] {axis} = {v:F2}");
        }

        // Botões
        foreach (var k in buttonKeys)
        {
            if (Input.GetKeyDown(k))
                Debug.Log($"[Button] {k} DOWN");
            if (Input.GetKeyUp(k))
                Debug.Log($"[Button] {k} UP");
        }
    }
}
