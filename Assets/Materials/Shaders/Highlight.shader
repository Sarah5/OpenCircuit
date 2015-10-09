Shader "Unlit/Highlight"
{
	Properties {
		_Color("Color", Color) = (0,0,0,0)
		_CutoffLevel("Alpha Cutoff Level", Range(0,1)) = 0.2
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		//		Blend SrcAlpha OneMinusSrcAlpha
		//		Blend One One // additive blending 
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vertexFunc
		#pragma surface surfaceFunc Standard fullforwardshadows alphatest:_CutoffLevel

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _Color;

		struct Input {
			float4 pos;
			float3 normal;
			INTERNAL_DATA
		};

		void vertexFunc(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.pos = v.vertex;
			o.normal = v.normal;
		}

		void surfaceFunc(Input IN, inout SurfaceOutputStandard o) {
			float3 viewDir = normalize(ObjSpaceViewDir(IN.pos));
			IN.normal = normalize(IN.normal);

			o.Albedo = _Color.rgb;
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = 1 -dot(viewDir, IN.normal);
			o.Normal = float3(0, 0, 0);
			//o.Alpha = height.a +IN.alpha -1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
