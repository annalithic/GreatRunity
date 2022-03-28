using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SoulsFormats;
using UnityEditor;

[ExecuteInEditMode]
public class GreatRunity : MonoBehaviour {
    public int map1;
    public int map2;
    public int map3;
    public int map4;
    public string path;
    public string gamePath = @"C:\Games\Steam\steamapps\common\ELDEN RING\Game"; //TODO DEHARDCODE

    enum PartType {
        PartTypeMapPiece = 0,
        PartTypeEnemy = 2,
        PartTypePlayer = 4,
        PartTypeCollision = 5,
        PartTypeDummyAsset = 9,
        PartTypeDummyEnemy = 10,
        PartTypeConnectCollision = 11,
        PartTypeAsset = 13,
    }


    public string GetPath() {
        return GetPath(map1, map2, map3, map4);
    }
    public string GetPath(int m1, int m2, int m3, int m4) {     //TODO MSBE
        return string.Format(@"E:\Extracted\Souls\Elden Ring\mapstudio\m{0:00}_{1:00}_{2:00}_{3:00}.txt", m1, m2, m3, m4) ;
    }

    public void ImportMapModels() { ImportMapModels(map1, map2, map3, map4); }
    public void ImportMapModels(int m1, int m2, int m3, int m4) {

        HashSet<string> assets = new HashSet<string>();
        
        foreach (string line in File.ReadAllLines(GetPath(m1, m2, m3, m4))) {
            string[] words = line.Split('|');
            PartType type = (PartType)int.Parse(words[1]);


            if (type == PartType.PartTypeAsset) {
                string asset = words[0].IndexOf('-') == -1 ? words[0] : words[0].Substring(words[0].IndexOf('-') + 1);
                asset = asset.Substring(0, asset.LastIndexOf('_'));
                asset = asset.ToLower();
                assets.Add($"asset\\aeg\\{asset.Substring(0, 6)}\\{asset}.geombnd.dcx");

            } //else if (type == PartType.PartTypeMapPiece) {
                //string piece = words[0].IndexOf('-') == -1 ? words[0] : words[0].Substring(words[0].IndexOf('-') + 1);
                //piece = piece.Substring(1, piece.LastIndexOf('_') - 1);
                //models.Add(piece);
                //Debug.Log(string.Format(@"E:\Anna\Anna\Unity\DSToolsEX\Assets\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\m{0:00}_{1:00}_{2:00}_{3:00}_{4}.prefab", map, x, y, level, asset));
            //}
        }

        int import = 0;
        foreach (string asset in assets) {
            if (!File.Exists(Path.Combine(Application.dataPath, asset.Replace(".geombnd.dcx", ".mesh")))) {
                if(import == 0) {
                    AssetDatabase.StartAssetEditing();
                }
                import++;
                ImportModel(asset);
            }
        }
        if (import > 0) AssetDatabase.StopAssetEditing();
        Debug.Log($"Imported {import} models");

    }

    public void ImportMap() { ImportMap(map1, map2, map3, map4); }

