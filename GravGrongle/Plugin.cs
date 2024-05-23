using System;
using System.Reflection;
using BepInEx;
using GravGrongle.ButtonTypes;
using Photon.Pun;
using UnityEngine;
using Utilla;

namespace GravGrongle
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static bool inRoom;
        public GameObject grongle;
        public GameObject gmenu;
        public Vector3 pos;

        void Start()
        {
            Events.GameInitialized += OnGameInitialized;
            Debug.Log("Grongle is cool B)");
        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
            Debug.Log("I don't think that'll do anything (GRONGLE 4EVER!");
        }

        public static GameObject gronglePrefab;

        void OnGameInitialized(object sender, EventArgs e)
        {
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GravGrongle.Resources.gronglebundle");
            var bundle = AssetBundle.LoadFromStream(fileStream);

            gronglePrefab = bundle.LoadAsset("gronglemodel") as GameObject;

            MeshCollider meshCollider = gronglePrefab.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = gronglePrefab.GetComponent<MeshFilter>().mesh;
            meshCollider.convex = true;

            Rigidbody rb = gronglePrefab.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            rb.useGravity = true;
            gronglePrefab.SetLayer(UnityLayer.Prop);


            GameObject grongle = Instantiate(gronglePrefab);
            grongle.AddComponent<OTL>();
            grongle.AddComponent<DevHoldable>();

            Debug.Log("Grongle collider size: " + meshCollider.bounds.size);
            Debug.Log("Grongle rigidbody mass: " + rb.mass);

            GameObject grongleButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grongleButton.name = "Grongle Button";
            grongleButton.transform.position = new Vector3(0f, 0f, 0f);
            grongleButton.AddComponent<returnToButton>();
            grongleButton.AddComponent<DevHoldable>();
            grongleButton.AddComponent<ButtonOTL>();
            grongleButton.AddComponent<MeshRenderer>();
            grongleButton.GetComponent<MeshRenderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            grongleButton.AddComponent<MeshCollider>();
            gameObject.AddComponent<GrongleNetworking>();
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            var table = new ExitGames.Client.Photon.Hashtable();
            table.Add(GrongleNetworking.modName, PluginInfo.Version);
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inRoom = false;
        }
    }
}
