using UnityEngine;

namespace GravGrongle
{
    public class ButtonOTL : MonoBehaviour
    {
        public Vector3 pos;

        public static ButtonOTL Instance;
        public void Start()
        {
            Vector3 newPosition = new Vector3(-65.3796f, 11.9504f, -81.414f);
            transform.position = newPosition;

            Vector3 newRotation = new Vector3(270f, 149.4319f, 0f);
            transform.eulerAngles = newRotation;

            Vector3 newScale = new Vector3(0.1f, 0.1f, 0.1f);
            transform.localScale = newScale;
        }
    }
}