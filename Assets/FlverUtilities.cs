using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using SoulsFormats;
using Object = UnityEngine.Object;

//s
/// <summary>
/// Flver import/export utilities
/// </summary>
class FlverUtilities
{
    // Hardcoded table of materials that don't have UV index 1 as their lightmap UVs
    static Dictionary<string, int> MaterialLightmapUVIndex = new Dictionary<string, int>()
    {
        { $@"M[ARSN]_l_m", 2 }
    };

    static void EulerToTransform(Vector3 e, Transform t)
    {
        Matrix4x4 mat;
        //mat = Matrix4x4.Rotate(Quaternion.AngleAxis(e.x * Mathf.Rad2Deg, new Vector3(1, 0, 0))) *
        //      Matrix4x4.Rotate(Quaternion.AngleAxis(-e.z * Mathf.Rad2Deg, new Vector3(0, 0, 1))) *
        //      Matrix4x4.Rotate(Quaternion.AngleAxis(e.y * Mathf.Rad2Deg, new Vector3(0, 1, 0)));
        mat = Matrix4x4.Rotate(Quaternion.AngleAxis(e.y * Mathf.Rad2Deg, new Vector3(0, 1, 0))) *
             Matrix4x4.Rotate(Quaternion.AngleAxis(e.z * Mathf.Rad2Deg, new Vector3(0, 0, 1))) *
             Matrix4x4.Rotate(Quaternion.AngleAxis(e.x * Mathf.Rad2Deg, new Vector3(1, 0, 0)));
        t.localRotation = mat.rotation;
        
    }

    static void SetBoneWorldTransform(Transform t, FLVER.Bone[] bones, int boneIndex)
    {
        Matrix4x4 mat = Matrix4x4.identity;
        var bone = bones[boneIndex];
        do
        {
            mat *= Matrix4x4.Scale(new Vector3(bone.Scale.X, bone.Scale.Y, bone.Scale.Z));
            mat *= Matrix4x4.Rotate(Quaternion.AngleAxis(bone.Rotation.X * Mathf.Rad2Deg, new Vector3(1, 0, 0)));
            mat *= Matrix4x4.Rotate(Quaternion.AngleAxis(bone.Rotation.Z * Mathf.Rad2Deg, new Vector3(0, 0, 1)));
            mat *= Matrix4x4.Rotate(Quaternion.AngleAxis(bone.Rotation.Y * Mathf.Rad2Deg, new Vector3(0, 1, 0)));
            mat *= Matrix4x4.Translate(new Vector3(bone.Translation.X, bone.Translation.Y, bone.Translation.Z));
            bone = (bone.ParentIndex != -1) ? bones[bone.ParentIndex] : null;
        } while (bone != null);
        t.position = new Vector3(mat.m03, mat.m13, mat.m23);
        var scale = new Vector3();
        scale.x = new Vector4(mat.m00, mat.m10, mat.m20, mat.m30).magnitude;
        scale.y = new Vector4(mat.m01, mat.m11, mat.m21, mat.m31).magnitude;
        scale.z = new Vector4(mat.m02, mat.m12, mat.m22, mat.m32).magnitude;
        t.localScale = scale;
        t.rotation = mat.rotation;
    }
    /*

    // Helper method to lookup nonlocal textures from another bnd
    static Texture2D FindTexture(string path, DarkSoulsTools.GameType gameType)
    {
        string gamePath = DarkSoulsTools.GameFolder(gameType);

        // Map texture reference
        if (path.Contains(@"\map\"))
        {
            var splits = path.Split('\\');
            var mapid = splits[splits.Length - 3];
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/{mapid}/{Path.GetFileNameWithoutExtension(path)}.dds");
            if (asset == null)
            {
                // Attempt to load UDSFM texture
                asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/UDSFMMapTextures/{Path.GetFileNameWithoutExtension(path)}.dds");
            }
            return asset;
        }
        // Chr texture reference
        else if (path.Contains(@"\chr\"))
        {
            var splits = path.Split('\\');
            var chrid = splits[splits.Length - 3];
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Chr/{chrid}/{Path.GetFileNameWithoutExtension(path)}.dds");
            if (asset == null)
            {
                // Attempt to load shared chr textures
                asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Chr/sharedTextures/{Path.GetFileNameWithoutExtension(path)}.dds");
            }
            return asset;
        }
        // Obj texture reference
        else if (path.Contains(@"\obj\"))
        {
            var splits = path.Split('\\');
            var objid = splits[splits.Length - 3];
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Obj/{objid}/{Path.GetFileNameWithoutExtension(path)}.dds");
            if (asset == null)
            {
                // Attempt to load shared chr textures
                asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Obj/sharedTextures/{Path.GetFileNameWithoutExtension(path)}.dds");
            }
            return asset;
        }
        // Parts texture reference
        else if (path.Contains(@"\parts\"))
        {
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Parts/textures/{Path.GetFileNameWithoutExtension(path)}.dds");
            return asset;
        }
        return null;
    }
    */

