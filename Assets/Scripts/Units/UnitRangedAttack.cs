using UnityEngine;

public class UnitRangedAttack : MonoBehaviour {
    [Header("Balancing")]
    public MedievalCombat shootSound;
    public MedievalCombat aimSound;

    [Header("References")]
    public Arrow arrowPrefab;
    public UnitRanged unitRanged;

    public void Awake() {
        unitRanged.OnAttack.AddListener(Attack);
        unitRanged.OnAim.AddListener(Aim);
    }

    public void Aim() {
        Game.m.PlaySound(aimSound, .25f);
    }

    public void Attack() {
        Game.m.PlaySound(shootSound);
        Arrow arrow = Instantiate(arrowPrefab, 
            transform.position + new Vector3(13/36f, 21/36f), 
            Quaternion.identity, 
            transform);
        arrow.owner = unitRanged.unit;
    }
}