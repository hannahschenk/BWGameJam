Shader "Toon"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Main Texture", 2D) = "white" {}
    }

    SubShader
    {
        Pass 
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase

            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;
            sampler2D _MainTex;
            float4 _LightColor0;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            void vert(appdata_full v, out v2f o)
            {
                o.uv = v.texcoord;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex);
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv);
                float ndotl = dot(i.normal, _WorldSpaceLightPos0.xyz);

                float toon = saturate(ndotl / fwidth(ndotl) + 0.5);

                float ambient = ShadeSH9(float4(i.normal, 1));

                float3 lighting = lerp(ambient, _LightColor0.rgb, toon);

                return float4(tex.rgb * _Color.rgb * lighting, 1.0);
            }
            ENDCG
        }

        Pass {
            Tags{ "LightMode" = "ForwardAdd" }
            BlendOp Max

            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdadd

            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;
            sampler2D _MainTex;
            float4 _LightColor0;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            void vert(appdata_full v, out v2f o)
            {
                o.uv = v.texcoord;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv);
                float3 lightDir = _WorldSpaceLightPos0.w == 0 ? _WorldSpaceLightPos0.xyz : normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float ndotl = dot(i.normal, lightDir);

                float toon = saturate(ndotl / fwidth(ndotl) + 0.5);

                float3 lighting = toon * _LightColor0.rgb;

                return float4(tex.rgb * _Color.rgb * lighting, 1.0);
              }
               ENDCG
        }
    }
}