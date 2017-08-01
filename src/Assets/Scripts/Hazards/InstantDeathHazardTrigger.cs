﻿using UnityEngine;

public partial class InstantDeathHazardTrigger : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D col)
  {
    GameManager.Instance.Player.Health.DeductAllHealth();
  }
}
