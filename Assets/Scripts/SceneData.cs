using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    [Required] public Camera MainCamera;
    [Required] public Transform RootMapTarget;
    [Required] public GameObject GameCursor;
    [Required] public Transform unitsParentTransform;
    public GameObject uiMapTargetPrefab;
    public Transform uiParentTransform;
}
