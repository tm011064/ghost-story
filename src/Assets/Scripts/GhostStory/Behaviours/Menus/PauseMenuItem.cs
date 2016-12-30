using UnityEngine;
using UnityEngine.UI;

public class PauseMenuItem : MonoBehaviour
{
  public Text Text;

  public RectTransform RectTransform;

  [HideInInspector]
  public InventoryItem InventoryItem;

  [HideInInspector]
  public bool Selected;

  public Color HighlightColor = new Color(1, 1, 0);

  public Color Color = new Color(1, 1, 1);

  void Awake()
  {
    Text = GetComponent<Text>();
    RectTransform = GetComponent<RectTransform>();
  }

  void Update()
  {
    Text.color = InventoryItem.IsActive
      ? HighlightColor
      : Color;

    Text.fontStyle = Selected ? FontStyle.Bold : FontStyle.Normal;
    Text.fontSize = Selected ? 24 : 20;
  }
}
