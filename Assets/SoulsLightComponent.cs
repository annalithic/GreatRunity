using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
public class SoulsLightComponent : MonoBehaviour
{
    Light lightComponent;

    public Color diffuseColor;
    public Color specularColor;
    public Color shadowColor;
    public BTL.Light l;

    public byte unkA00;
    public byte unkA01;
    public byte unkA02;
    public byte unkA03;

    public bool disableSomethingA;
    public bool disableSomethingB;
    public bool disableSomethingC;
    public bool disableSomethingD;


    Color ToUnityColor(System.Drawing.Color c) { return new Color(c.R / 255f, c.G / 255f, c.B / 255f); }
    System.Drawing.Color ToSystemColor(Color c) { return System.Drawing.Color.FromArgb((byte)(c.r * 255), (byte)(c.g * 255), (byte)(c.b * 255)); }

    public void Import(BTL.Light l) {

        name = l.Name;
        transform.position = new Vector3(l.Position.X, l.Position.Y, l.Position.Z);

        diffuseColor = ToUnityColor(l.DiffuseColor);
        specularColor = ToUnityColor(l.SpecularColor);
        shadowColor = ToUnityColor(l.ShadowColor);

        unkA00 = l.UnkA0[0];
        unkA01 = l.UnkA0[1];
        unkA02 = l.UnkA0[2];
        unkA03 = l.UnkA0[3];

        disableSomethingA = l.UnkC0[0] == 1;
        disableSomethingB = l.UnkC0[1] == 1;
        disableSomethingC = l.UnkC0[2] == 1;
        disableSomethingD = l.UnkC0[3] == 1;

        this.l = l;
        UpdateLight();
    }

    public BTL.Light Export() {
        l.Name = name;
        l.Position = new System.Numerics.Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);


        l.SpecularColor = ToSystemColor(specularColor);
        l.ShadowColor = ToSystemColor(shadowColor);

        l.UnkA0[0] = unkA00;
        l.UnkA0[1] = unkA01;
        l.UnkA0[2] = unkA02;
        l.UnkA0[3] = unkA03;

        l.UnkC0[0] = disableSomethingA ? (byte)1 : (byte)0;
        l.UnkC0[1] = disableSomethingB ? (byte)1 : (byte)0;
        l.UnkC0[2] = disableSomethingC ? (byte)1 : (byte)0;
        l.UnkC0[3] = disableSomethingD ? (byte)1 : (byte)0;

        Light lightComponent = GetComponent<Light>();

        if (lightComponent.type == LightType.Directional) l.Type = BTL.LightType.Directional;
        else if (lightComponent.type == LightType.Spot) l.Type = BTL.LightType.Spot;
        else l.Type = BTL.LightType.Point;

        l.Radius = lightComponent.range;
        l.DiffusePower = lightComponent.intensity;
        l.DiffuseColor = ToSystemColor(lightComponent.color);
        l.ConeAngle = lightComponent.spotAngle;
        return l;
    }

    public void UpdateLight() {
        if(lightComponent == null) lightComponent = GetComponent<Light>();

        lightComponent.range = l.Radius;
        lightComponent.intensity = l.DiffusePower;
        lightComponent.color = diffuseColor;
        lightComponent.spotAngle = l.ConeAngle;

        if (l.Type == BTL.LightType.Directional && lightComponent.type != LightType.Directional) lightComponent.type = LightType.Directional;
        else if (l.Type == BTL.LightType.Spot && lightComponent.type != LightType.Spot) lightComponent.type = LightType.Spot;
        else if (l.Type == BTL.LightType.Point && lightComponent.type != LightType.Point) lightComponent.type = LightType.Point;
    }
}
