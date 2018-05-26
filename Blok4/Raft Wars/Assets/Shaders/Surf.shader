Shader "Custom/Surf" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "blue" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//add shadow to changed vertices
		#pragma surface surf Standard fullforwardshadows vertext:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting 
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;	//direction from which the pixel is viewed
			float4 screenPos;//position of pixel in screen space
			float3 worldPos;//contains world spacee position
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + half2(_Time.x, 0)) * _Color;
			//half2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			//half2 worldUV = IN.worldPos.xz;
			//fixed4 c = tex2D (_MainTex, screenUV) * _Color;
			//fixed4 c = tex2D (_MainTex, worldUV) * _Color;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			half fresnel = 1 - dot(IN.viewDir, o.Normal);
			o.Emission = fresnel * abs(sin(_Time.y));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
