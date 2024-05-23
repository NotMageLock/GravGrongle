using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GravGrongle
{
    internal class Networking : MonoBehaviourPunCallbacks //Huge thanks to HuskyGT for networking the mod
    {
        internal const byte grabCode = 87;
        internal const byte releaseCode = 88;
        internal const byte teleportCode = 89;
        void Awake()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnEvent(EventData data)
        {
            switch(data.Code)
            {
                case grabCode:
                    break;
                case releaseCode:
                    break;
                case teleportCode:
                    break;
                default:
                    break;
            }
        }

        internal static void SendGrabEvent(bool IsLeftHand)
        {
            var Actors = GetModUsersActors();
            SendRaiseEvent(grabCode, IsLeftHand, Actors);
        }
        internal static int[] GetModUsersActors()
        {
            List<int> actors = new List<int>();
            foreach (var player in PhotonNetwork.PlayerListOthers)
            {
                if (player.CustomProperties.ContainsKey("GravGrongle") && player.CustomProperties["GravGrongle"].ToString() == PluginInfo.Version)
                {
                    actors.Add(player.ActorNumber);
                }
            }
            return actors.ToArray();
        }
        internal static void SendRaiseEvent(byte code, object contents)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All
            };
            PhotonNetwork.RaiseEvent(code, contents, raiseEventOptions, SendOptions.SendReliable);
        }
        internal static void SendRaiseEvent(byte code, object contents, params int[] targetActors)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var raiseEventOptions = new RaiseEventOptions()
            {
                TargetActors = targetActors
            };
            PhotonNetwork.RaiseEvent(code, contents, raiseEventOptions, SendOptions.SendReliable);
        }
        internal static void SendRaiseEvent(byte code, object contents, ReceiverGroup receivers)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = receivers
            };
            PhotonNetwork.RaiseEvent(code, contents, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}
