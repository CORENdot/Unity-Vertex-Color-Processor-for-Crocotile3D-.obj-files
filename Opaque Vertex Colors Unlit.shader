Shader "Opaque Vertex Colors Unlit" 
{
    Properties 
    {
        _MainTex ("Base Texture", 2D) = "white" {}
    }

    SubShader 
    {
        Lighting Off
        LOD 80

        Tags 
        { 
            "RenderType" = "Opaque" 
        }

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
			#pragma fragmentoption ARB_precision_hint_fastest
            
            #include "UnityCG.cginc"

            sampler2D_half _MainTex;
            fixed4 _MainTex_ST;

            struct appdata 
            {
                fixed3 pos : POSITION;
                fixed3 uv0 : TEXCOORD0;
                fixed3 vColor : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f 
            {
                fixed4 pos : SV_POSITION;
                fixed2 uv0 : TEXCOORD0;
                fixed3 vColor : COLOR0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata IN) 
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO (OUT);

                OUT.pos = UnityObjectToClipPos (IN.pos);
				OUT.uv0 = TRANSFORM_TEX (IN.uv0, _MainTex);
                OUT.vColor = IN.vColor;

                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target 
            {
                return tex2D (_MainTex, IN.uv0) * fixed4 (IN.vColor, 1);
            }

            ENDCG
        }
    }
}
