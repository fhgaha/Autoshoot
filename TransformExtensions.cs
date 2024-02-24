using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AutoShoot
{
    public static class TransformExtensions
    {
        public static List<Transform> GetTopLevelChildren(this Transform parent)
        {
            List<Transform> cldrn = new();
            for (int i = 0; i < parent.childCount; ++i)
                cldrn.Add(parent.GetChild(i));
            return cldrn;
        }
    }
}
