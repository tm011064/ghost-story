using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour, IObjectPoolBehaviour
{
  private GameObject[] _healthBars = new GameObject[0];

  private int _spriteWidth;

  void Start()
  {
    InitializeHealthBars();

    GameManager.Instance.Player.Health.HealthChanged += OnHealthChanged;
  }

  void OnDestroy()
  {
    GameManager.Instance.Player.Health.HealthChanged -= OnHealthChanged;
  }

  private void InitializeHealthBars()
  {
    var units = GameManager.Instance.Player.Health.HealthUnits;

    var objectPoolingManager = ObjectPoolingManager.Instance;
    for (var i = 0; i < _healthBars.Length; i++)
    {
      objectPoolingManager.Deactivate(_healthBars[i]);
    }

    _healthBars = new GameObject[units];

    UpdateHealthBars(units);
  }

  private void UpdateHealthBars(int totalFullBars)
  {
    var objectPoolingManager = ObjectPoolingManager.Instance;

    for (var i = 0; i < _healthBars.Length; i++)
    {
      var barName = i <= totalFullBars
        ? "Full"
        : "Empty";

      var position = transform.position.SetX(_spriteWidth * i).SetY(0);

      position = transform.TransformPoint(position);

      if (_healthBars[i] == null
        || _healthBars[i].activeSelf == false)
      {
        _healthBars[i] = objectPoolingManager.GetObject(barName, position);
      }
      else if (!_healthBars[i].name.StartsWith(barName, StringComparison.OrdinalIgnoreCase))
      {
        objectPoolingManager.Deactivate(_healthBars[i]);

        _healthBars[i] = objectPoolingManager.GetObject(
          barName,
          position);
      }

      _healthBars[i].transform.parent = transform;
    }
  }

  void OnHealthChanged(int healthUnits, EnemyContactReaction enemyContactReaction)
  {
    UpdateHealthBars(healthUnits);
  }

  public IEnumerable<ObjectPoolRegistrationInfo> GetObjectPoolRegistrationInfos()
  {
    //var units = GameManager.Instance.GetPlayerByName(PlayableCharacterNames.Misa.ToString()).Health.HealthUnits;
    //var units = GameManager.Instance.Player.Health.HealthUnits;
    var units = 10; // TODO (Roman): load

    var full = this.GetChildGameObject("Full");

    _spriteWidth = Mathf.RoundToInt(full.GetComponent<SpriteRenderer>().sprite.rect.width) + 1;

    yield return new ObjectPoolRegistrationInfo(full, units);

    full.SetActive(false);

    var empty = this.GetChildGameObject("Empty");

    yield return new ObjectPoolRegistrationInfo(empty, units);

    empty.SetActive(false);
  }
}