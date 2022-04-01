using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SoulsFormats;

public class BTLComponent : MonoBehaviour
{
    public int version;
    public int m1; public int m2; public int m3; public int m4;

    public void SetData(int version, int m1, int m2, int m3, int m4) {
        this.version = version; this.m1 = m1; this.m2 = m2; this.m3 = m3; this.m4 = m4;
    }

    public void Export() {
        GreatRunity importer = GameObject.FindGameObjectWithTag("importer").GetComponent<GreatRunity>();
        if(importer == null) { Debug.LogError("Could not find importer"); return; }
        if(importer.modPath == null) { Debug.LogError("Mod path not set"); return; }
        string exportPath = Path.Combine(importer.modPath, importer.GetLightPath(m1, m2, m3, m4, true));

        BTL btl = new BTL();
        btl.Version = version;
        for(int i = 0; i < transform.childCount; i++) {
            Transform lightObj = transform.GetChild(i);
            SoulsLightComponent soulsLight = lightObj.GetComponent<SoulsLightComponent>(); 
            if (soulsLight == null) { Debug.LogError($"{lightObj.name} has no SoulsLightComponent"); continue; }
            btl.Lights.Add(soulsLight.Export());
        }

        if (!Directory.Exists(Path.GetDirectoryName(exportPath))) Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
        btl.Write(exportPath, DCX.Type.DCX_KRAK);
        Debug.Log("exported " + exportPath);
    }
}
