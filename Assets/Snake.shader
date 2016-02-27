Shader "d3cr1pt0r/Snake" {
	Properties{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform half4 _Color;

			struct vertexInput {
				half4 vertex : POSITION;
				float3 normal : NORMAL;
				half2 texcoord : TEXCOORD0;
			};

			struct vertexOutput {
				half4 pos : SV_POSITION;
				half2 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tex = v.texcoord;
				o.posWorld = mul(_Object2World, v.vertex);
				o.normalDir = normalize(mul(float4(v.normal, 0.0), _World2Object).xyz);

				return o;
			}

			fixed4 frag(vertexOutput i) : COLOR
			{
				return float4(_Color.rgb, _Color.a);
			}
			ENDCG
		}
	}
}
