
Shader "Voxel/Volume Grass" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Scale ("Texture Scale", Float) = 1
		_Spacing("Billboard Spacing", Float) = 0.2
		_Depth("Maximum Depth", Float) = 0.2
		_MinFullFlatness("Minimum Flatness for Ground", Range(0.0, 1.0)) = 0.7
		_TexGroundDif ("Diffuse Ground Texture", 2D) = "surface" {}
		_TexGrassDif ("Diffuse Grass Texture", 2D) = "surface" {}
		_CutoffLevel("Alpha Cutoff Level", Range(0.0,1.0)) = 0.5
		[HideInInspector]_ActualCutoffLevel ("Hidden, no touchy", Range(0,0)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
//		Blend SrcAlpha OneMinusSrcAlpha
//		Blend One One // additive blending 
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vertexFunc
		#pragma surface surfaceFunc Standard fullforwardshadows alphatest:_ActualCutoffLevel

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
		float _MinFullFlatness;
		float _CutoffLevel;
		fixed4 _Color;
		
		struct Input {
			float4 pos;
			float3 normal;
			float depth;
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
			if (input.normal.y <= 0)
				return fixed4(0, 0, 0, 0);

			//if (input.pos.x < 30 && input.pos.z < 30)
				//return fixed4(0, 0, 0, 0);

			float3 moddedPos = input.pos;
			moddedPos.x = fmod(moddedPos.x, 1);
			moddedPos.z = fmod(moddedPos.z, 1);
			float3 pos = moddedPos;
			pos.y = 1;
			float amountSame = abs(dot(normalize(viewDir.xz), input.normal.xz));
			float3 bentViewDir = viewDir;
			bentViewDir.y -= amountSame / input.normal.y;
			float3 diffz = bentViewDir / abs(bentViewDir.z);
			float3 diffx = bentViewDir / abs(bentViewDir.x);
			float depth = input.depth;
			//if (input.normal.y < 0.3f)
			//	depth *= 10 * (input.normal.y - 0.2f);


			for (int i = 0; i < 8; ++i) {
				float modz = abs(bentViewDir.z < 0 ?
					myMod(pos.z, _Spacing) :
					_Spacing - myMod(pos.z, _Spacing));
				float modx = abs(bentViewDir.x < 0 ?
					myMod(pos.x, _Spacing) :
					_Spacing - myMod(pos.x, _Spacing));
				float2 uv;
				if (modx /abs(bentViewDir.x) < modz /abs(bentViewDir.z)) {
					pos += modx *diffx;
					uv = float2(pos.z, pos.y);
					uv.x += (pos.x - moddedPos.x + input.pos.x) *(pos.x - moddedPos.x +input.pos.x);
				} else {
					pos += modz *diffz;
					uv = float2(pos.x, pos.y);
					uv.x += (pos.z - moddedPos.z + input.pos.z) *(pos.z - moddedPos.z + input.pos.z);
				}
				//if (pos.y > 0.99f)
					//break;
					//return fixed4(1, 1, 1, 1);
				if (pos.y <= 1 - depth)
					break;
				//uv.y = max(0.99f, uv.y);
				//uv = float2(uv.x, 1);
				fixed4 color = tex2D(_TexGrassDif, TRANSFORM_TEX(uv, _TexGrassDif));
				if (color.a > _CutoffLevel) {
					//return fixed4(1 -pos.y, 0, 0, 1);
					return color;
				}
				pos += bentViewDir *0.0001f;
			}
			if (input.normal.y > _MinFullFlatness && depth > 0.1f *_Depth)
				return tex2D(_TexGroundDif, TRANSFORM_TEX(input.pos.xz - (bentViewDir.xz *(depth / bentViewDir.y)), _TexGroundDif));
			return fixed4(0, 0, 0, 0);
		}
		
		void vertexFunc(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
 			o.pos = v.vertex; 
			o.normal = v.normal;
			o.depth = _Depth *(1 -v.texcoord.y);
        }

		void surfaceFunc(Input IN, inout SurfaceOutputStandard o) {
			float3 viewDir = -normalize(ObjSpaceViewDir(IN.pos));
			//fixed4 dif = fixed4(0, 0, 0, 0);
			//if (IN.normal.y > 0.3f)
			fixed4 dif = getColor(IN, viewDir);
			
			o.Albedo = dif.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = dif.a -_CutoffLevel;
			//o.Normal = float3(1, 1, 1);
			//o.Alpha = height.a +IN.alpha -1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
