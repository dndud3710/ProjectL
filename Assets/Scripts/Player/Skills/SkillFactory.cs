using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactory 
{
    public static Dictionary<KeyCode,SkillInstance> CreateSkill(Class @class)
    {
        Dictionary<KeyCode, SkillInstance>  skills = new Dictionary<KeyCode, SkillInstance>();

        switch (@class)
        {
            case Class.Warrior:
                //skills.Add(KeyCode.Q, )
                break;
            case Class.Gunner:

                break;
            case Class.Mage:

                break;
        }

        return skills;
    }
}
