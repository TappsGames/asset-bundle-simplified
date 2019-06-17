using System;
using AssetBundleSimplified.AssetReference;
using UnityEngine;

[System.Serializable]
public class AnimationReference : AssetReference<AnimationClip>{};

[Serializable]
public class MonoBehaviorRef : AssetReference<MonoBehaviour>
{
};

[CreateAssetMenu(menuName = "AAA/ReferenceTest")]

public class ReferenceTest : ScriptableObject
{
    /*[SerializeField]
    private SpriteReference spriteReference;

    [SerializeField]
    private GameObjectReference gameObjectReference;*/

    [SerializeField] private AnimationReference animationReference;
    [SerializeField] private MonoBehaviorRef monoBehaviorRef;
}