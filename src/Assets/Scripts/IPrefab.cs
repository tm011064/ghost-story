public interface IInstantiable<TInstantiationArgument>
  where TInstantiationArgument : AbstractInstantiationArguments
{
#if UNITY_EDITOR
  void Instantiate(TInstantiationArgument arguments);
#endif
}