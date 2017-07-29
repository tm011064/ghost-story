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

  public Color UnavailableColor = new Color(.7f, .7f, .2f);

  public Color HighlightColor = new Color(1, 1, 0);

  public Color Color = new Color(1, 1, 1);

  private int _defaultFontSize;

  void Awake()
  {
    Text = GetComponent<Text>();
    RectTransform = GetComponent<RectTransform>();

    _defaultFontSize = Text.fontSize;
  }

  void Update()
  {
    if (!InventoryItem.IsAvailable)
    {
      Text.color = UnavailableColor;
    }
    else
    {
      Text.color = InventoryItem.IsActive
        ? HighlightColor
        : Color;
    }

    Text.fontStyle = Selected ? FontStyle.Bold : FontStyle.Normal;
    Text.fontSize = Selected ? _defaultFontSize + 2 : _defaultFontSize;
  }
}
