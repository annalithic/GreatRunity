using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
public class SoulsLightComponent : MonoBehaviour
{

    public Color specularColor;
    public Color shadowColor;
    public BTL.Light l;

    Color ToUnityColor(System.Drawing.Color c) { return new Color(c.R / 255f, c.G / 255f, c.B / 255f); }
    System.Drawing.Color ToSystemColor(Color c) { return System.Drawing.Color.FromArgb((byte)(c.r * 255), (byte)(c.g * 255), (byte)(c.b * 255)); }

    public void Import(BTL.Light light) {

        name = light.Name;
        transform.position = new Vector3(light.Position.X, light.Position.Y, light.Position.Z);

        specularColor = ToUnityColor(light.SpecularColor);
        shadowColor = ToUnityColor(light.ShadowColor);

        Light lightComponent = GetComponent<Light>();
        lightComponent.range = light.Radius;
        lightComponent.intensity = light.DiffusePower;
        lightComponent.color = new Color(light.DiffuseColor.R / 255f, light.DiffuseColor.G / 255f, light.DiffuseColor.B / 255f);
        lightComponent.spotAngle = light.ConeAngle;

        this.l = light;
    }

    public BTL.Light Export() {
        l.Name = name;
        l.Position = new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z);

        l.SpecularColor = ToSystemColor(specularColor);
        l.ShadowColor = ToSystemColor(shadowColor);

        Light lightComponent = GetComponent<Light>();
        l.Radius = lightComponent.range;
        l.DiffusePower = lightComponent.intensity;
        l.DiffuseColor = ToSystemColor(lightComponent.color);
        l.ConeAngle = lightComponent.spotAngle;
        return l;
    }
}
