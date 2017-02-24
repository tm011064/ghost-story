public partial interface ICameraPositionCalculator
{
  void Update();

  float CalculateTargetPosition();

  float GetCameraPosition();
}
