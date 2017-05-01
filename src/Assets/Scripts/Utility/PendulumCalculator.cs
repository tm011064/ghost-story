public class PendulumCalculator
{
  private readonly float _min;

  private readonly float _max;

  private float _speed;

  private float _value;

  public PendulumCalculator(float startValue, float min, float max, float speed = 1)
  {
    _min = min;
    _max = max;
    _speed = speed;
    _value = startValue;
  }

  public float Add(float value)
  {
    _value += value * _speed;
    if (_value > _max)
    {
      _value = _max;
      _speed *= -1;
    }
    else if (_value < _min)
    {
      _value = _min;
      _speed *= -1;
    }

    return _value;
  }
}