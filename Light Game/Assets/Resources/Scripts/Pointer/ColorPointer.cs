using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPointer : Pointer
{
   protected override void Refract()
    {
        if (!hit.transform.GetComponent<Prism>().LinesToCombine.Contains(this))
        {
            hit.transform.GetComponent<Prism>().LinesToCombine.Add(this);
        }
    }
}
