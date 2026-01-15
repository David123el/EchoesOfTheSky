Shader "Custom/PixelSaturation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Saturation ("Saturation", Range(0,1)) = 0
        _Fade ("Fade", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _Saturation;
            float _Fade;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = lerp(float3(gray, gray, gray), col.rgb, _Saturation);

                col.a *= _Fade;   // 👈 זה הקריטי
                return col;
            }
            ENDCG
        }
    }
}
