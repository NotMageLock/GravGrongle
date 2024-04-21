using System;
using System.EnterpriseServices;
using System.IO;
using System.Reflection;
using BepInEx;
using GravGrongle.ButtonTypes;
using OculusSampleFramework;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;
using Utilla;
using static UnityEngine.UI.DefaultControls;

namespace GravGrongle
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        public GameObject grongle;
        public GameObject gmenu;
        public Vector3 pos;
        private bool gripButtonPressed = false;
        private bool inHand = false;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GravGrongle.Resources.gronglebundle");
            var bundle = AssetBundle.LoadFromStream(fileStream);

            var grongleResource = bundle.LoadAsset("gronglemodel") as GameObject;

            GameObject grongle = Instantiate(grongleResource);
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

            GameObject grongleButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grongleButton.name = "Grongle Button";
            grongleButton.transform.position = new Vector3(0f, 0f, 0f);
            grongleButton.AddComponent<returnToButton>();
            grongleButton.AddComponent<DevHoldable>();
            grongleButton.AddComponent<ButtonOTL>();
            grongleButton.AddComponent<MeshRenderer>();
            grongleButton.GetComponent<MeshRenderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            grongleButton.AddComponent<MeshCollider>();
        }


        void Update()
        {
        
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inRoom = false;
        }
    }
}