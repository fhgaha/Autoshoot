using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AutoShoot
{
    public static class TransformExtensions
    {
        public static IEnumerable<Transform> GetTopLevelChildren(this Transform parent)
        {
            foreach (Transform c in parent)
                yield return c;
        }
    }
}
