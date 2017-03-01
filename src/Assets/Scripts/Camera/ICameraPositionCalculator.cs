public partial interface ICameraPositionCalculator
{
  SmoothDampedPositionCalculator SmoothDampedPositionCalculator { get; }

  float WindowPosition { get; }

  void Update();

  float CalculateTargetPosition();

  float GetCameraPosition();
}