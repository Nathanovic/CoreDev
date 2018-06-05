Shader "Custom/New"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always//altijd draween

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;

				//custom:
				float3 worldPos : TEXTCOORD1;
				float3 worldNormal : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Alpha;

			uniform float3 _ClipPosition;

			v2f vert (appdata v, float3 normal : NORMAL)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(normal);

				return o;
			} 
			
			fixed4 frag (v2f i) : SV_Target
			{

				// sample the texture
				///fixed4 col = tex2D(_MainTex, i.uv);
				 
				fixed4 r = tex2D(_MainTex, i.worldPos.zy);
				fixed4 gr = tex2D(_MainTex, i.worldPos.xz);
				fixed4 b = tex2D(_MainTex, i.worldPos.xy);

				float3 blend = abs(i.worldNormal);
				blend *= blend;
				fixed4 col = blend.r * r + blend.g * gr + blend.b * b;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				half lightColor = (dot(i.worldNormal, half3(0,1,0)) + 1) * 0.5;
				col += lightColor * 0.25;
				col.a = _Alpha;

				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
				float fresnel = dot(viewDir, i.worldNormal);

				return col + pow(fresnel, 8);
			}
			ENDCG
		}
	}
}
