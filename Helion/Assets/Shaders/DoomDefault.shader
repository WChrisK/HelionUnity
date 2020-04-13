// This is a modified shader.
// Source: https://raw.githubusercontent.com/nubick/unity-utils/master/sources/Assets/Scripts/Shaders/Sprites-Default.shader
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "Doom/Default"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
//		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
//		Tags
//		{
//			"Queue"="Transparent"
//			"IgnoreProjector"="True"
//			"RenderType"="Transparent"
//			"PreviewType"="Plane"
//			"CanUseSpriteAtlas"="True"
//		}

		Tags
		{
		    "RenderType" = "Opaque"
		}

		LOD 200
		Cull Back
//		Lighting Off
//		ZWrite On
//		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;

				// For opaque textures that are missing a color, we do not
				// want to draw it if it has any translucency. Due to some
				// filtering we get bad blending, so this is the hacky
				// workaround for not drawing transparent or blended pixels.
				// It's not perfect, we can almost certainly do better later.
				if (c.a <= 0.9)
				{
				    discard;
				}

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
