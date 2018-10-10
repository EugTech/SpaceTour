Shader "Custom/TransFlipNormal" {
	
		Properties
		{
			_MainTex("Albedo Texture", 2D) = "white" {}
			_TintColor("Tint Color", Color) = (1,1,1,1)
			_Transparency("Transparency", Range(0.0,1.0)) = 0.25
			_CutoutThresh("Cutout Threshold", Range(0.0,1.0)) = 0.2
			_Distance("Distance", Float) = 1
			_Amplitude("Amplitude", Float) = 1
			_Speed("Speed", Float) = 1
			_Amount("Amount", Range(0.0,1.0)) = 1
		}

			SubShader
		{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _TintColor;
		float _Transparency;
		float _CutoutThresh;
		float _Distance;
		float _Amplitude;
		float _Speed;
		float _Amount;

		

		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.color.xyz = v.normal * 0.5 + 0.5;
			o.color.w = 1.0;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{

			
			// sample the texture
			fixed4 col = i.color
		col.a = _Transparency;
		clip(col.r - _CutoutThresh);
		return col;
		}
			ENDCG
		}
		}
	}
