using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public bool importMapPieces;
    public bool importAsset;
    public string importAssetFolder;
    public Material mat;
    public GameObject partPrefab;

    public int map;
    public int x;
    public int y;
    public int level;
    public bool importMap;

    // Update is called once per frame
    void Update()
    {
        if (importMapPieces) { importMapPieces = false; ImportMeshBnd(@"C:\Games\Steam\steamapps\common\ELDEN RING\Game\asset\aeg\aeg003\aeg003_300.geombnd.dcx");  }
        //if (importMapPieces) { importMapPieces = false; ImportMapPieces(); }
        if (importAsset) { importAsset = false; ImportAssets(importAssetFolder); }
        if(importMap) { importMap = false; ImportMap(string.Format(@"E:\Extracted\Souls\Elden Ring\mapstudio\m{0:00}_{1:00}_{2:00}_{3:00}.txt", map, x, y, level)); }
    }

    enum PartType {
        PartTypeMapPiece = 0,
        PartTypeEnemy = 2,
        PartTypePlayer = 4,
        PartTypeCollision = 5,
        PartTypeDummyAsset = 9, // Highly speculative
        PartTypeDummyEnemy = 10,
        PartTypeConnectCollision = 11,
        PartTypeAsset = 13,
    }

    void ImportMap(string path) {
        Transform root = new GameObject(Path.GetFileNameWithoutExtension(path)).transform;
        foreach(string line in File.ReadAllLines(path)) {
            string[] words = line.Split('|');
            PartType type = (PartType)int.Parse(words[1]);
            
            GameObject obj =  partPrefab;

            if (type == PartType.PartTypeAsset) {
                string asset = words[0].IndexOf('-') == -1 ? words[0] : words[0].Substring(words[0].IndexOf('-') + 1);
                asset = asset.Substring(0, asset.LastIndexOf('_'));
                if (File.Exists($@"E:\Anna\Anna\Unity\DSToolsEX\Assets\Asset\{asset.Substring(3, 3)}\{asset}.prefab")) {
                    obj = (GameObject)AssetDatabase.LoadAssetAtPath($@"Assets\Asset\{asset.Substring(3, 3)}\{asset}.prefab", typeof(GameObject));
                } else if (File.Exists($@"E:\Anna\Anna\Unity\DSToolsEX\Assets\Asset\{asset.Substring(3, 3)}\{asset}_01.prefab")) {
                    obj = (GameObject)AssetDatabase.LoadAssetAtPath($@"Assets\Asset\{asset.Substring(3, 3)}\{asset}_01.prefab", typeof(GameObject));
                }
            } else if (type == PartType.PartTypeEnemy || type == PartType.PartTypePlayer || type == PartType.PartTypeDummyEnemy) {
                obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/PrefabChr.prefab", typeof(GameObject));
            } else if (type == PartType.PartTypeMapPiece) {

                string asset = words[0].IndexOf('-') == -1 ? words[0] : words[0].Substring(words[0].IndexOf('-') + 1);
                asset = asset.Substring(1, asset.LastIndexOf('_') - 1);
                Debug.Log(string.Format(@"E:\Anna\Anna\Unity\DSToolsEX\Assets\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\m{0:00}_{1:00}_{2:00}_{3:00}_{4}.prefab", map, x, y, level, asset));
                if (File.Exists(string.Format(@"E:\Anna\Anna\Unity\DSToolsEX\Assets\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\m{0:00}_{1:00}_{2:00}_{3:00}_{4}.prefab", map, x, y, level, asset))) {
                    obj = (GameObject)AssetDatabase.LoadAssetAtPath(string.Format(@"Assets\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\m{0:00}_{1:00}_{2:00}_{3:00}_{4}.prefab", map, x, y, level, asset), typeof(GameObject));
                }
            }
            Transform part = ((GameObject)PrefabUtility.InstantiatePrefab(obj, root)).transform;
            part.localPosition = new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]));
            Quaternion qy = Quaternion.AngleAxis(float.Parse(words[6]), new Vector3(0, 1, 0));
            Quaternion qz = Quaternion.AngleAxis(float.Parse(words[7]), new Vector3(0, 0, 1));
            Quaternion qx = Quaternion.AngleAxis(float.Parse(words[5]), new Vector3(1, 0, 0));
            part.rotation = qy * qz * qx;
            part.localScale = new Vector3(float.Parse(words[8]), float.Parse(words[9]), float.Parse(words[10]));

            //part.SetParent(root);
            part.name = words[0];
        }
    }

    void DoThing() {
        //foreach (string path in Directory.EnumerateFiles(@"F:\Extracted\Souls\Elden Ring\CUSA18880-app\asset\aeg", "*.geombnd.dcx", SearchOption.AllDirectories)) ImportMapBnd(path);
        //ImportMapBnd(@"F:\Extracted\Souls\Elden Ring\CUSA18880-app\asset\aeg\aeg001\aeg001_169.geombnd.dcx");
        ImportOverworld();
        //foreach (var file in bnd.Files) Debug.Log(file.Name);
        //foreach (string path in Directory.EnumerateFiles(@"F:\Extracted\Souls\Elden Ring\CUSA18880-app\map\m18\m18_00_00_00", "*.mapbnd.dcx")) ImportMapBnd(path);
    }

    void ImportOverworld() {
        
        foreach (string directory in Directory.EnumerateDirectories(@"C:\Games\Steam\steamapps\common\ELDEN RING\Game\map\m60")) {
            string map = Path.GetFileName(directory);
            if (!map.EndsWith("_00")) continue;
            //int x = int.Parse(map.Split('_')[1]);
            //int y = int.Parse(map.Split('_')[2]);
            //Vector3 offset = new Vector3((x - 32) * 256, 0, (y - 32) * 256);
            foreach (string file in Directory.EnumerateFiles(directory, "*.mapbnd.dcx")) {
                string filename = Path.GetFileNameWithoutExtension(file);
                int x = int.Parse(filename.Substring(15, 2));
                int y = int.Parse(filename.Substring(17, 2));
                Vector3 offset = new Vector3((x - 8) * 1024, 0, (y - 8) * 1024);
                ImportMapBnd(file, offset, "m60");
            }
            
        }
    }

    void ImportAssets(string folderNum) {
        AssetDatabase.StartAssetEditing();
        foreach (string path in Directory.EnumerateFiles($@"C:\Games\Steam\steamapps\common\ELDEN RING\Game\asset\aeg\aeg{folderNum}", "*.geombnd.dcx", SearchOption.AllDirectories)) {
            ImportMeshBnd(path, "Asset\\" + folderNum);
            //break;
            //Debug.Log(path);
        }
        AssetDatabase.StopAssetEditing();
    }


    void ImportMapPieces() {
        Material m = (Material)AssetDatabase.LoadAssetAtPath("Assets/matmap.mat", typeof(Material));
        AssetDatabase.StartAssetEditing();
        //Debug.Log(string.Format(@"C:\Games\Steam\steamapps\common\ELDEN RING\Game\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\", map, x, y, level));
        foreach (string path in Directory.EnumerateFiles(string.Format(@"C:\Games\Steam\steamapps\common\ELDEN RING\Game\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}", map, x, y, level), "*.mapbnd.dcx", SearchOption.AllDirectories)) {
            //Debug.Log(path);
            ImportMapBnd(path, Vector3.zero, string.Format(@"map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\", map, x, y, level), m);
            //break;
            //Debug.Log("s");
        }
        AssetDatabase.StopAssetEditing();
    }
    //ss
    void ImportMapBnd(string path) { ImportMapBnd(path, Vector3.zero); }

    void ImportMeshBnd(string path, string saveFolder = "") {
        BND4 bnd = BND4.Read(path);
        foreach (var file in bnd.Files) {
            if (Path.GetExtension(file.Name) != ".flver") continue;
            try {
                FLVER2 flv = FLVER2.Read(file.Bytes);
                //Debug.Log(flv.Meshes.Count);
                Mesh m = FlverUtilities.ImportFlverMesh(flv, false);
                m.name = Path.GetFileNameWithoutExtension(path);
                if (saveFolder != "") {
                    if (!Directory.Exists(Path.Combine(Application.dataPath, saveFolder))) Directory.CreateDirectory(Path.Combine(Application.dataPath,saveFolder));
                    AssetDatabase.CreateAsset(m, $"Assets\\{saveFolder}\\{m.name}.mesh");
                } else {
                    AssetDatabase.CreateAsset(m, $"Assets\\{m.name}.mesh");
                }
            } catch {
                Debug.LogError(file.Name);
            }   
        }
    }


    void ImportMapBnd(string path, Vector3 offset, string saveFolder = "", Material m = null) {
        if (m == null) m = mat;
        BND4 bnd = BND4.Read(path);

        foreach (var file in bnd.Files) {
            if (Path.GetExtension(file.Name) == ".flver") {
                //Debug.Log(file.Name);
                try {
                    FLVER2 flv = FLVER2.Read(file.Bytes);
                    //Debug.Log(flv.Meshes.Count);
                    GameObject obj = FlverUtilities.ImportFlver(flv, Path.GetFileName(file.Name), m, true);
                    obj.transform.position += offset;
                    if(saveFolder != "") {
                        if (!Directory.Exists(Application.dataPath + "\\Asset\\" + saveFolder)) Directory.CreateDirectory(Application.dataPath + "\\Asset\\" + saveFolder);
                        PrefabUtility.SaveAsPrefabAsset(obj, $"Asset\\{saveFolder}\\{obj.name}.prefab", out bool success);
                        //PrefabUtility.CreatePrefab("Assets\\Asset\\" + obj.name + ".prefab", obj);
                        //AssetDatabase.CreateAsset(, );
                        DestroyImmediate(obj);
                    }
                } catch {
                    Debug.LogError(file.Name);
                }
            }

        }
    }
}
