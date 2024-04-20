using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface RaycastableHolder
{
    public void EnterMouseOver();
    public void ExitMouseOver();
    public int GetOwner();
}
