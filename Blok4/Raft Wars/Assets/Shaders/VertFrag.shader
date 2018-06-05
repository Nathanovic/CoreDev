Shader "Custom/VertFrag"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata//kan ook gewoon in de vert meegegeven worden: (vert: POSITION, uv : TEXTCOORD0) - het gaat om de semantics!
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;

				//custom values (bad GPU : max 4, modern GPU : max 7)
				float3 worldPos : TEXTCOORD1;
				float3 worldNormal : TEXTCOORD2;
				float4 screenPos : TEXTCOORD3;
				float3 worldRefl : TEXTCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			//uv doorgeven
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				//worldPos:
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				//worldNNormal: (normal richting van huidige vertex)
				//o.worldNormal = normalize(mul(unity_objectToWorld, v.normal));

				//screenPos:
				o.screenPos = ComputeGrabScreenPos(o.vertex);

				return o;
			}

			//fragments aanpassen
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
