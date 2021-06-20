using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

[InitializeOnLoad]
public class MyRenderSettings : EditorWindow
{
	[MenuItem("Window/Render Settings Temp Fix")]
	static void Init()
	{
		MyRenderSettings renderSettings = (MyRenderSettings)EditorWindow.GetWindow(typeof(MyRenderSettings));
	}
	
	AmbientMode ambientMode
	{
		get { return RenderSettings.ambientMode; }
		set { RenderSettings.ambientMode = value; }
	}
	Color ambientLight
	{
		get { return RenderSettings.ambientLight; }
		set { RenderSettings.ambientLight = value; }
	}
	Color ambientSkyColor
	{
		get { return RenderSettings.ambientSkyColor; }
		set { RenderSettings.ambientSkyColor = value; }
	}
	Color ambientEquatorColor
	{
		get { return RenderSettings.ambientEquatorColor; }
		set { RenderSettings.ambientEquatorColor = value; }
	}
	Color ambientGroundColor
	{
		get { return RenderSettings.ambientGroundColor; }
		set { RenderSettings.ambientGroundColor = value; }
	}
	float ambientSkyboxAmount
	{
		get { return RenderSettings.ambientIntensity; }
		set { RenderSettings.ambientIntensity = value; }
	}
	SphericalHarmonicsL2 ambientProbe
	{
		get { return RenderSettings.ambientProbe; }
		set { RenderSettings.ambientProbe = value; }
	}
	
	DefaultReflectionMode defaultReflectionMode
	{
		get { return RenderSettings.defaultReflectionMode; }
		set { RenderSettings.defaultReflectionMode = value; }
	}
	Cubemap customReflection
	{
		get { return RenderSettings.customReflection; }
		set { RenderSettings.customReflection = value; }
	}
	
	float flareFadeSpeed
	{
		get { return RenderSettings.flareFadeSpeed; }
		set { RenderSettings.flareFadeSpeed = value; }
	}
	float flareStrength
	{
		get { return RenderSettings.flareStrength; }
		set { RenderSettings.flareStrength = value; }
	}
	
	bool fog
	{
		get { return RenderSettings.fog; }
		set { RenderSettings.fog = value; }
	}
	FogMode fogMode
	{
		get { return RenderSettings.fogMode; }
		set { RenderSettings.fogMode = value; }
	}
	Color fogColor
	{
		get { return RenderSettings.fogColor; }
		set { RenderSettings.fogColor = value; }
	}
	float fogDensity
	{
		get { return RenderSettings.fogDensity; }
		set { RenderSettings.fogDensity = value; }
	}
	float fogStartDistance
	{
		get { return RenderSettings.fogStartDistance; }
		set { RenderSettings.fogStartDistance = value; }
	}
	float fogEndDistance
	{
		get { return RenderSettings.fogEndDistance; }
		set { RenderSettings.fogEndDistance = value; }
	}
	
	float haloStrength
	{
		get { return RenderSettings.haloStrength; }
		set { RenderSettings.haloStrength = value; }
	}
	Material skybox
	{
		get { return RenderSettings.skybox; }
		set { RenderSettings.skybox = value; }
	}
	
	void OnGUI()
	{
		GUILayout.Label("Ambient and skybox", EditorStyles.boldLabel);
		ambientMode = (AmbientMode)EditorGUILayout.EnumPopup("Ambient Mode", ambientMode);
		ambientLight = EditorGUILayout.ColorField("Ambient Light Color", ambientLight);
		ambientSkyColor = EditorGUILayout.ColorField("Ambient Sky Color", ambientSkyColor);
		ambientEquatorColor = EditorGUILayout.ColorField("Ambient Equator Color", ambientEquatorColor);
		ambientGroundColor = EditorGUILayout.ColorField("Ambient Ground Color", ambientGroundColor);
		ambientSkyboxAmount = EditorGUILayout.FloatField("Ambient Skybox Amount", ambientSkyboxAmount);
		//ambientProbe = (SphericalHarmonicsL2)EditorGUILayout.ObjectField(ambientProbe, typeof(SphericalHarmonicsL2), true);   //Doesnt seem t0o work, skipped this
		haloStrength = EditorGUILayout.FloatField("Halo Strength", haloStrength);
		skybox = (Material)EditorGUILayout.ObjectField("Skybox", skybox, typeof(Material), true);
		
		GUILayout.Label("Reflection", EditorStyles.boldLabel);
		defaultReflectionMode = (DefaultReflectionMode)EditorGUILayout.EnumPopup("Default Reflection Mode", defaultReflectionMode);
		customReflection = (Cubemap)EditorGUILayout.ObjectField("Custom Reflection", customReflection, typeof(Cubemap), true);
		
		GUILayout.Label("Flare", EditorStyles.boldLabel);
		flareFadeSpeed = EditorGUILayout.FloatField("Flare Fade Speed", flareFadeSpeed);
		flareStrength = EditorGUILayout.FloatField("Flare Strength", flareStrength);
		
		GUILayout.Label("Fog", EditorStyles.boldLabel);
		fog = EditorGUILayout.Toggle("Fog enabled", fog);
		fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", fogMode);
		fogColor = EditorGUILayout.ColorField("Fog Color", fogColor);
		fogDensity = EditorGUILayout.FloatField("Fog Density", fogDensity);
		fogStartDistance = EditorGUILayout.FloatField("Fog Start Distance", fogStartDistance);
		fogEndDistance = EditorGUILayout.FloatField("Fog End Distance", fogEndDistance);
	}
}