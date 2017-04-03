using UnityEngine;

public partial class NurseryTrapdoor : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D collider)
  {
    var animator = GetComponent<Animator>();
    animator.SetTrigger("Fall Down");
  }

  void OnFallFinished()
  {
    Logger.UnityDebugLog("DOWN");
  }
}
