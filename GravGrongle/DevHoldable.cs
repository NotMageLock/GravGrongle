using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

namespace GravGrongle
{
    public class DevHoldable : HoldableObject
    {
        public bool
            InHand = false,
            InLeftHand = false,
            PickUp = true,
            DidSwap = false,
            SwappedLeft = true;

        private Vector3 previousLeftControllerPosition;
        private Vector3 previousRightControllerPosition;

        public float
            GrabDistance = 0.15f,
            ThrowForce = 5f;

        public virtual void OnGrab(bool isLeft)
        {
            GameObject grongle = GameObject.Find("gronglemodel(Clone)");

            if (grongle != null)
            {
                Rigidbody rigidbody = grongle.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = true;
                }
                else
                {
                    Debug.LogWarning("Grongle Rigidbody component not found.");
                }
            }
            else
            {
                Debug.LogError("Grongle not found.");
            }
        }

        public virtual void OnDrop(bool isLeft)
        {
            GameObject grongle = GameObject.Find("gronglemodel(Clone)");

            if (grongle != null)
            {
                Rigidbody rigidbody = grongle.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = false;

                    Vector3 throwDirection = isLeft ?
                        (Player.Instance.leftControllerTransform.position - previousLeftControllerPosition) :
                        (Player.Instance.rightControllerTransform.position - previousRightControllerPosition);

                    rigidbody.velocity = throwDirection / Time.deltaTime * ThrowForce;
                }
                else
                {
                    Debug.LogWarning("Grongle Rigidbody component not found.");
                }
            }
            else
            {
                Debug.LogError("Grongle not found.");
            }
        }

        public void Update()
        {
            Vector3 currentLeftControllerPosition = Player.Instance.leftControllerTransform.position;
            Vector3 currentRightControllerPosition = Player.Instance.rightControllerTransform.position;

            float left = ControllerInputPoller.instance.leftControllerGripFloat;
            bool leftGrip = left >= 0.5f;

            float right = ControllerInputPoller.instance.rightControllerGripFloat;
            bool rightGrip = right >= 0.5f;

            var Distance = GrabDistance * Player.Instance.scale;
            if (DidSwap && (!SwappedLeft ? !leftGrip : !rightGrip))
                DidSwap = false;

            bool pickLeft = PickUp && leftGrip && Vector3.Distance(currentLeftControllerPosition, transform.position) < Distance && !InHand && EquipmentInteractor.instance.leftHandHeldEquipment == null && !DidSwap;
            bool swapLeft = InHand && leftGrip && rightGrip && !DidSwap && (Vector3.Distance(currentLeftControllerPosition, transform.position) < Distance) && !SwappedLeft && EquipmentInteractor.instance.leftHandHeldEquipment == null;
            if (pickLeft || swapLeft)
            {
                DidSwap = swapLeft;
                SwappedLeft = true;
                InLeftHand = true;
                InHand = true;

                transform.SetParent(GorillaTagger.Instance.offlineVRRig.leftHandTransform.parent);
                GorillaTagger.Instance.StartVibration(true, 0.1f, 0.05f);
                EquipmentInteractor.instance.leftHandHeldEquipment = this;
                if (DidSwap) EquipmentInteractor.instance.rightHandHeldEquipment = null;

                OnGrab(true);
            }
            else if (!leftGrip && InHand && InLeftHand)
            {
                InLeftHand = true;
                InHand = false;
                transform.SetParent(null);

                EquipmentInteractor.instance.leftHandHeldEquipment = null;
                OnDrop(true);
            }

            bool pickRight = PickUp && rightGrip && Vector3.Distance(currentRightControllerPosition, transform.position) < Distance && !InHand && EquipmentInteractor.instance.rightHandHeldEquipment == null && !DidSwap;
            bool swapRight = InHand && leftGrip && rightGrip && !DidSwap && (Vector3.Distance(currentRightControllerPosition, transform.position) < Distance) && SwappedLeft && EquipmentInteractor.instance.rightHandHeldEquipment == null;
            if (pickRight || swapRight)
            {
                DidSwap = swapRight;
                SwappedLeft = false;

                InLeftHand = false;
                InHand = true;
                transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent);

                GorillaTagger.Instance.StartVibration(false, 0.1f, 0.05f);
                EquipmentInteractor.instance.rightHandHeldEquipment = this;
                if (DidSwap) EquipmentInteractor.instance.leftHandHeldEquipment = null;

                OnGrab(false);
            }
            else if (!rightGrip && InHand && !InLeftHand)
            {
                InLeftHand = false;
                InHand = false;
                transform.SetParent(null);

                EquipmentInteractor.instance.rightHandHeldEquipment = null;
                OnDrop(false);
            }

            previousLeftControllerPosition = currentLeftControllerPosition;
            previousRightControllerPosition = currentRightControllerPosition;
        }
    }
}
