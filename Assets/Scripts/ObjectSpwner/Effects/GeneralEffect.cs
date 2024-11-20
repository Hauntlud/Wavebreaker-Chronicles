using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Effects/General Effect")]
public class GeneralEffect : ScriptableObject
{
    public EffectType effectType;  // Defines the type of effect (e.g., AttackSpeed, MoveSpeed)
    public float effectValue;      // Defines the strength of the effect (e.g., 10% attack speed increase)
    public float duration;         // Duration for temporary effects, if needed
}