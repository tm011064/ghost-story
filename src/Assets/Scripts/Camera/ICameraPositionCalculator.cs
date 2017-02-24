public interface ICameraPositionCalculator
{
  void Update();

  float CalculateTargetPosition();

  float GetCameraPosition();

#if UNITY_EDITOR
  void DrawGizmos(); // TODO (Roman): not nice
#endif
}
