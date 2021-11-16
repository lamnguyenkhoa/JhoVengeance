Shader "Custom/HoverOutlinePhongEpic"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        outline("Outline Color", Color) = (0, 0, 0, 1)
        outline2("Outline Color 2", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0.01, 1)) = 0.01
    }

        CGINCLUDE
#include "UnityCG.cginc"

            struct appdata //vertIn
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        struct v2f //vertOut
        {
            float4 pos : SV_POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
            UNITY_FOG_COORDS(1)
        };

        float _OutlineWidth;
        float4 outline;
        float4 outline2;


        ENDCG
            SubShader
        {
            Tags { "Queue" = "Transparent" }
            LOD 3000

            Pass
            {
                Cull front
                Zwrite off
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                v2f vert(appdata v)
                {
                    float3 newPos = v.vertex.xyz * (_OutlineWidth + 1.2);
                    float3 normal = normalize(v.normal);
                    newPos += normal * _OutlineWidth;

                    v2f output;
                    UNITY_INITIALIZE_OUTPUT(v2f, output);
                    output.pos = UnityObjectToClipPos(newPos);
                    return output;
                }

                half4 frag(v2f i) : COLOR
                {
                   return outline2;
                }

                ENDCG
            }
            Pass //Render item in outline colour, a bit bigger but behind
            {
                Cull front
                Zwrite off
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                v2f vert(appdata v)
                {
                    v.vertex.xyz = (_OutlineWidth + 1.2) * v.vertex.xyz;
                    v2f output;
                    UNITY_INITIALIZE_OUTPUT(v2f, output);
                    output.pos = UnityObjectToClipPos(v.vertex);
                    return output;
                }

                half4 frag(v2f i) : COLOR
                {
                   return outline;
                }

                ENDCG
            }


            Pass //Render item normally in front
            {
                    /*Based on phong model with blinn-phong approximation as described in workshops*/
                    ZWrite On
                    cull front
                    Lighting On
                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag

                    #include "UnityCG.cginc"
                    #include "UnityLightingCommon.cginc"


                    struct vertIn
                    {
                        float4 vertex : POSITION;
                        float4 normal : NORMAL;
                        float4 color : COLOR;
                        float4 uv : TEXCOORD0;
                    };

                    struct vertOut
                    {
                        float4 vertex : SV_POSITION;
                        float4 color : COLOR;
                        float4 uv : TEXCOORD0;
                        float4 worldVertex : TEXCOORD1;
                        float3 worldNormal : TEXCOORD12;
                    };

                    sampler2D _MainTex;

                    //Vertex
                    vertOut vert(vertIn v)
                    {
                        vertOut o;

                        float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
                        float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.color = v.color;
                        o.uv = v.uv;

                        o.worldVertex = worldVertex;
                        o.worldNormal = worldNormal;

                        return o;
                    }

                    // Implementation of the fragment shader
                    fixed4 frag(vertOut v) : SV_Target
                    {
                        // Sample the texture for the "unlit" colour for this pixel
                        float4 unlitColor = tex2D(_MainTex, v.uv);

                        // Our interpolated normal might not be of length 1
                        float3 interpNormal = normalize(v.worldNormal);

                        // Calculate ambient RGB intensities
                        float Ka = 1;
                        float3 amb = unlitColor.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

                        // Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
                        // (when calculating the reflected ray in our specular component)
                        float fAtt = 1;
                        float Kd = 1;
                        float3 L = _WorldSpaceLightPos0;
                        float LdotN = dot(L, interpNormal);
                        float3 dif = fAtt * _LightColor0 * Kd * unlitColor.rgb * saturate(LdotN);

                        float Ks = 1;
                        float specNSquared = 25;
                        float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);

                        float3 H = normalize(V + L);
                        float3 spe = fAtt * _LightColor0 * Ks * pow(saturate(dot(interpNormal, H)), specNSquared);

                        float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
                        returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
                        returnColor.a = unlitColor.a;

                        return returnColor;
                    }
                    ENDCG
                }
        }
}