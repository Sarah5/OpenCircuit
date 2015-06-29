
Shader "Voxel/PhysicalTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TexWallDif ("Diffuse Wall Texture", 2D) = "surface" {}
		_TexFlrDif ("Diffuse Floor Texture", 2D) = "surface" {}
		_TexCeilDif ("Diffuse Ceiling Texture", 2D) = "surface" {}
		_TexWallNorm ("Normal Wall Texture", 2D) = "surface" {}
		_TexFlrNorm ("Normal Floor Texture", 2D) = "surface" {}
		_TexCeilNorm ("Normal Ceiling Texture", 2D) = "surface" {}
		_Sigma ("Blend Tightness", Range(0, 0.57)) = 0.5
		_Blend ("Blend Sharpness", Float) = 4
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _TexWallDif;
		sampler2D _TexFlrDif;
		sampler2D _TexCeilDif;
		sampler2D _TexWallNorm;
		sampler2D _TexFlrNorm;
		sampler2D _TexCeilNorm;
		float _Sigma;
		float _Blend;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			float2 uv = float2(IN.worldPos.x, IN.worldPos.y);
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			
			

			float3 worldNormal = normalize(IN.worldNormal);
			float bx = pow(max(0, abs(worldNormal.x) -_Sigma), _Blend);
			float bz = pow(max(0, abs(worldNormal.z) -_Sigma), _Blend);
			float by = pow(max(0, abs(worldNormal.y) -_Sigma), _Blend);
			if (worldNormal.y < 0) by = -by;
			
			float bmag = bx +bz +abs(by);
			fixed4 col = fixed4((
				bx *tex2D(_TexWallDif, IN.worldPos.zy) +
				bz *tex2D(_TexWallDif, IN.worldPos.xy) +
				((by > 0)?by *tex2D(_TexFlrDif, IN.worldPos.xz):
				-by *tex2D(_TexCeilDif, IN.worldPos.zx))
				) /bmag);
			//fixed3 norm = 
			o.Albedo = col.rgb;
			//o.Normal = UnpackNormal((
			//	bx *((tex2D(_TexWallNorm, IN.worldPos.zy))) +
			//	bz *((tex2D(_TexWallNorm, IN.worldPos.xy))) +
			//	((by > 0)?by *((tex2D(_TexFlrNorm, IN.worldPos.xz))):
			//	-by *((tex2D(_TexCeilNorm, IN.worldPos.zx))))
			//	) /bmag);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
