using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MenuScreen
{
    public void ExecuteCurrentSelection(int index);
    public int getMaxIndex();
    public float getPointerTopPosY();
}
