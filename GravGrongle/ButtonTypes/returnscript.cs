using System;
using UnityEngine;

namespace GravGrongle.ButtonTypes
{
    public class returnscript : MonoBehaviour
    {
        public Action OnPress;

        float touchTime;
        float debounceTime = 0.2f;

        public returnscript() => gameObject.layer = 18;

        public void OnTriggerEnter(Collider collider)
        {
            if (!enabled || !(touchTime + debounceTime < Time.time) || collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
            {
                return;
            }

            touchTime = Time.time;

            OnPress.Invoke();

            GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();

            if (!(component == null))
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);

                GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
            }

            GameObject grongle = GameObject.Find("gronglemodel(Clone)");

            GameObject gorillaPlayer = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player");

            if (gorillaPlayer != null && grongle != null)
            {
                Vector3 gorillaPlayerPosition = gorillaPlayer.transform.position;

                grongle.transform.position = gorillaPlayerPosition;
            }
            else
            {
                Debug.LogError("Failed to find either gorillaPlayer or grongle(Clone). Make sure the GameObjects exist in the scene with the specified names.");
            }
        }
    }
}
