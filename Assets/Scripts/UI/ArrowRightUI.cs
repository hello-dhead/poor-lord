using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace poorlord
{
    public class ArrowRightUI : MonoBehaviour, IUpdatable, IPointerDownHandler, IPointerUpHandler
    {
        #pragma warning disable CS0649

        private readonly int SPEED = 3;

        [SerializeField]
        private GameObject mainCamera;

        public void OnPointerDown(PointerEventData eventData)
        {
            GameManager.Instance.AddUpdate(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            GameManager.Instance.RemoveUpdate(this);
        }

        public void UpdateFrame(float dt)
        {
            int MaxDistance = TileManager.Instance.GetTileListCount() - 3;
            if (mainCamera.transform.position.x < MaxDistance)
                mainCamera.transform.Translate(Vector3.right * SPEED * dt);
        }
    }
}