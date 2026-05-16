Shader "Custom/RainbowBridgeFlowURP"
{
    Properties
    {
        _BridgeWidth ("Bridge Width", Float) = 12
        _CenterX ("Center X", Float) = 0

        _Saturation ("Saturation", Range(0, 1)) = 0.85
        _Brightness ("Brightness", Range(0, 3)) = 1.2

        _BandFrequency ("Flow Band Frequency", Float) = 0.08
        _BandWidth ("Flow Band Width", Range(0.01, 0.5)) = 0.12
        _BandStrength ("Flow Band Strength", Range(0, 3)) = 0.8

        _FlowOffset ("Flow Offset", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "Rainbow Bridge"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _BridgeWidth;
            float _CenterX;
            float _Saturation;
            float _Brightness;
            float _BandFrequency;
            float _BandWidth;
            float _BandStrength;
            float _FlowOffset;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            float3 HueToRGB(float h)
            {
                float3 rgb = abs(frac(h + float3(0.0, 0.666666, 0.333333)) * 6.0 - 3.0) - 1.0;
                return saturate(rgb);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

                output.positionWS = positionWS;
                output.positionHCS = TransformWorldToHClip(positionWS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float hue = saturate((input.positionWS.x - _CenterX) / max(_BridgeWidth, 0.001) + 0.5);

                float3 rainbow = HueToRGB(hue);
                rainbow = lerp(float3(1, 1, 1), rainbow, _Saturation);
                rainbow *= _Brightness;

                float flow = frac(input.positionWS.z * _BandFrequency + _FlowOffset);
                float distToBand = abs(flow - 0.5);
                float band = 1.0 - smoothstep(0.0, _BandWidth, distToBand);

                float3 finalColor = rainbow + band * _BandStrength;

                return half4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}