    public void ImportMap(int m1, int m2, int m3, int m4) {


        //TODO dunno
        Dictionary<string, string> aegNames = new Dictionary<string, string>();
        foreach (string line in File.ReadAllLines(@"E:\Extracted\Souls\Elden Ring\aegnames.txt")) aegNames[line.Split("\t")[0].ToUpper()] = line.Split("\t")[1];


        GameObject partPrefab = Resources.Load<GameObject>("PartPrefab");
        Material partMat = (Material)AssetDatabase.LoadAssetAtPath($@"Assets\Art\matAsset.mat", typeof(Material));
        Material noColMat = (Material)AssetDatabase.LoadAssetAtPath($@"Assets\Art\matNoCol.mat", typeof(Material));

        string path = GetPath(m1, m2, m3, m4);
        Transform root = new GameObject(Path.GetFileNameWithoutExtension(path)).transform;
        foreach (string line in File.ReadAllLines(path)) {
            string[] words = line.Split('|');
            PartType type = (PartType)int.Parse(words[1]);

            Transform part = ((GameObject)PrefabUtility.InstantiatePrefab(partPrefab, root)).transform;
            part.localPosition = new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]));
            Quaternion qy = Quaternion.AngleAxis(float.Parse(words[6]), new Vector3(0, 1, 0));
            Quaternion qz = Quaternion.AngleAxis(float.Parse(words[7]), new Vector3(0, 0, 1));
            Quaternion qx = Quaternion.AngleAxis(float.Parse(words[5]), new Vector3(1, 0, 0));
            part.rotation = qy * qz * qx;
            part.localScale = new Vector3(float.Parse(words[8]), float.Parse(words[9]), float.Parse(words[10]));
            part.name = words[0];


            if (type == PartType.PartTypeAsset) {
                string asset = words[0].IndexOf('-') == -1 ? words[0] : words[0].Substring(words[0].IndexOf('-') + 1);
                asset = asset.Substring(0, asset.LastIndexOf('_'));
                if (File.Exists(Path.Combine(Application.dataPath, $@"asset\aeg\{asset.Substring(0, 6)}\{asset}.mesh"))) {
                    MeshFilter m = part.GetComponent<MeshFilter>();
                    m.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath($@"Assets\asset\aeg\{asset.Substring(0, 6)}\{asset}.mesh", typeof(Mesh));
                    MeshRenderer r = part.GetComponent<MeshRenderer>();

                    //TODO SHOULD DO THIS STUFF ON IMPORT, CREATE PREFABS
                    Material mat = partMat;
                    if (!File.Exists(Path.Combine(gamePath, $@"asset\aeg\{asset.Substring(0, 6)}\{asset.Substring(0, 10)}_l.geomhkxbnd.dcx")) &&
                        !File.Exists(Path.Combine(gamePath, $@"asset\aeg\{asset.Substring(0, 6)}\{asset.Substring(0, 10)}_h.geomhkxbnd.dcx")))
                        mat = noColMat;

                    Material[] mats = new Material[m.sharedMesh.subMeshCount];
                    for (int i = 0; i < mats.Length; i++) mats[i] = mat;
                    r.sharedMaterials = mats;

                    if (aegNames.ContainsKey(asset)) part.name = aegNames[asset];

                //} else if (File.Exists($@"E:\Anna\Anna\Unity\DSToolsEX\Assets\Asset\{asset.Substring(3, 3)}\{asset}_01.prefab")) {
                //    obj = (GameObject)AssetDatabase.LoadAssetAtPath($@"Assets\Asset\{asset.Substring(3, 3)}\{asset}_01.prefab", typeof(GameObject));
                }
                /*
            } else if (type == PartType.PartTypeEnemy || type == PartType.PartTypePlayer || type == PartType.PartTypeDummyEnemy) {
                obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/PrefabChr.prefab", typeof(GameObject));
            } else if (type == PartType.PartTypeMapPiece) {

                string asset = words[0].IndexOf('-') == -1 ? words[0] : words[0].Substring(words[0].IndexOf('-') + 1);
                asset = asset.Substring(1, asset.LastIndexOf('_') - 1);
                Debug.Log(string.Format(@"E:\Anna\Anna\Unity\DSToolsEX\Assets\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\m{0:00}_{1:00}_{2:00}_{3:00}_{4}.prefab", map, x, y, level, asset));
                if (File.Exists(string.Format(@"E:\Anna\Anna\Unity\DSToolsEX\Assets\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\m{0:00}_{1:00}_{2:00}_{3:00}_{4}.prefab", map, x, y, level, asset))) {
                    obj = (GameObject)AssetDatabase.LoadAssetAtPath(string.Format(@"Assets\map\m{0:00}\m{0:00}_{1:00}_{2:00}_{3:00}\m{0:00}_{1:00}_{2:00}_{3:00}_{4}.prefab", map, x, y, level, asset), typeof(GameObject));
                }
                */
            }

            //part.SetParent(root);
        }
    }

    void ImportModel(string path) {
        try  {
            BND4 bnd = BND4.Read(Path.Combine(gamePath, path));
            FLVER2 flv = FLVER2.Read(bnd.Files[0].Bytes);
            Mesh m = FlverUtilities.ImportFlverMesh(flv);
            m.name = Path.GetFileNameWithoutExtension(path);
            if (!Directory.Exists(Path.Combine(Application.dataPath, Path.GetDirectoryName(path)))) Directory.CreateDirectory(Path.Combine(Application.dataPath, Path.GetDirectoryName(path)));
            AssetDatabase.CreateAsset(m, Path.Combine("Assets", path.Replace(".geombnd.dcx", ".mesh")));
        } catch {
            Debug.LogError(Path.GetFileName(path) + "import failed.");
        }
    }
}
