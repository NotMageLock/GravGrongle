using System;
using UnityEngine;

namespace GravGrongle.ButtonTypes
{
    public class returnToButton : MonoBehaviour
    {
        public Action OnPress;

        float touchTime;
        float debounceTime = 0.2f;

        public returnToButton() => gameObject.layer = 18;

        public void OnTriggerEnter(Collider collider)
        {
            if (!enabled || !(touchTime + debounceTime < Time.time) || collider == null || collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
            {
                return;
            }

            touchTime = Time.time;

            OnPress?.Invoke();

            GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();

            if (component != null)
            {
                GorillaTagger.Instance?.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);

                GorillaTagger.Instance?.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
            }

            GameObject grongleButton = GameObject.Find("Grongle Button");
            GameObject grongle = GameObject.Find("gronglemodel(Clone)");
            if (grongle != null)
            {
                grongle.transform.position = grongleButton.transform.position;
                GrongleNetworking.SendTeleportEvent(grongleButton.transform.position);
            }

            else
            {
                Debug.LogWarning("Grongle model not found.");
            }
        }
    }
}
