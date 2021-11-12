using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public abstract class Skill : MonoBehaviour
    {
        public abstract void ExecuteSkill(Unit caster, Unit target, int damage);
    }
}