    static public GameObject ImportFlver(FLVER2 flver, string assetName, Material mat, bool saveMeshes, string texturePath = null, bool mapflver = false)
    {
 

   

        GameObject root = new GameObject(Path.GetFileNameWithoutExtension(assetName));
        //GameObject meshesObj = new GameObject("Meshes");
        //GameObject bonesObj = new GameObject("Bones");
        //meshesObj.transform.parent = root.transform;
        //bonesObj.transform.parent = root.transform;

        // import the skeleton

        /*
        Transform[] bones = new Transform[flver.Bones.Count];
        Matrix4x4[] bindPoses = new Matrix4x4[flver.Bones.Count];
        for (int i = 0; i < flver.Bones.Count; i++)
        {
            var fbone = flver.Bones[i];
            bones[i] = new GameObject(fbone.Name).transform;
            EulerToTransform(new Vector3(fbone.Rotation.X, fbone.Rotation.Y, fbone.Rotation.Z), bones[i]);
            bones[i].localPosition = new Vector3(fbone.Translation.X, fbone.Translation.Y, fbone.Translation.Z);
            bones[i].localScale = new Vector3(fbone.Scale.X, fbone.Scale.Y, fbone.Scale.Z);
            //SetBoneWorldTransform(bones[i], flver.Bones.ToArray(), i);
            //bindPoses[i] = bones[i].worldToLocalMatrix * root.transform.localToWorldMatrix;
        }

        // Skeleton parenting
        for (int i = 0; i < flver.Bones.Count; i++)
        {
            var fbone = flver.Bones[i];
            if (fbone.ParentIndex == -1)
            {
                //bones[i].parent = root.transform;
                bones[i].SetParent(bonesObj.transform, false);
                bindPoses[i] = bones[i].worldToLocalMatrix * root.transform.localToWorldMatrix;
            }
            else
            {
                //bones[i].parent = bones[fbone.ParentIndex];
                bones[i].SetParent(bones[fbone.ParentIndex], false);
                bindPoses[i] = bones[i].worldToLocalMatrix * root.transform.localToWorldMatrix;
            }
        }
        */

        // Import the meshes

        CombineInstance[] combine = new CombineInstance[flver.Meshes.Count]; 

        int index = 0;
        for (int meshIndex = 0; meshIndex < flver.Meshes.Count; meshIndex++ )
        {
            var m = flver.Meshes[meshIndex];
            var mesh = new Mesh();

            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var boneweights = new List<BoneWeight>();
            var smcount = 0;
            bool usestangents = false;
            int uvcount = m.Vertices[0].UVs.Count;
            List<Vector2>[] uvs = new List<Vector2>[uvcount];
            List<Material> matList = new List<Material>();

            // Add the mesh to the asset link
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new List<Vector2>();
            }
            //bool isSkinned = false;
            foreach (var v in m.Vertices)
            {
                verts.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                normals.Add(new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z));
                if (v.Tangents.Count > 0)
                {
                    tangents.Add(new Vector4(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z, v.Tangents[0].W));
                    usestangents = true;
                }
                else
                {
                    tangents.Add(new Vector4(0, 0, 0, 1));
                }
                for (int i = 0; i < uvs.Length; i++)
                {
                    // Swap lightmap uvs with uv index 1 because lmao unity
                    //if (i == 1)
                    //{
                    //    uvs[i].Add(new Vector2(v.UVs[lightmapUVIndex].X, 1.0f - v.UVs[lightmapUVIndex].Y));
                    //}
                    //else if (i == lightmapUVIndex)
                    //{
                    //    uvs[i].Add(new Vector2(v.UVs[1].X, 1.0f - v.UVs[1].Y));
                    //}
                    //else
                    //{
                        uvs[i].Add(new Vector2(v.UVs[i].X, 1.0f - v.UVs[i].Y));
                    //}
                }
                /*
                if (v.BoneWeights != null && v.BoneWeights.Count() > 0)
                {
                    isSkinned = true;
                    var weight = new BoneWeight();

                    if (m.Unk1 == 0)
                    {
                        weight.boneIndex0 = v.BoneIndices[0];
                        weight.boneIndex1 = v.BoneIndices[1];
                        weight.boneIndex2 = v.BoneIndices[2];
                        weight.boneIndex3 = v.BoneIndices[3];
                    }
                    else
                    {
                        weight.boneIndex0 = m.BoneIndices[v.BoneIndices[0]];
                        weight.boneIndex1 = m.BoneIndices[v.BoneIndices[1]];
                        weight.boneIndex2 = m.BoneIndices[v.BoneIndices[2]];
                        weight.boneIndex3 = m.BoneIndices[v.BoneIndices[3]];
                    }
                    if (v.BoneWeights[0] < 0.0)
                    {
                        weight.weight0 = 1.0f;
                    }
                    else
                    {
                        weight.weight0 = v.BoneWeights[0];
                    }
                    weight.weight1 = v.BoneWeights[1];
                    weight.weight2 = v.BoneWeights[2];
                    weight.weight3 = v.BoneWeights[3];
                    boneweights.Add(weight);
                }
                else
                {
                    */
                    //boneweights.Add(new BoneWeight());
                //}
                
            }
            foreach (var fs in m.FaceSets)
            {
                if (fs.Indices.Count() > 0 && fs.Flags == FLVER2.FaceSet.FSFlags.None)
                {
                    //matList.Add(materials[m.MaterialIndex]);
                    smcount++;
                }
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.subMeshCount = smcount;
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            if (usestangents)
                mesh.SetTangents(tangents);
            //if (isSkinned)
            //{
            //   mesh.boneWeights = boneweights.ToArray();
            //    mesh.bindposes = bindPoses;
            //}

            for (int i = 0; i < uvs.Length; i++)
            {
                mesh.SetUVs(i, uvs[i]);
            }

            var submesh = 0;
            foreach (var fs in m.FaceSets)
            {
                if (fs.Indices.Count() == 0)
                    continue;
                if (fs.Flags != FLVER2.FaceSet.FSFlags.None)
                    continue;

                mesh.SetTriangles(fs.Triangulate(false).ToArray(), submesh, true, 0);
                mesh.triangles = mesh.triangles.Reverse().ToArray();
                submesh++;
            }
            mesh.RecalculateBounds();
            combine[meshIndex].mesh = mesh;

            // Setup a game object asset
            //GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(assetName) + $@"_{index}");

            //if (saveMeshes) AssetDatabase.CreateAsset(mesh, "Assets\\resources\\" + obj.name + ".mesh");
            /*
            if (isSkinned)
            {
                obj.AddComponent<SkinnedMeshRenderer>();
                obj.GetComponent<SkinnedMeshRenderer>().materials = matList.ToArray();
                obj.GetComponent<SkinnedMeshRenderer>().bones = bones;
                obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
            }
            else
            {*/
                //obj.AddComponent<MeshRenderer>().sharedMaterial = mat; ;
                
                //obj.GetComponent<MeshRenderer>().materials = matList.ToArray();
                //obj.GetComponent<MeshRenderer>().materials = matList.ToArray();
                //obj.AddComponent<MeshFilter>().mesh = mesh;
            //}
            //obj.AddComponent<FlverSubmesh>();
            //obj.GetComponent<FlverSubmesh>().Link = assetLink;
            //obj.GetComponent<FlverSubmesh>().SubmeshIdx = index;
            //obj.transform.parent = root.transform;

            //AssetDatabase.CreateAsset(mesh, assetName + $@"/{Path.GetFileNameWithoutExtension(assetName)}_{index}.mesh");
            //index++;
        }

