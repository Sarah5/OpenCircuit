
Shader "Voxel/PhysicalTest" {
	Properties {
//		_Color ("Color", Color) = (1,1,1,1)
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Scale("Texture Scale", Float) = 1
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
		Blend SrcAlpha OneMinusSrcAlpha
//		Blend One One // additive blending 
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vertexFunc
		#pragma surface surfaceFunc Standard fullforwardshadows

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
		float _Scale;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		struct Input {
			float2 uv_MainTex;
			float4 pos;
			float3 b;
			float alpha;
			INTERNAL_DATA
		};
		
		fixed4 computeDiffuse(Input i, float blendMag) {
			return (
				i.b.x *tex2D(_TexWallDif, i.pos.zy *_Scale) +
				i.b.z *tex2D(_TexWallDif, i.pos.xy *_Scale) +
				((i.b.y > 0)?i.b.y *tex2D(_TexFlrDif, i.pos.xz *_Scale):
				-i.b.y *tex2D(_TexCeilDif, i.pos.zx *_Scale))
				) /blendMag;
		}
		
		fixed4 computeHeight(Input i, float blendMag) {
			return (
				i.b.x *tex2D(_TexWallNorm, i.pos.zy *_Scale) +
				i.b.z *tex2D(_TexWallNorm, i.pos.xy *_Scale) +
				((i.b.y > 0)?i.b.y *tex2D(_TexFlrNorm, i.pos.xz *_Scale):
				-i.b.y *tex2D(_TexCeilNorm, i.pos.zx *_Scale))
				) /blendMag;
		}
		
		void vertexFunc(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
 			o.pos = v.vertex;
            
			o.b.x = pow(max(0, abs(v.normal.x) -_Sigma), _Blend);
			o.b.z = pow(max(0, abs(v.normal.z) -_Sigma), _Blend);
			o.b.y = pow(max(0, abs(v.normal.y) -_Sigma), _Blend);
			
			if (v.normal.y < 0) o.b.y = -o.b.y;
			
			o.alpha = v.texcoord.y;
        }

		void surfaceFunc(Input IN, inout SurfaceOutputStandard o) {
			float bmag = IN.b.x +IN.b.z +abs(IN.b.y);
			fixed4 dif = computeDiffuse(IN, bmag);
			fixed4 height = computeHeight(IN, bmag);
			
			o.Albedo = dif.rgb;
			o.Normal = UnpackNormal(height);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = IN.alpha;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
