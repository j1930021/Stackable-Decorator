using StackableDecorator;
using UnityEngine;

public class ValidateObjectSample : MonoBehaviour
{
    [NotNull]
    [StackableField]
    public GameObject notNull1;

    [NotNull]
    [StackableField]
    public GameObject notNull2;

    [AssetOnly]
    [StackableField]
    public GameObject assetOnly1;

    [AssetOnly]
    [StackableField]
    public GameObject assetOnly2;

    [SceneOnly]
    [StackableField]
    public GameObject sceneOnly1;

    [SceneOnly]
    [StackableField]
    public GameObject sceneOnly2;
}