        // If there's no meshes, create an empty one to bind the skeleton to so that Maya works
        // when you export the skeleton (like with c0000).
        /*
        if (flver.Meshes.Count == 0)
        {
            var mesh = new Mesh();

            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var boneweights = new List<BoneWeight>();
            for (var i = 0; i < 3; i++)
            {
                verts.Add(new Vector3(0.0f, 0.0f, 0.0f));
                normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                tangents.Add(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
                var weight = new BoneWeight();
                weight.boneIndex0 = 0;
                weight.weight0 = 1.0f;
                boneweights.Add(weight);
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.subMeshCount = 1;
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            mesh.SetTangents(tangents);
            mesh.boneWeights = boneweights.ToArray();
            //mesh.bindposes = bindPoses;
            mesh.SetTriangles( new int [] { 0, 1, 2 }, 0);

            GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(assetName) + $@"_{index}");
            obj.AddComponent<SkinnedMeshRenderer>();
            obj.GetComponent<SkinnedMeshRenderer>().bones = bones;
            obj.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
            obj.transform.parent = meshesObj.transform;

            //AssetDatabase.CreateAsset(mesh, assetName + $@"/{Path.GetFileNameWithoutExtension(assetName)}_{index}.mesh");
        }
        */

        //root.AddComponent<FlverMesh>();
        //root.GetComponent<FlverMesh>().Link = assetLink;

