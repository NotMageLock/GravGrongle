using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GravGrongle
{
    internal class GrongleNetworking : MonoBehaviourPunCallbacks
    {
        internal static Dictionary<Player, Rigidbody> playerGrongleObjects = new Dictionary<Player, Rigidbody>();
        internal const byte grabCode = 87;
        internal const byte releaseCode = 88;
        internal const byte teleportCode = 89;
        internal const string modName = "GravGrongle";

        void Awake()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        public override void OnPlayerLeftRoom(Player player)
        {
            base.OnPlayerLeftRoom(player);
            if (playerGrongleObjects.ContainsKey(player))
            {
                Destroy(playerGrongleObjects[player].gameObject);
                playerGrongleObjects.Remove(player);
            }
        }

        private void OnEvent(EventData data)
        {
            var sender = PhotonNetwork.LocalPlayer.Get(data.Sender);
            var content = data.CustomData;

            if (sender == null || content == null)
                return;

            Rigidbody playersGrongle = GetPlayersGrongle(sender);

            switch (data.Code)
            {
                case grabCode:
                    VRRigCache.Instance.TryGetVrrig(sender, out var rigContainer);
                    var IsLeftHand = (bool)content;
                    playersGrongle.isKinematic = true;
                    playersGrongle.transform.SetParent(IsLeftHand ? rigContainer.vrrig.leftHandTransform : rigContainer.vrrig.rightHandTransform, false);
                    Debug.Log($"Recieved Grongle Grab Event From: {sender.NickName} - IsLeftHand: {IsLeftHand}");
                    break;

                case releaseCode:
                    var velocity = (Vector3)content;
                    playersGrongle.transform.parent = null;
                    playersGrongle.isKinematic = false;
                    playersGrongle.velocity = velocity;
                    Debug.Log($"Recieved Grongle Release Event From: {sender.NickName} - Velocity: {velocity}");
                    break;

                case teleportCode:
                    var position = (Vector3)content;
                    playersGrongle.transform.position = position;
                    Debug.Log($"Recieved Grongle Teleport Event From: {sender.NickName} - Position: {position}");
                    break;

                default:
                    break;
            }
        }

        Rigidbody GetPlayersGrongle(Player sender)
        {
            if (!playerGrongleObjects.ContainsKey(sender))
            {
                playerGrongleObjects.Add(sender, Instantiate(Plugin.gronglePrefab).GetComponent<Rigidbody>());
            }
            return playerGrongleObjects[sender];
        }

        internal static int[] GetModUsersActors()
        {
            int[] actors = PhotonNetwork.PlayerListOthers.Where(player => player.CustomProperties.ContainsKey(modName)).Select(player => player.ActorNumber).ToArray();
            return actors;
        }

        internal static void SendGrabEvent(bool IsLeftHand)
        {
            var actors = GetModUsersActors();
            if (actors.Length == 0)
                return;

            SendRaiseEvent(grabCode, IsLeftHand, actors);
            Debug.Log($"Sent Grongle Grab Event - IsLeftHand: {IsLeftHand}");
        }

        internal static void SendReleaseEvent(Vector3 velocity)
        {
            var actors = GetModUsersActors();
            if (actors.Length == 0)
                return;

            SendRaiseEvent(releaseCode, velocity, actors);
            Debug.Log($"Sent Grongle Release Event - Velocity: {velocity}");
        }

        internal static void SendTeleportEvent(Vector3 position)
        {
            var actors = GetModUsersActors();
            if (actors.Length == 0)
                return;

            SendRaiseEvent(releaseCode, position, actors);
            Debug.Log($"Sent Grongle Teleport Event - Position: {position}");
        }

        internal static void SendRaiseEvent(byte code, object contents, params int[] targetActors)
        {
            if (!PhotonNetwork.InRoom || !Plugin.inRoom)
                return;

            var raiseEventOptions = new RaiseEventOptions()
            {
                TargetActors = targetActors
            };
            PhotonNetwork.RaiseEvent(code, contents, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}