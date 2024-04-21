using System;
using System.Reflection;
using UnityEngine;

namespace GravGrongle.ButtonTypes
{
    public class spawn : MonoBehaviour
    {
        public GameObject grongle;
        public Action OnPress;

        float touchTime;
        float debounceTime = 0.2f;

        public spawn() => gameObject.layer = 18;

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
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GravGrongle.Resources.gronglebundle");
            var bundle = AssetBundle.LoadFromStream(fileStream);
            var resource = bundle.LoadAsset("gronglemodel") as GameObject;

            grongle = Instantiate(resource);
            grongle.AddComponent<OTL>();
            grongle.SetLayer(UnityLayer.Prop);
            grongle.AddComponent<DevHoldable>();

            MeshCollider meshCollider = grongle.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = grongle.GetComponent<MeshFilter>().mesh;
            meshCollider.convex = true;

            Rigidbody rb = grongle.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            rb.useGravity = true;

            Debug.Log("Grongle collider size: " + meshCollider.bounds.size);
            Debug.Log("Grongle rigidbody mass: " + rb.mass);
        }
    }
}
