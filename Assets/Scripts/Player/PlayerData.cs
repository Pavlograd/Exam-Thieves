using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerData", menuName = "CustomUbisoft/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string nameCharacters = "Nerd";
    public string descCharacters = "Desc Nerd";
    public float staminaRegenSpeed = 0.3f;
    public float staminaBurnSpeed = 1f;
    public string nameObjAnimation = "teddy";
    public RuntimeAnimatorController _animator;
    public GameObject skinPlayer;
    public Image playerImage;
    public CharacterType type;
    public Sprite powerImage;
}
