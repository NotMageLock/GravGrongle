﻿using UnityEngine;

namespace GravGrongle
{
    public class OTL : MonoBehaviour
    {
        public Vector3 pos;

        public static OTL Instance;
        public void Start() //Initial position for Grongle
        {
            Vector3 newPosition = new Vector3(-65.3796f, 11.814f, -81.414f);
            transform.position = newPosition;

            Vector3 newRotation = new Vector3(270f, 149.4319f, 0f);
            transform.eulerAngles = newRotation;

            Vector3 newScale = new Vector3(25f, 25f, 25f);
            transform.localScale = newScale;
        }
    }
}