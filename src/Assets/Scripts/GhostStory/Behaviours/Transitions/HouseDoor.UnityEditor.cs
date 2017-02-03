#if UNITY_EDITOR

using System;
using Assets.Scripts.GhostStory.Behaviours.Transitions;
using UnityEditor;
using UnityEngine;

public partial class HouseDoor : IInstantiable<PrefabInstantiationArguments>
{
  public void Instantiate(PrefabInstantiationArguments arguments)
  {
    var sceneTransitionInstantiationArguments = SceneTransitionInstantiationArguments.FromPrefabInstantiationArguments(arguments);

    transform.position = sceneTransitionInstantiationArguments.Position;

    TransitionToScene = sceneTransitionInstantiationArguments.TransitionToScene;
    TransitionToPortalName = sceneTransitionInstantiationArguments.TransitionToPortalName;
    PortalName = sceneTransitionInstantiationArguments.PortalName;

    AssignDoorObject(sceneTransitionInstantiationArguments, "Door", true);
    AssignDoorObject(sceneTransitionInstantiationArguments, "Transition Door", false);

    DeleteDoorTemplates();

    AddFadeOutBehaviour(sceneTransitionInstantiationArguments);
    AddFadeInBehaviour(sceneTransitionInstantiationArguments);
  }

  private void DeleteDoorTemplates()
  {
    var doorSprites = transform.Find("Door Sprites");
    DestroyImmediate(doorSprites.gameObject);
  }

  private void AddFadeInBehaviour(SceneTransitionInstantiationArguments arguments)
  {
    var houseDoorFadeInBehaviour = GetComponentInChildren<HouseDoorFadeInBehaviour>();
    
    houseDoorFadeInBehaviour.DoorLocation = arguments.DoorLocation;
    houseDoorFadeInBehaviour.Size = arguments.CameraBounds.size;
    houseDoorFadeInBehaviour.transform.position = arguments.CameraBounds.center;
  }

  private void AddFadeOutBehaviour(SceneTransitionInstantiationArguments arguments)
  {
    var houseDoorFadeOutBehaviour = GetComponentInChildren<HouseDoorFadeOutBehaviour>();

    var triggerEnterBehaviour = houseDoorFadeOutBehaviour.GetComponentInChildren<DoorTriggerEnterBehaviour>();
    triggerEnterBehaviour.DoorKeysNeededToEnter = new DoorKey[] { arguments.DoorKey };
    triggerEnterBehaviour.DoorLocation = arguments.DoorLocation == HorizontalDirection.Left
      ? Direction.Left
      : Direction.Right;
  }

  private GameObject CreateBoxColliderGameObject(Bounds bounds, string name = "Door Handle Trigger")
  {
    var boxColliderGameObject = new GameObject(name);

    boxColliderGameObject.transform.position = bounds.center;
    boxColliderGameObject.layer = gameObject.layer;

    var boxCollider = boxColliderGameObject.AddComponent<BoxCollider2D>();

    boxCollider.isTrigger = true;
    boxCollider.size = bounds.size;

    return boxColliderGameObject;
  }

  private void AssignDoorObject(SceneTransitionInstantiationArguments arguments, string doorName, bool isActive)
  {
    var doorObjectName = GetDoorObjectName(arguments.DoorKey, doorName);

    var doorObject = gameObject.transform.FindFirstRecursive(doorObjectName);
    if (arguments.DoorLocation == HorizontalDirection.Left)
    {
      var spriteRenderer = doorObject.GetComponent<SpriteRenderer>();
      spriteRenderer.flipX = true;
      doorObject.localPosition = doorObject.localPosition.SetX(-doorObject.localPosition.x);
    }

    doorObject.name = doorName;
    doorObject.transform.parent = transform;
    doorObject.gameObject.SetActive(isActive);
  }

  private string GetDoorObjectName(DoorKey doorKey, string doorName)
  {
    var color = GetColorName(doorKey);

    return color + " Right " + doorName;
  }

  private string GetColorName(DoorKey doorKey)
  {
    switch (doorKey)
    {
      case DoorKey.GreenHouseDoorKey: return "Green";
      case DoorKey.PurpleHouseDoorKey: return "Purple";
      case DoorKey.RedHouseDoorKey: return "Red";
    }

    throw new NotImplementedException(doorKey.ToString());
  }
}

#endif