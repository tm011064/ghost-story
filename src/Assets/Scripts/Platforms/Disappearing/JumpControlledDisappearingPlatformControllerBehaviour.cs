using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public partial class JumpControlledDisappearingPlatformControllerBehaviour : MonoBehaviour
{
  public int MaxVisiblePlatforms = 2;

  private IDictionary<GameObject, Node> _nodesByTimerPlatformGameObject;

  private Node _activePlayerNode;

  private Node _first;

  private Node _last;

  private PlayerController _playerController;

  public virtual void Awake()
  {
    _playerController = GameManager.Instance.Player;

    _nodesByTimerPlatformGameObject = BuildNodes().ToDictionary(n => n.Platform.gameObject);

    Assert.IsTrue(MaxVisiblePlatforms > 0);
    Assert.IsTrue(_nodesByTimerPlatformGameObject.Count() > MaxVisiblePlatforms);

    _first = _nodesByTimerPlatformGameObject.First().Value.FindFirst();
    _last = _first.FindLast();
  }

  private IEnumerable<Node> BuildNodes()
  {
    Node previous = null;

    var orderedPlatforms = GetComponentsInChildren<TimerPlatform>()
      .OrderBy(platform => platform.Index);

    foreach (TimerPlatform platform in orderedPlatforms)
    {
      var node = new Node(platform, previous);

      if (previous != null)
      {
        previous.Next = node;
      }

      previous = node;

      yield return node;
    };
  }

  public virtual void Start()
  {
    ResetNodes();
  }

  private void ResetNodes()
  {
    _activePlayerNode = null;

    DisableAllPlatforms();
    ActivateStartNodes();
  }

  public virtual void OnEnable()
  {
    _playerController = GameManager.Instance.Player;
    if (_playerController == null)
    {
      return; // TODO (Roman): why do we need this?
    }

    _playerController.GroundedPlatformChanged += OnGroundedPlatformChanged;
  }

  public virtual void OnDisable()
  {
    if (_playerController == null)
    {
      return; // TODO (Roman): why do we need this?
    }

    _playerController.GroundedPlatformChanged -= OnGroundedPlatformChanged;
  }

  private void ActivateStartNodes()
  {
    _nodesByTimerPlatformGameObject.Values
      .Where(n => n.IsFirst())
      .First()
      .Traverse(MaxVisiblePlatforms)
      .ForEach(node => node.ActivateGameObject());
  }

  private Node GetGroundedNode(GroundedPlatformArgs groundedPlatformArgs)
  {
    var parent = groundedPlatformArgs.CurrentPlatform.GetParentGameObject();
    if (parent == null)
    {
      return null;
    }

    Node node;
    if (!_nodesByTimerPlatformGameObject.TryGetValue(parent, out node))
    {
      return null;
    }

    return node;
  }

  private void OnGroundedPlatformChanged(GroundedPlatformArgs groundedPlatformArgs)
  {
    if (groundedPlatformArgs.CurrentPlatform == null)
    {
      _activePlayerNode = null;
      return;
    }

    var node = GetGroundedNode(groundedPlatformArgs);
    if (node == null)
    {
      if (_last.IsActive())
      {
        ResetNodes();
      }

      return;
    }

    _activePlayerNode = node;

    if (node.IsLast())
    {
      return;
    }

    if (!node.IsNextActive())
    {
      var firstActiveNode = node.FindFirstActive();

      firstActiveNode.DeactivateGameObject();
      node.Next.ActivateGameObject();
    }
  }

  private void DisableAllPlatforms()
  {
    _nodesByTimerPlatformGameObject.Keys.ForEach(o => o.SetActive(false));
  }

  private class Node
  {
    public Node(
      TimerPlatform platform,
      Node previous)
    {
      Platform = platform;
      Previous = previous;
    }

    public TimerPlatform Platform;

    public Node Next;

    public Node Previous;

    public bool IsGrounded;

    public bool IsFirst()
    {
      return Previous == null;
    }

    public bool IsLast()
    {
      return Next == null;
    }

    public bool IsActive()
    {
      return Platform.gameObject.activeSelf;
    }

    public bool IsNextActive()
    {
      return Next != null && Next.IsActive();
    }

    public bool IsPreviousActive()
    {
      return Previous != null && Previous.IsActive();
    }

    public void ActivateGameObject()
    {
      Platform.gameObject.SetActive(true);
    }

    public void DeactivateGameObject()
    {
      Platform.gameObject.SetActive(false);
    }

    public Node FindFirstActive()
    {
      var node = this;
      while (node.IsPreviousActive())
      {
        node = node.Previous;
      }
      return node;
    }

    public Node FindFirst()
    {
      var node = this;
      while (!node.IsFirst())
      {
        node = node.Previous;
      }
      return node;
    }

    public Node FindLast()
    {
      var node = this;
      while (!node.IsLast())
      {
        node = node.Next;
      }
      return node;
    }

    public IEnumerable<Node> Traverse(int count)
    {
      var node = this;
      for (var i = 0; i < count; i++)
      {
        if (node == null)
        {
          yield break;
        }

        yield return node;

        node = node.Next;
      }
    }
  }
}