        //AssetDatabase.CreateAsset(assetLink, assetName + ".asset");
        //AssetDatabase.SaveAssets();
        //PrefabUtility.SaveAsPrefabAsset(root, assetName + ".prefab");
        //Object.DestroyImmediate(root);

        return root;
    }

    static public Mesh ImportFlverMesh(FLVER2 flver, bool combineMeshes) {

        CombineInstance[] combine = new CombineInstance[flver.Meshes.Count];

        for (int meshIndex = 0; meshIndex < flver.Meshes.Count; meshIndex++) {
            var m = flver.Meshes[meshIndex];
            var mesh = new Mesh();

            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var smcount = 0;
            bool usestangents = false;
            int uvcount = m.Vertices[0].UVs.Count;
            List<Vector2>[] uvs = new List<Vector2>[uvcount];


            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] = new List<Vector2>();
            }
            foreach (var v in m.Vertices) {
                verts.Add(new Vector3(v.Position.X, v.Position.Y, v.Position.Z));
                normals.Add(new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z));
                if (v.Tangents.Count > 0) {
                    tangents.Add(new Vector4(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z, v.Tangents[0].W));
                    usestangents = true;
                } else {
                    tangents.Add(new Vector4(0, 0, 0, 1));
                }
                for (int i = 0; i < uvs.Length; i++) {
                    uvs[i].Add(new Vector2(v.UVs[i].X, 1.0f - v.UVs[i].Y));
                }
            }
            foreach (var fs in m.FaceSets) {
                if (fs.Indices.Count() > 0 && fs.Flags == FLVER2.FaceSet.FSFlags.None) {
                    //matList.Add(materials[m.MaterialIndex]);
                    smcount++;
                }
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.subMeshCount = smcount;
            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            if (usestangents)  mesh.SetTangents(tangents);

            for (int i = 0; i < uvs.Length; i++) {
                mesh.SetUVs(i, uvs[i]);
            }

            var submesh = 0;
            foreach (var fs in m.FaceSets) {
                if (fs.Indices.Count() == 0)
                    continue;
                if (fs.Flags != FLVER2.FaceSet.FSFlags.None)
                    continue;

                mesh.SetTriangles(fs.Triangulate(false).ToArray(), submesh, false, 0);
                submesh++;
            }
            mesh.triangles = mesh.triangles.Reverse().ToArray();

            //combine submeshes
            //mesh.SetTriangles(mesh.triangles, 0);
            //mesh.subMeshCount = 1;

            mesh.RecalculateBounds();



            combine[meshIndex].mesh = mesh;
            combine[meshIndex].transform = Matrix4x4.identity;
            //combine[meshIndex].subMeshIndex = meshIndex;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, combineMeshes);
        //Mesh combinedMesh = combine[1].mesh;
        return combinedMesh;
    }
}
