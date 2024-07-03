using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowUI : MonoBehaviour
{
    public void Disable()
    {
        OBPool.Instance.UIdespawn(this.gameObject,true);
    }
}
