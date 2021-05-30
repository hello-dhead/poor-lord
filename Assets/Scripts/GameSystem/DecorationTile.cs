using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    /// <summary>
    /// 장식용 타일
    /// </summary>
    public class DecorationTile : MonoBehaviour
    {
        private Transform tileTransform;

        public void init(Vector3 position)
        {
            if (tileTransform == null)
            {
                tileTransform = this.gameObject.transform;
            }
            tileTransform.position = position;
        }
    }
}