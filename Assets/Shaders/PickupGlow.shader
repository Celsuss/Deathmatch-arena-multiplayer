Shader "Custom/PickupGlow" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_ColorTint ("Color Tint", Color) = (1, 1, 1, 1)
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimStrength ("Rim Strength", Range(0.0, 10.0)) = 5.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		//#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float4 color : Color;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float3 _ColorTint;
		float4 _RimColor;
		float _RimStrength;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//IN.color = _ColorTint;
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _ColorTint;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

			o.Emission = (_RimColor.rgb * pow(rim, _RimStrength));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
