using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text displayedCode;
    private CodeFragment currentCode;

    public bool HasBeenDecoded { get; set; } = false;

    public void SetCode(CodeFragment code)
    {
        currentCode = code;
        displayedCode.text = code.fragmentDisplayText;
        displayedCode.enabled = true;
        HasBeenDecoded = false;
    }

    public string GetCodeText()
    {
        return currentCode.fragmentDisplayText;
    }

    public void ClearSlot()
    {
        displayedCode.enabled = false;
    }

    public void MarkAsDecoded()
    {
        HasBeenDecoded = true;
    }

    public CodeFragment GetCurrentCode()
    {
        return currentCode;
    }

}
