
Shader "Voxel/GrassExperiment" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Scale ("Texture Scale", Float) = 1
		_Spacing("Billboard Spacing", Float) = 0.2
		_Depth("Maximum Depth", Float) = 0.2
		_TexGroundDif ("Diffuse Ground Texture", 2D) = "surface" {}
		_TexGrassDif ("Diffuse Grass Texture", 2D) = "surface" {}
		_CutoffLevel ("Alpha Cutoff Level (Leave at 0)", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
//		Blend SrcAlpha OneMinusSrcAlpha
//		Blend One One // additive blending 
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vertexFunc
		#pragma surface surfaceFunc Standard fullforwardshadows alphatest:_CutoffLevel

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _TexGroundDif;
		float4 _TexGroundDif_ST;
		sampler2D _TexGrassDif;
		float4 _TexGrassDif_ST;
		half _Glossiness;
		half _Metallic;
		float _Scale;
		float _Spacing;
		float _Depth;
		fixed4 _Color;
		
		struct Input {
			float4 pos;
			float3 normal;
			INTERNAL_DATA
		};

		float myMod(float numerator, float denominator) {
			float quotient = numerator / denominator;
			float fraction = quotient - ((int)quotient);
			if (fraction < 0)
				fraction += 1;
			return fraction *denominator;
		}

		fixed4 getColor(Input input, float3 viewDir) {
			input.normal = normalize(input.normal);
			float3 pos = input.pos;
			pos.y = 1;
			float amountSame = -dot(normalize(viewDir.xz), input.normal.xz);
			viewDir.y -= amountSame / input.normal.y;
			float3 diff = viewDir / abs(viewDir.z);
			float mod = viewDir.z < 0 ?
				myMod(pos.z, _Spacing) :
				_Spacing -myMod(pos.z, _Spacing);
			pos += abs(mod) *diff;
			for (int i = 0; i < 10; ++i) {
				if (pos.y < 1 -_Depth)
					break;
				float2 uv = pos.xy;
				uv.x += pos.z*pos.z;
				fixed4 color = tex2D(_TexGrassDif, TRANSFORM_TEX(uv, _TexGrassDif));
				if (color.a > 0.5)
					return color;
				pos += _Spacing *diff;
			}
			return tex2D(_TexGroundDif, TRANSFORM_TEX(input.pos.xz -(viewDir.xz *(_Depth /viewDir.y)), _TexGroundDif));
		}
		
		void vertexFunc(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
 			o.pos = v.vertex;
			o.normal = v.normal;
        }

		void surfaceFunc(Input IN, inout SurfaceOutputStandard o) {
			float3 viewDir = -normalize(ObjSpaceViewDir(IN.pos));
			fixed4 dif = getColor(IN, viewDir);
			
			o.Albedo = dif.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = dif.a;
			//o.Normal = float3(1, 1, 1);
			//o.Alpha = height.a +IN.alpha -1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
