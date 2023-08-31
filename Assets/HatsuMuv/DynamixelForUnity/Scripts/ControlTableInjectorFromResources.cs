using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_64
using HatsuMuv.DynamixelForUnity.x64;
#else
using HatsuMuv.DynamixelForUnity.x86;
#endif

namespace HatsuMuv.DynamixelForUnity {

    public class ControlTableInjectorFromResources: MonoBehaviour
    {
        public string resourcesPath = "ControlTables";

        public void Awake()
        {
            TextAsset[] textAssets = Resources.LoadAll<TextAsset>(resourcesPath);

            var controlTableData = new List<string>();

            foreach (TextAsset textAsset in textAssets)
            {
                controlTableData.Add(textAsset.text);
            }

            var controlTable = new ControlTables(controlTableData);

            // Inject control table
            var targets = GetComponents(typeof(IControlTableUser));

            if (targets == null)
                return;

            foreach (var target in targets)
            {
                if(target is IControlTableUser u)
                    u.SetControlTables(controlTable);
            }
        }
    }
}