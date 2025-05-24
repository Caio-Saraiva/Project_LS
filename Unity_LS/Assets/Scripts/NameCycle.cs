using UnityEngine;
using TMPro;

public class NameCycle : MonoBehaviour
{
    [Header("Display")]
    [Tooltip("TextMeshProUGUI onde a letra será exibida")]
    public TextMeshProUGUI displayText;

    private char _currentChar = 'A';

    void Start()
    {
        // Inicializa no 'A'
        CharReset();
    }

    /// <summary>
    /// Vai para a próxima letra (A→B→...→Z→A)
    /// </summary>
    public void CharUp()
    {
        if (_currentChar == 'Z')
            _currentChar = 'A';
        else
            _currentChar = (char)(_currentChar + 1);

        UpdateDisplay();
    }

    /// <summary>
    /// Vai para a letra anterior (A←B←...←Z←A)
    /// </summary>
    public void CharDown()
    {
        if (_currentChar == 'A')
            _currentChar = 'Z';
        else
            _currentChar = (char)(_currentChar - 1);

        UpdateDisplay();
    }

    /// <summary>
    /// Reseta para 'A'
    /// </summary>
    public void CharReset()
    {
        _currentChar = 'A';
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (displayText != null)
            displayText.text = _currentChar.ToString();
        else
            Debug.LogWarning("NameCycle: displayText não atribuído.");
    }
}
