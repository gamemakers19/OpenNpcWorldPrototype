using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningBehavior : MonoBehaviour, IAttackable
{
    public NPC npc;
    private void Awake()
    {
        npc = GetComponent<NPC>();
    }
    public void OnAttack(GameObject attacker, Attack attack)
    {
        npc.attacked = true;
    }
}
