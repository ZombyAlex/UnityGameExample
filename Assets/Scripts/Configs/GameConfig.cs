using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Installers/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] public float CellSize;
    [SerializeField] public TextAsset EntitiesInfo;

    [SerializeField] public GameContent Content;


    [SerializeField] public UIMapTarget PrefabMapTarget;

}