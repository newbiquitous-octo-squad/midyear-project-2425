using Alteruna;
using TMPro;
using UnityEngine;

public class NameShow : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameText;
    private string originalText;

    private void Start()
    {
        originalText = transform.parent.name;
        originalText = originalText.Substring(8);
        originalText = originalText.TrimEnd(')');
        nameText.text = originalText;
    }
}
